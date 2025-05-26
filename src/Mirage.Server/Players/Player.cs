using System.Diagnostics.CodeAnalysis;
using Mirage.Net;
using Mirage.Net.Protocol.FromServer;
using Mirage.Net.Protocol.FromServer.New;
using Mirage.Server.Maps;
using Mirage.Server.Net;
using Mirage.Server.Npcs;
using Mirage.Server.Repositories;
using Mirage.Shared.Constants;
using Mirage.Shared.Data;
using Serilog;

namespace Mirage.Server.Players;

public sealed class Player
{
    private const float RegenIntervalInSeconds = 10.0f;

    private readonly NetworkSession _session;
    private float _regenTimer;

    public int Id { get; }
    public CharacterInfo Character { get; }
    public PlayerInventory Inventory { get; }
    public int AttackTimer { get; set; }
    public bool CastedSpell { get; set; }
    public bool InParty { get; set; }
    public bool IsPartyStarter { get; set; }
    public Player? PartyMember { get; set; }
    public Map NewMap { get; private set; }
    public Player? TargetPlayer { get; set; }
    public Npc? TargetNpc { get; set; }

    public Player(int id, NetworkSession session, CharacterInfo character, Map map)
    {
        Id = id;
        Character = character;
        Inventory = new PlayerInventory(character);

        _session = session;

        SendWelcome();

        NewMap = map;
        NewMap.Add(this);

        // CheckEquippedItems();

        var color = Character.AccessLevel <= AccessLevel.Moderator ? ColorCode.JoinLeftColor : ColorCode.White;
        
        // SendSpells();
        // Send(new PlayerInventory(Character.Inventory.Skip(1).ToArray()));
        // SendEquipment();
        //
        // WarpTo(Character.MapId, Character.X, Character.Y);

        Send<EnterGameCommand>();
        
        Network.SendGlobalMessage($"{Character.Name} has joined {Options.GameName}!", color);
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

            PartyMember.Tell($"{Character.Name} has left {Options.GameName}, disbanning party.", ColorCode.Pink);
            PartyMember = null;
        }

        CharacterRepository.Save(Character);

        var color = Character.AccessLevel <= AccessLevel.Moderator ? ColorCode.JoinLeftColor : ColorCode.White;

        Network.SendToAll(new ChatCommand($"{Character.Name} has left {Options.GameName}!", color));

        Log.Information("{CharacterName} has left {GameName}", Character.Name, Options.GameName);

