using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mirage.Net;
using Mirage.Net.Protocol.FromServer;
using Mirage.Server.Maps;
using Mirage.Server.Net;
using Mirage.Server.Npcs;
using Mirage.Server.Repositories;
using Mirage.Server.Repositories.Characters;
using Mirage.Shared.Constants;
using Mirage.Shared.Data;

namespace Mirage.Server.Players;

public sealed class Player
{
    private const float RegenIntervalInSeconds = 10.0f;

    private readonly NetworkConnection _connection;
    private readonly ILogger<Player> _logger;
    private readonly IPlayerService _players;
    private readonly IMapService _mapService;
    private readonly ICharacterRepository _characterRepository;
    private float _regenTimer;

    public int Id { get; }
    public string Address { get; }
    public CharacterInfo Character { get; }
    public PlayerInventory Inventory { get; }
    public int AttackTimer { get; set; }
    public bool CastedSpell { get; set; }
    public bool InParty { get; set; }
    public bool IsPartyStarter { get; set; }
    public Player? PartyMember { get; set; }
    public Map Map { get; private set; }
    public Player? TargetPlayer { get; set; }
    public Npc? TargetNpc { get; set; }

    public Player(NetworkConnection connection, CharacterInfo character, Map map, IServiceProvider services)
    {
        _connection = connection;
        _logger = services.GetRequiredService<ILogger<Player>>();
        _players = services.GetRequiredService<IPlayerService>();
        _mapService = services.GetRequiredService<IMapService>();
        _characterRepository = services.GetRequiredService<ICharacterRepository>();

        Id = connection.Id;
        Address = connection.Address;
        Character = character;
        Inventory = new PlayerInventory(this, services.GetRequiredService<IRepository<ItemInfo>>());

        Tell($"Welcome to {Options.GameName}!", ColorCode.BrightBlue);
        Tell("Type /help for help on commands. Use arrow keys to move, hold down shift to run, and use ctrl to attack.", ColorCode.Cyan);

        if (File.Exists("Motd.txt"))
        {
            var motd = File.ReadAllText("Motd.txt");
            if (!string.IsNullOrWhiteSpace(motd))
            {
                Tell("MOTD: " + motd.Trim(), ColorCode.BrightCyan);
            }
        }

        SendWhosOnline();

        Inventory.SendToPlayer();

        Map = map;
        Map.Add(this);

        Send<EnterGameCommand>();

        var color = Character.AccessLevel <= AccessLevel.Moderator ? ColorCode.JoinLeftColor : ColorCode.White;

        _players.Send(new ChatCommand($"{Character.Name} has joined the game!", color));
    }

    public void Update(float deltaTime)
    {
        UpdateRegen(deltaTime);
    }

    private void UpdateRegen(float deltaTime)
    {
        _regenTimer += deltaTime;
        if (_regenTimer < RegenIntervalInSeconds)
        {
            return;
        }

        _regenTimer -= RegenIntervalInSeconds;

        var newHealth = Math.Clamp(Character.Health + Character.HealthRegen, 0, Character.MaxHealth);
        var newMana = Math.Clamp(Character.Mana + Character.ManaRegen, 0, Character.MaxMana);
        var newStamina = Math.Clamp(Character.Stamina + Character.StaminaRegen, 0, Character.MaxStamina);

        if (newHealth == Character.Health && newMana == Character.Mana && newStamina == Character.Stamina)
        {
            return;
        }

        Character.Health = newHealth;
        Character.Mana = newMana;
        Character.Stamina = newStamina;

        SendVitals();
    }

    public void Destroy()
    {
        // TODO: Implement boot map logic...

        if (InParty && PartyMember is not null)
        {
            InParty = false;

            PartyMember.Tell($"{Character.Name} has left, disbanning party.", ColorCode.Pink);
            PartyMember = null;
        }

        Inventory.UpdateCharacter();

        _characterRepository.Save(Character);

        var color = Character.AccessLevel <= AccessLevel.Moderator ? ColorCode.JoinLeftColor : ColorCode.White;

        _players.Send(new ChatCommand($"{Character.Name} has left!", color));

        _logger.LogInformation("{CharacterName} has left", Character.Name);

        Map.Remove(this);
    }

    public void WarpTo(Map map, int x, int y)
    {
        if (Map == map)
        {
            Character.X = x;
            Character.Y = y;

            Map.Send(new SetActorPositionCommand(Id, Character.Direction, x, y));
            return;
        }

        Character.Map = Map.FileName;

        // var shopInfo = ShopRepository.Get(oldShopId);
        // if (shopInfo is not null && !string.IsNullOrWhiteSpace(shopInfo.LeaveSay))
        // {
        //     Tell($"{shopInfo.Name} says, '{shopInfo.LeaveSay}'", ColorCode.SayColor);
        // }

        // Remove the player from the current map.
        Map.Remove(this);

        // Add the player to the destination map.
        Map = map;
        Map.Add(this);

        // shopInfo = ShopRepository.Get(newShopId);
        // if (shopInfo is not null && !string.IsNullOrWhiteSpace(shopInfo.JoinSay))
        // {
        //     Tell($"{shopInfo.Name} says, '{shopInfo.JoinSay}'", ColorCode.SayColor);
        // }
    }

