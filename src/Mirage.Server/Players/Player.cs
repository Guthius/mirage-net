using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mirage.Net;
using Mirage.Net.Protocol.FromServer;
using Mirage.Net.Protocol.FromServer.New;
using Mirage.Server.Maps;
using Mirage.Server.Net;
using Mirage.Server.Npcs;
using Mirage.Server.Repositories;
using Mirage.Server.Repositories.Characters;
using Mirage.Server.Repositories.Jobs;
using Mirage.Server.Repositories.Spells;
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
    private readonly IRepository<ItemInfo> _itemRepository;
    private readonly IJobRepository _jobRepository;
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
        _itemRepository = services.GetRequiredService<IRepository<ItemInfo>>();
        _jobRepository = services.GetRequiredService<IJobRepository>();

        Id = connection.Id;
        Address = connection.Address;
        Character = character;
        Inventory = new PlayerInventory(character);

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

        Map = map;
        Map.Add(this);

        // SendSpells();
        // Send(new PlayerInventory(Character.Inventory.Skip(1).ToArray()));
        // SendEquipment();

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

        var newHealth = Math.Clamp(Character.HP + Character.HPRegen, 0, Character.MaxHP);
        var newMana = Math.Clamp(Character.MP + Character.MPRegen, 0, Character.MaxMP);
        var newStamina = Math.Clamp(Character.SP + Character.SPRegen, 0, Character.MaxSP);

        if (newHealth == Character.HP && newMana == Character.MP && newStamina == Character.SP)
        {
            return;
        }

        Character.HP = newHealth;
        Character.MP = newMana;
        Character.SP = newStamina;

        SendVitals();
    }

    public void Destroy()
    {
        // TODO:
        // if (Map.Info.BootMapId > 0)
        // {
        //     Character.X = Map.Info.BootX;
        //     Character.Y = Map.Info.BootY;
        //     Character.MapId = Map.Info.BootMapId;
        // }

        if (InParty && PartyMember is not null)
        {
            InParty = false;

            PartyMember.Tell($"{Character.Name} has left, disbanning party.", ColorCode.Pink);
            PartyMember = null;
        }

        _characterRepository.Save(Character);

        var color = Character.AccessLevel <= AccessLevel.Moderator ? ColorCode.JoinLeftColor : ColorCode.White;

        _players.Send(new ChatCommand($"{Character.Name} has left!", color));

        _logger.LogInformation("{CharacterName} has left", Character.Name);

        Map.Remove(this);
    }

    public void ClearInventorySlot(int slot)
    {
        var slotInfo = Character.Inventory[slot];

        var itemInfo = _itemRepository.Get(slotInfo.ItemId);
        if (itemInfo is null)
        {
            return;
        }

        switch (itemInfo.Type)
        {
            case ItemType.Weapon:
                if (slot == Character.WeaponSlot)
                {
                    Character.WeaponSlot = 0;
                    SendEquipment();
                }

                break;

            case ItemType.Armor:
                if (slot == Character.ArmorSlot)
                {
                    Character.ArmorSlot = 0;
                    SendEquipment();
                }

                break;

            case ItemType.Helmet:
                if (slot == Character.HelmetSlot)
                {
                    Character.HelmetSlot = 0;
                    SendEquipment();
                }

                break;

            case ItemType.Shield:
                if (slot == Character.ShieldSlot)
                {
                    Character.ShieldSlot = 0;
                    SendEquipment();
                }

                break;
        }

        slotInfo.ItemId = string.Empty;
        slotInfo.Quantity = 0;
        slotInfo.Durability = 0;

        SendInventoryUpdate(slot);
    }

    public void GiveItem(string itemId, int quantity)
    {
        var itemInfo = _itemRepository.Get(itemId);
        if (itemInfo is null)
        {
            return;
        }

        var slot = GetFreeInventorySlot(itemInfo);
        if (slot == 0)
        {
            Tell("Your inventory is full.", ColorCode.BrightRed);
            return;
        }

        Character.Inventory[slot].ItemId = itemId;
        Character.Inventory[slot].Quantity += quantity;

        if (itemInfo.Type is ItemType.Armor or ItemType.Weapon or ItemType.Helmet or ItemType.Shield)
        {
            Character.Inventory[slot].Durability = itemInfo.Data1;
        }

        SendInventoryUpdate(slot);
    }

    public void TakeItem(string itemId, int quantity = 0)
    {
        var itemInfo = _itemRepository.Get(itemId);
        if (itemInfo is null)
        {
            return;
        }

        for (var slot = 1; slot <= Limits.MaxInventory; slot++)
        {
            var slotInfo = Character.Inventory[slot];
            if (slotInfo.ItemId != itemInfo.Id)
            {
                continue;
            }

            if (itemInfo.Type == ItemType.Currency && quantity < slotInfo.Quantity)
            {
                slotInfo.Quantity -= quantity;

                SendInventoryUpdate(slot);

                return;
            }

            ClearInventorySlot(slot);

            return;
        }
    }

    public void UseItem(int slot)
    {
        if (slot is < 1 or > Limits.MaxInventory)
        {
            return;
        }

        var itemInfo = _itemRepository.Get(Character.Inventory[slot].ItemId);
        if (itemInfo is null)
        {
            return;
        }

        switch (itemInfo.Type)
        {
            case ItemType.Armor:
            case ItemType.Weapon:
            case ItemType.Helmet:
            case ItemType.Shield:
                UseEquipment(slot, itemInfo);
                break;

            case ItemType.PotionAddHp:
            case ItemType.PotionAddMp:
            case ItemType.PotionAddSp:
            case ItemType.PotionSubHp:
            case ItemType.PotionSubMp:
            case ItemType.PotionSubSp:
                UsePotion(slot, itemInfo.Type, itemInfo.Data1);
                break;

            case ItemType.Spell:
                UseSpell(slot, itemInfo);
                break;
        }
    }

    private void UseEquipment(int inventorySlot, ItemInfo itemInfo)
    {
        switch (itemInfo.Type)
        {
            case ItemType.Armor:
                if (inventorySlot != Character.ArmorSlot)
                {
                    if (Character.Defense < itemInfo.Data2)
                    {
                        Tell($"Your defense is to low to wear this armor!  Required DEF ({itemInfo.Data2})", ColorCode.BrightRed);
                        return;
                    }

                    Character.ArmorSlot = inventorySlot;
                }
                else
                {
                    Character.ArmorSlot = 0;
                }

                SendEquipment();
                break;

            case ItemType.Weapon:
                if (inventorySlot != Character.WeaponSlot)
                {
                    if (Character.Strength < itemInfo.Data2)
                    {
                        Tell($"Your strength is to low to wear this armor!  Required STR ({itemInfo.Data2})", ColorCode.BrightRed);
                        return;
                    }

                    Character.WeaponSlot = inventorySlot;
                }
                else
                {
                    Character.WeaponSlot = 0;
                }

                SendEquipment();
                break;

            case ItemType.Helmet:
                if (inventorySlot != Character.HelmetSlot)
                {
                    if (Character.Speed < itemInfo.Data2)
                    {
                        Tell($"Your speed coordination is to low to wear this helmet!  Required SPEED ({itemInfo.Data2})", ColorCode.BrightRed);
                        return;
                    }

                    Character.HelmetSlot = inventorySlot;
                }
                else
                {
                    Character.HelmetSlot = 0;
                }

                SendEquipment();
                break;

            case ItemType.Shield:
                Character.ShieldSlot = inventorySlot != Character.ShieldSlot ? inventorySlot : 0;
                SendEquipment();
                break;
        }
    }

    private void UsePotion(int inventorySlot, ItemType itemType, int value)
    {
        switch (itemType)
        {
            case ItemType.PotionAddHp:
                Character.HP += value;
                ClearInventorySlot(inventorySlot);
                SendVitals();
                break;

            case ItemType.PotionAddMp:
                Character.MP += value;
                ClearInventorySlot(inventorySlot);
                SendVitals();
                break;

            case ItemType.PotionAddSp:
                Character.SP += value;
                ClearInventorySlot(inventorySlot);
                SendVitals();
                break;

            case ItemType.PotionSubHp:
                Character.HP -= value;
                ClearInventorySlot(inventorySlot);
                SendVitals();
                break;

            case ItemType.PotionSubMp:
                Character.MP -= value;
                ClearInventorySlot(inventorySlot);
                SendVitals();
                break;

            case ItemType.PotionSubSp:
                Character.SP -= value;
                ClearInventorySlot(inventorySlot);
                SendVitals();
                break;
        }
    }

    private void UseSpell(int inventorySlot, ItemInfo itemInfo)
    {
        var spellInfo = SpellRepository.Get(itemInfo.Data1);
        if (spellInfo is null)
        {
            Tell("This scroll is not connected to a spell, please inform an admin!", ColorCode.White);
            return;
        }

        if (!string.IsNullOrEmpty(spellInfo.RequiredClassId) && spellInfo.RequiredClassId != Character.JobId)
        {
            Tell($"This spell can only be learned by a {_jobRepository.GetName(spellInfo.RequiredClassId)}.", ColorCode.White);
            return;
        }

        if (spellInfo.RequiredLevel > Character.Level)
        {
            Tell($"You must be level {spellInfo.RequiredLevel} to learn this spell.", ColorCode.White);
            return;
        }

        var spellSlot = GetFreeSpellSlot();
        if (spellSlot <= 0)
        {
            Tell("You have learned all that you can learn!", ColorCode.BrightRed);
            return;
        }

        ClearInventorySlot(inventorySlot);

        if (HasSpell(spellInfo.Id))
        {
            Tell("You have already learned this spell! The spells crumbles into dust.", ColorCode.BrightRed);
            return;
        }

        Character.SpellIds[spellSlot] = spellInfo.Id;

        Tell("You study the spell carefully...", ColorCode.Yellow);
        Tell("You have learned a new spell!", ColorCode.White);
    }

    public void DropItem(int inventorySlot, int quantity = 0)
    {
        // if (inventorySlot is <= 0 or > Limits.MaxInventory)
        // {
        //     return;
        // }
        //
        // var slotInfo = Character.Inventory[inventorySlot];
        //
        // var itemId = slotInfo.ItemId;
        // var itemInfo = ItemRepository.Get(itemId);
        // if (itemInfo is null)
        // {
        //     return;
        // }
        //
        // if (itemInfo.Type == ItemType.Currency && quantity <= 0)
        // {
        //     Network.ReportHackAttempt(Id, "Trying to drop 0 amount of currency");
        //     return;
        // }
        //
        // quantity = Math.Min(quantity, slotInfo.Quantity);
        //
        // var spawned = Map.SpawnItem(Character.X, Character.Y, itemInfo.Id, quantity, slotInfo.Durability);
        // if (!spawned)
        // {
        //     Tell("Too many items already on the ground.", ColorCode.BrightRed);
        //     return;
        // }
        //
        // if (itemInfo.Type != ItemType.Currency)
        // {
        //     ClearInventorySlot(inventorySlot);
        //     NewMap.SendMessage(itemInfo.IsEquipment
        //             ? $"{Character.Name} drops a {itemInfo.Name} {slotInfo.Durability}/{itemInfo.Data1}."
        //             : $"{Character.Name} drops a {itemInfo}.",
        //         ColorCode.Yellow);
        //
        //     return;
        // }
        //
        // if (quantity == slotInfo.Quantity)
        // {
        //     ClearInventorySlot(inventorySlot);
        //     NewMap.SendMessage($"{Character.Name} drops {quantity} {itemInfo.Name}.", ColorCode.Yellow);
        //     return;
        // }
        //
        // slotInfo.Quantity -= quantity;
        //
        // NewMap.SendMessage($"{Character.Name} drops {quantity} {itemInfo.Name}.", ColorCode.Yellow);
        //
        // SendInventoryUpdate(inventorySlot);
    }

    public void PickupItem()
    {
        // for (var slot = 1; slot < Limits.MaxMapItems; slot++)
        // {
        //     var item = Map.GetItem(slot);
        //     if (item is null || item.X != Character.X || item.Y != Character.Y)
        //     {
        //         continue;
        //     }
        //
        //     var itemInfo = ItemRepository.Get(item.ItemId);
        //     if (itemInfo is null)
        //     {
        //         continue;
        //     }
        //
        //     var inventorySlot = GetFreeInventorySlot(itemInfo);
        //     if (inventorySlot == 0)
        //     {
        //         Tell("Your inventory is full.", ColorCode.BrightRed);
        //         return;
        //     }
        //
        //     var slotInfo = Character.Inventory[inventorySlot];
        //
        //     slotInfo.ItemId = itemInfo.Id;
        //     slotInfo.Durability = item.Dur;
        //
        //     if (itemInfo.Type == ItemType.Currency)
        //     {
        //         slotInfo.Quantity += item.Value;
        //         Tell($"You picked up {item.Value} {itemInfo.Name}.", ColorCode.Yellow);
        //     }
        //     else
        //     {
        //         slotInfo.Quantity = 0;
        //         Tell($"You picked up a {itemInfo.Name}.", ColorCode.Yellow);
        //     }
        //
        //     Map.ClearItem(slot);
        //     SendInventoryUpdate(inventorySlot);
        //
        //     return;
        // }
    }

    public int GetFreeInventorySlot(ItemInfo itemInfo)
    {
        if (itemInfo.Type == ItemType.Currency)
        {
            for (var slot = 1; slot <= Limits.MaxInventory; slot++)
            {
                if (Character.Inventory[slot].ItemId == itemInfo.Id)
                {
                    return slot;
                }
            }
        }

        for (var slot = 1; slot <= Limits.MaxInventory; slot++)
        {
            if (Character.Inventory[slot].ItemId == string.Empty)
            {
                return slot;
            }
        }

        return 0;
    }

    public bool HasSpell(int spellId)
    {
        for (var spellSlot = 1; spellSlot <= Limits.MaxPlayerSpells; spellSlot++)
        {
            if (Character.SpellIds[spellSlot] == spellId)
            {
                return true;
            }
        }

        return false;
    }

    public int GetFreeSpellSlot()
    {
        for (var slot = 1; slot <= Limits.MaxPlayerSpells; slot++)
        {
            if (Character.SpellIds[slot] == 0)
            {
                return slot;
            }
        }

        return 0;
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

    private void ReduceDurability(int inventorySlot)
    {
        var slotInfo = Character.Inventory[inventorySlot];

        var itemInfo = _itemRepository.Get(slotInfo.ItemId);
        if (itemInfo is null)
        {
            return;
        }

        slotInfo.Durability--;

        switch (slotInfo.Durability)
        {
            case <= 0:
                Tell($"Your {itemInfo.Name.Trim()} has broken!", ColorCode.Red);
                ClearInventorySlot(inventorySlot);
                break;

            case <= 5:
                Tell($"Your {itemInfo.Name.Trim()} is about to break!", ColorCode.Yellow);
                break;
        }
    }

    public int CalculateDamage()
    {
        var damage = Character.Strength / 2;
        if (damage <= 0)
        {
            damage = 1;
        }

        var weaponSlot = Character.WeaponSlot;
        if (weaponSlot <= 0)
        {
            return damage;
        }

        var itemId = Character.Inventory[weaponSlot].ItemId;
        var itemInfo = _itemRepository.Get(itemId);
        if (itemInfo is null)
        {
            return damage;
        }

        damage += itemInfo.Data2;

        ReduceDurability(weaponSlot);

        return damage;
    }

    public int CalculateProtection()
    {
        var protection = Character.Defense / 5;

        var armorSlot = Character.ArmorSlot;
        var armorItemId = Character.Inventory[armorSlot].ItemId;
        var armorItemInfo = _itemRepository.Get(armorItemId);

        if (armorItemInfo is not null)
        {
            protection += armorItemInfo.Data2;

            ReduceDurability(armorSlot);
        }

        var helmSlot = Character.HelmetSlot;
        if (helmSlot <= 0)
        {
            return protection;
        }

        var helmItemId = Character.Inventory[helmSlot].ItemId;
        var helmItemInfo = _itemRepository.Get(helmItemId);
        if (helmItemInfo is null)
        {
            return protection;
        }

        protection += helmItemInfo.Data2;

        ReduceDurability(helmSlot);

        return protection;
    }

    public bool TryCriticalHit()
    {
        if (Character.WeaponSlot <= 0)
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

    public bool TryBlockHit([NotNullWhen(true)] out ItemInfo? shieldInfo)
    {
        shieldInfo = null;

        if (Character.ShieldSlot <= 0)
        {
            return false;
        }

        shieldInfo = _itemRepository.Get(Character.Inventory[Character.ShieldSlot].ItemId);
        if (shieldInfo is null)
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

    public void Kill(int experienceLost)
    {
        DropItem(Character.WeaponSlot);
        DropItem(Character.ArmorSlot);
        DropItem(Character.HelmetSlot);
        DropItem(Character.ShieldSlot);

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

        Character.HP = Character.MaxHP;
        Character.MP = Character.MaxMP;
        Character.SP = Character.MaxSP;

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

    public void Disconnect(string message)
    {
        _connection.Disconnect(message);
    }

    public void SendVitals()
    {
        Send(new UpdateActorVitalsCommand(Id,
            Character.MaxHP, Character.HP,
            Character.MaxMP, Character.MP,
            Character.MaxSP, Character.SP));
    }

    public void SendPlayerData()
    {
        Map.Send(new PlayerData(Id,
            Character.Name,
            Character.Sprite,
            0,
            Character.X,
            Character.Y,
            Character.Direction,
            Character.AccessLevel,
            Character.PlayerKiller));
    }

    public void SendInventoryUpdate(int inventorySlot)
    {
        var slotInfo = Character.Inventory[inventorySlot];

        Send(new PlayerInventoryUpdate(inventorySlot, slotInfo.ItemId, slotInfo.Quantity, slotInfo.Durability));
    }

    public void SendEquipment()
    {
        Send(new PlayerEquipment(Character.ArmorSlot, Character.WeaponSlot, Character.HelmetSlot, Character.ShieldSlot));
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
}