        NewMap.Remove(this);
    }

    public int GetItemQuantity(int itemId)
    {
        if (itemId is <= 0 or > Limits.MaxItems)
        {
            return 0;
        }

        for (var slot = 1; slot <= Limits.MaxInventory; slot++)
        {
            var slotInfo = Character.Inventory[slot];
            if (slotInfo.ItemId != itemId)
            {
                continue;
            }

            var itemInfo = ItemRepository.Get(itemId);
            if (itemInfo?.Type == ItemType.Currency)
            {
                return slotInfo.Quantity;
            }

            return 1;
        }

        return 0;
    }

    public void ClearInventorySlot(int slot)
    {
        var slotInfo = Character.Inventory[slot];

        var itemInfo = ItemRepository.Get(slotInfo.ItemId);
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

        slotInfo.ItemId = 0;
        slotInfo.Quantity = 0;
        slotInfo.Durability = 0;

        SendInventoryUpdate(slot);
    }

    public void GiveItem(int itemId, int quantity)
    {
        var itemInfo = ItemRepository.Get(itemId);
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

    public void TakeItem(int itemId, int quantity = 0)
    {
        var itemInfo = ItemRepository.Get(itemId);
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

        var itemInfo = ItemRepository.Get(Character.Inventory[slot].ItemId);
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
            Tell($"This spell can only be learned by a {JobRepository.GetName(spellInfo.RequiredClassId)}.", ColorCode.White);
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
            if (Character.Inventory[slot].ItemId == 0)
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

    public SpellInfo? GetSpell(int slot)
    {
        if (slot is <= 0 or > Limits.MaxPlayerSpells)
        {
            return null;
        }

        var spellId = Character.SpellIds[slot];

        return SpellRepository.Get(spellId);
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
        if (NewMap == map)
        {
            // TODO: Move the player to the target coordinates...
            return;
        }

        Character.Map = NewMap.FileName;

        // var shopInfo = ShopRepository.Get(oldShopId);
        // if (shopInfo is not null && !string.IsNullOrWhiteSpace(shopInfo.LeaveSay))
        // {
        //     Tell($"{shopInfo.Name} says, '{shopInfo.LeaveSay}'", ColorCode.SayColor);
        // }

        // Remove the player from the current map.
        NewMap.Remove(this);

        // Add the player to the destination map.
        NewMap = map;
        NewMap.Add(this);

        // shopInfo = ShopRepository.Get(newShopId);
        // if (shopInfo is not null && !string.IsNullOrWhiteSpace(shopInfo.JoinSay))
        // {
        //     Tell($"{shopInfo.Name} says, '{shopInfo.JoinSay}'", ColorCode.SayColor);
        // }
    }

    public void CheckEquippedItems()
    {
        Character.WeaponSlot = UnequipIfNotValid(Character.WeaponSlot, ItemType.Weapon);
        Character.ArmorSlot = UnequipIfNotValid(Character.ArmorSlot, ItemType.Armor);
        Character.HelmetSlot = UnequipIfNotValid(Character.HelmetSlot, ItemType.Helmet);
        Character.ShieldSlot = UnequipIfNotValid(Character.ShieldSlot, ItemType.Shield);

        int UnequipIfNotValid(int slot, ItemType itemType)
        {
            if (slot <= 0)
            {
                return 0;
            }

            var itemId = Character.Inventory[slot].ItemId;
            var itemInfo = ItemRepository.Get(itemId);

            if (itemInfo is null || itemInfo.Type != itemType)
            {
                return 0;
            }

            return slot;
        }
    }

    public void CheckLevelUp()
    {
        if (Character.Exp < Character.RequiredExp)
        {
            return;
        }

        var statPoints = Math.Clamp(Character.Speed / 10, 1, 3);

        Character.Level++;
        Character.StatPoints += statPoints;
        Character.Exp -= Character.RequiredExp;

        Network.SendGlobalMessage($"{Character.Name} has reached level {Character.Level}!", ColorCode.Brown);

        Tell($"You have gained a level! You now have {Character.StatPoints} stat points to distribute.", ColorCode.BrightBlue);
    }

    private void ReduceDurability(int inventorySlot)
    {
        var slotInfo = Character.Inventory[inventorySlot];

        var itemInfo = ItemRepository.Get(slotInfo.ItemId);
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
        var itemInfo = ItemRepository.Get(itemId);
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
        var armorItemInfo = ItemRepository.Get(armorItemId);

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
        var helmItemInfo = ItemRepository.Get(helmItemId);
        if (helmItemInfo is null)
        {
            return protection;
        }

        protection += helmItemInfo.Data2;

        ReduceDurability(helmSlot);

        return protection;
    }

    private static (int X, int Y) GetAdjacentPosition(int x, int y, Direction direction)
    {
        return direction switch
        {
            Direction.Up => (x, y - 1),
            Direction.Down => (x, y + 1),
            Direction.Left => (x - 1, y),
            Direction.Right => (x + 1, y),
            _ => (x, y)
        };
    }

    public bool CanAttackPlayer(Player victim)
    {
        // const int minPvpLevel = 10;
        //
        // if (victim.GettingMap || victim.Character.HP <= 0)
        // {
        //     return false;
        // }
        //
        // if (Character.MapId != victim.Character.MapId || AttackTimer + 950 >= Environment.TickCount)
        // {
        //     return false;
        // }
        //
        // var (targetX, targetY) = GetAdjacentPosition(Character.X, Character.Y, Character.Direction);
        // if (victim.Character.X != targetX || victim.Character.Y != targetY)
        // {
        //     return false;
        // }
        //
        // if (Character.AccessLevel > AccessLevel.Moderator)
        // {
        //     Tell("You cannot attack any player for thou art an admin!", ColorCode.BrightBlue);
        //     return false;
        // }
        //
        // if (victim.Character.AccessLevel > AccessLevel.Moderator)
        // {
        //     Tell($"You cannot attack {victim.Character.Name}!", ColorCode.BrightRed);
        //     return false;
        // }
        //
        // if (NewMap.Info.Moral == MapMoral.Safe && !victim.Character.PlayerKiller)
        // {
        //     Tell("This is a safe zone!", ColorCode.BrightRed);
        //     return false;
        // }
        //
        // if (Character.Level < minPvpLevel)
        // {
        //     Tell($"You are below level {minPvpLevel}, you cannot attack another player yet!", ColorCode.BrightRed);
        //     return false;
        // }
        //
        // if (victim.Character.Level >= minPvpLevel)
        // {
        //     return true;
        // }
        //
        // Tell($"{victim.Character.Name} is below level {minPvpLevel}, you cannot attack this player yet!", ColorCode.BrightRed);
        return false;
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

        shieldInfo = ItemRepository.Get(Character.Inventory[Character.ShieldSlot].ItemId);
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

    public void AttackPlayer(Player victim, int damage)
    {
        // if (damage < 0)
        // {
        //     return;
        // }
        //
        // var weaponSlot = Character.WeaponSlot;
        // var weaponItemId = weaponSlot > 0 ? Character.Inventory[weaponSlot].ItemId : 0;
        // var weapon = ItemRepository.Get(weaponItemId);
        //
        // Map.Send(Id, new Attack(Id));
        // AttackTimer = Environment.TickCount;
        //
        // if (damage < victim.Character.HP)
        // {
        //     victim.Character.HP -= damage;
        //     victim.SendVitals();
        //
        //     if (weapon is null)
        //     {
        //         Tell($"You hit {victim.Character.Name} for {damage} hit points.", ColorCode.White);
        //
        //         victim.Tell($"{Character.Name} hit you for {damage} hit points.", ColorCode.BrightRed);
        //     }
        //     else
        //     {
        //         Tell($"You hit {victim.Character.Name} with a {weapon.Name} for {damage} hit points.", ColorCode.White);
        //
        //         victim.Tell($"{Character.Name} hit you with a {weapon.Name} for {damage} hit points.", ColorCode.BrightRed);
        //     }
        //
        //     return;
        // }
        //
        // victim.Character.HP = 0;
        //
        // if (weapon is null)
        // {
        //     Tell($"You hit {victim.Character.Name} for {damage} hit points.", ColorCode.White);
        //
        //     victim.Tell($"{Character.Name} hit you for {damage} hit points.", ColorCode.BrightRed);
        // }
        // else
        // {
        //     Tell($"You hit {victim.Character.Name} with a {weapon.Name} for {damage} hit points.", ColorCode.White);
        //
        //     victim.Tell($"{Character.Name} hit you with a {weapon.Name} for {damage} hit points.", ColorCode.BrightRed);
        // }
        //
        // Network.SendGlobalMessage($"{victim.Character.Name} has been killed by {Character.Name}.", ColorCode.BrightRed);
        //
        // var exp = Math.Max(0, victim.Character.Exp / 10);
        //
        // victim.Kill(exp);
        // if (exp == 0)
        // {
        //     victim.Tell("You lost no experience points.", ColorCode.BrightRed);
        //
        //     Tell("You received no experience points from that weak insignificant player.", ColorCode.BrightBlue);
        // }
        // else
        // {
        //     victim.Character.Exp -= exp;
        //     victim.Tell($"You lost {exp} experience points.", ColorCode.BrightRed);
        //
        //     Character.Exp += exp;
        //     Tell($"You got {exp} experience points for killing {victim.Character.Name}.", ColorCode.BrightBlue);
        //
        //     CheckLevelUp();
        // }
        //
        // if (TargetType == TargetType.Player && Target == victim.Id)
        // {
        //     Target = 0;
        //     TargetType = 0;
        // }
        //
        // if (!victim.Character.PlayerKiller)
        // {
        //     if (Character.PlayerKiller)
        //     {
        //         return;
        //     }
        //
        //     Character.PlayerKiller = true;
        //     SendPlayerData();
        //
        //     Network.SendGlobalMessage($"{Character.Name} has been deemed a Player Killer!!!", ColorCode.BrightRed);
        // }
        // else
        // {
        //     victim.Character.PlayerKiller = false;
        //     victim.SendPlayerData();
        //
        //     Network.SendGlobalMessage($"{victim.Character.Name} has paid the price for being a Player Killer!!!", ColorCode.BrightRed);
        // }
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

    public void Cast(int spellSlot)
    {
        var spellInfo = GetSpell(spellSlot);
        if (spellInfo is null)
        {
            return;
        }

        if (Character.MP < spellInfo.RequiredMp)
        {
            Tell("Not enough mana points!", ColorCode.BrightRed);
            return;
        }

        if (spellInfo.RequiredLevel > Character.Level)
        {
            Tell($"You must be level {spellInfo.RequiredLevel} to cast this spell.", ColorCode.BrightRed);
            return;
        }

        if (Environment.TickCount < AttackTimer + 1000)
        {
            return;
        }

        if (spellInfo.Type == SpellType.GiveItem)
        {
            var itemInfo = ItemRepository.Get(spellInfo.Data1);
            if (itemInfo is null)
            {
                return;
            }

            var slot = GetFreeInventorySlot(itemInfo);
            if (slot > 0)
            {
                GiveItem(spellInfo.Data1, spellInfo.Data2);
                NewMap.SendMessage($"{Character.Name} casts {spellInfo.Name}.", ColorCode.BrightBlue);

                goto L_Casted;
            }

            Tell("Your inventory is full!", ColorCode.BrightRed);
            return;
        }

        var mapId = Character.MapId;
        var mapInfo = MapRepository.Get(mapId);
        if (mapInfo is null)
        {
            return;
        }

        if (TargetPlayer is not null)
        {
            if (mapInfo.Moral == MapMoral.None &&
                TargetPlayer.Character.MapId == mapId &&
                TargetPlayer.Character is {HP: > 0, Level: >= 10, AccessLevel: AccessLevel.None} &&
                Character is {Level: >= 10, AccessLevel: AccessLevel.None})
            {
                NewMap.SendMessage($"{Character.Name} casts {spellInfo.Name} on {TargetPlayer.Character.Name}.", ColorCode.BrightBlue);

                switch (spellInfo.Type)
                {
                    case SpellType.SubHp:
                        var damage = Character.Intelligence / 4 + spellInfo.Data1 - TargetPlayer.CalculateProtection();
                        if (damage > 0)
                        {
                            AttackPlayer(TargetPlayer, damage);
                        }
                        else
                        {
                            Tell($"The spell was to weak to hurt {TargetPlayer.Character.Name}!", ColorCode.BrightRed);
                        }

                        break;

                    case SpellType.SubMp:
                        TargetPlayer.Character.MP -= spellInfo.Data1;
                        TargetPlayer.SendVitals();
                        break;

                    case SpellType.SubSp:
                        TargetPlayer.Character.SP -= spellInfo.Data1;
                        TargetPlayer.SendVitals();
                        break;
                }

                goto L_Casted;
            }

            if (mapId == TargetPlayer.Character.MapId && spellInfo.Type is >= SpellType.AddHp and <= SpellType.AddSp)
            {
                NewMap.SendMessage($"{Character.Name} casts {spellInfo.Name} on {TargetPlayer.Character.Name}.", ColorCode.BrightBlue);

                switch (spellInfo.Type)
                {
                    case SpellType.SubHp:
                        TargetPlayer.Character.HP += spellInfo.Data1;
                        TargetPlayer.SendVitals();
                        break;

                    case SpellType.SubMp:
                        TargetPlayer.Character.MP += spellInfo.Data1;
                        TargetPlayer.SendVitals();
                        break;

                    case SpellType.SubSp:
                        TargetPlayer.Character.SP += spellInfo.Data1;
                        TargetPlayer.SendVitals();
                        break;
                }

                goto L_Casted;
            }

            Tell("Could not cast spell!", ColorCode.BrightRed);
            return;
        }

        if (TargetNpc is not null && TargetNpc.Alive &&
            TargetNpc.Info.Behavior != NpcBehavior.Friendly &&
            TargetNpc.Info.Behavior != NpcBehavior.Shopkeeper)
        {
            NewMap.SendMessage($"{Character.Name} casts {spellInfo.Name} on a {TargetNpc.Info.Name}.", ColorCode.BrightBlue);

            switch (spellInfo.Type)
            {
                case SpellType.AddHp:
                    TargetNpc.Health += spellInfo.Data1;
                    break;

                case SpellType.SubHp:
                    var damage = Character.Intelligence / 4 + spellInfo.Data1 - TargetNpc.Info.Defense / 2;
                    if (damage > 0)
                    {
                        // TODO: AttackNpc(npc, damage);
                    }
                    else
                    {
                        Tell($"The spell was to weak to hurt {TargetNpc.Info.Name}!", ColorCode.BrightRed);
                    }

                    break;

                case SpellType.AddMp:
                case SpellType.SubMp:
                case SpellType.AddSp:
                case SpellType.SubSp:
                    break;
            }

            goto L_Casted;
        }

        Tell("Could not cast spell!", ColorCode.BrightRed);
        return;

        L_Casted:
        Character.MP -= spellInfo.RequiredMp;
        SendVitals();

        AttackTimer = Environment.TickCount;
        CastedSpell = true;
    }

    public void Tell(string message, int color)
    {
        Send(new ChatCommand(message, color));
    }

    /// <summary>
    /// Kills the player and moves them back to a respawn point.
    /// </summary>
    /// <param name="experienceLost">The amount of experience points lost.</param>
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

        NewMap.Remove(this);

        var map = MapManager.GetByName(Options.StartMapName);
        if (map is null)
        {
            return;
        }

        WarpTo(map, Options.StartX, Options.StartY);

        Character.HP = Character.MaxHP;
        Character.MP = Character.MaxMP;
        Character.SP = Character.MaxSP;

        NewMap.Add(this);
    }

    public void Send<TPacket>(TPacket packet) where TPacket : IPacket<TPacket>
    {
        _session.Send(packet);
    }

    public void Send<TPacket>() where TPacket : IPacket<TPacket>, new()
    {
        ((IPacketRecipient) _session).Send<TPacket>();
    }

    public void Disconnect(string message)
    {
        _session.Disconnect(message);
    }

    private void SendWelcome()
    {
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
        NewMap.Send(new PlayerData(Id,
            Character.Name,
            Character.Sprite,
            Character.MapId,
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

    public void SendStats()
    {
        Send(new PlayerStats(Character.Strength, Character.Defense, Character.Speed, Character.Intelligence));
    }

    public void SendWhosOnline()
    {
        var playerNames = Network
            .OnlinePlayers()
            .Select(x => x.Character.Name)
            .ToList();

        var message = string.Join(", ", playerNames);

        message = message.Length == 0
            ? "There are no other players online."
            : $"There are {playerNames.Count} other players online: {message}";

        Tell(message, ColorCode.WhoColor);
    }
}