    public void CheckLevelUp()
    {
        if (Character.Exp < Character.RequiredExp)
        {
            return;
        }

        while (Character.Exp >= Character.RequiredExp)
        {
            var statPoints = Math.Clamp(Character.Speed / 10, 1, 3);

            Character.Level++;
            Character.StatPoints += statPoints;
            Character.Exp -= Character.RequiredExp;
        }

        _players.Send(new ChatCommand($"{Character.Name} has reached level {Character.Level}!", ColorCode.Brown));

        Tell($"You have gained a level! You now have {Character.StatPoints} stat points to distribute.", ColorCode.BrightBlue);
    }

    public int CalculateDamage()
    {
        var damage = Character.Strength / 2;
        if (damage <= 0)
        {
            damage = 1;
        }

        var weapon = Inventory.Equipment.Weapon;
        if (weapon is null)
        {
            return damage;
        }

        damage += weapon.Item.Damage;

        Inventory.Equipment.ReduceDurability(EquipmentType.Weapon);

        return damage;
    }

    public int CalculateProtection()
    {
        var protection = Character.Defense / 5;

        var armor = Inventory.Equipment.Armor;
        if (armor is not null)
        {
            protection += armor.Item.Protection;

            Inventory.Equipment.ReduceDurability(EquipmentType.Armor);
        }

        var helmet = Inventory.Equipment.Helmet;
        if (helmet is null)
        {
            return protection;
        }

        protection += helmet.Item.Protection;

        Inventory.Equipment.ReduceDurability(EquipmentType.Helmet);

        return protection;
    }

    public bool TryCriticalHit()
    {
        if (Inventory.Equipment.Weapon is null)
        {
            return false;
        }

        var randomRoll = Random.Shared.Next(0, 2);
        if (randomRoll != 1)
        {
            return false;
        }

        var criticalHitChance = Random.Shared.Next(0, 100) + 1;

        return criticalHitChance <= Character.CriticalHitRate;
    }

    public bool TryBlockHit([NotNullWhen(true)] out PlayerEquipmentSlot? shield)
    {
        shield = Inventory.Equipment.Shield;
        if (shield is null)
        {
            return false;
        }

        var randomRoll = Random.Shared.Next(0, 2);
        if (randomRoll != 1)
        {
            return false;
        }

        var blockChance = Random.Shared.Next(1, 100);

        return blockChance <= Character.BlockRate;
    }

    public void Attack(Npc npc)
    {
        var damage = CalculateDamage();
        if (damage < 0)
        {
            return;
        }

        npc.Hurt(this, damage);
    }

    public void GrantExperience(int experience)
    {
        var partyMember = PartyMember;

        if (!InParty)
        {
            Character.Exp += experience;

            Tell($"You have gained {experience} experience points.", ColorCode.BrightBlue);
        }
        else
        {
            experience = Math.Min(1, experience / 2);

            Character.Exp += experience;

            Tell($"You have gained {experience} party experience points.", ColorCode.BrightBlue);

            if (partyMember is not null)
            {
                partyMember.Character.Exp += experience;
                partyMember.Tell($"You have gained {experience} party experience points.", ColorCode.BrightBlue);
            }
        }

        CheckLevelUp();

        if (InParty && partyMember is not null)
        {
            partyMember.CheckLevelUp();
        }
    }

    public void Tell(string message, int color)
    {
        Send(new ChatCommand(message, color));
    }

    private void DropEquipment(PlayerEquipmentSlot? slot)
    {
        if (slot is null)
        {
            return;
        }

        Map.SpawnItem(
            slot.Item.Id,
            Character.X,
            Character.Y,
            slot.Durability, 1);
    }

    public void Kill(int experienceLost)
    {
        DropEquipment(Inventory.Equipment.Weapon);
        DropEquipment(Inventory.Equipment.Armor);
        DropEquipment(Inventory.Equipment.Helmet);
        DropEquipment(Inventory.Equipment.Shield);

        Inventory.Equipment.Clear();

        if (experienceLost == 0)
        {
            Tell("You lost no experience points.", ColorCode.BrightRed);
        }
        else
        {
            Character.Exp -= experienceLost;

            Tell($"You lost {experienceLost} experience points.", ColorCode.BrightRed);
        }

        Map.Remove(this);

        var map = _mapService.GetByName(Options.StartMapName);
        if (map is null)
        {
            return;
        }

        WarpTo(map, Options.StartX, Options.StartY);

        Character.Health = Character.MaxHealth;
        Character.Mana = Character.MaxMana;
        Character.Stamina = Character.MaxStamina;

        Map.Add(this);
    }

    public void Send<TPacket>() where TPacket : IPacket<TPacket>, new()
    {
        _connection.Send<TPacket>();
    }

    public void Send<TPacket>(TPacket packet) where TPacket : IPacket<TPacket>
    {
        _connection.Send(packet);
    }

    public void Send(byte[] bytes)
    {
        _connection.Send(bytes);
    }

    public void SendVitals()
    {
        Send(new UpdateActorVitalsCommand(Id,
            Character.MaxHealth, Character.Health,
            Character.MaxMana, Character.Mana,
            Character.MaxStamina, Character.Stamina));
    }

    public void SendWhosOnline()
    {
        var playerNames = _players.Where(x => x != this)
            .Select(x => x.Character.Name)
            .ToList();

        var message = string.Join(", ", playerNames);

        message = message.Length == 0
            ? "There are no other players online."
            : $"There are {playerNames.Count} other players online: {message}";

        Tell(message, ColorCode.WhoColor);
    }

    public void Disconnect(string message)
    {
        _connection.Disconnect(message);
    }
}