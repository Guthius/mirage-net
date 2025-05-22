using System.Diagnostics.CodeAnalysis;
using Mirage.Game.Constants;
using Mirage.Game.Data;
using Mirage.Net;
using Mirage.Net.Protocol.FromServer;
using Mirage.Server.Net;
using Mirage.Server.Repositories;
using Serilog;

namespace Mirage.Server.Game;

public sealed class GamePlayer
{
    private readonly GameSession _session;

    public int Id { get; }
    public CharacterInfo Character { get; }
    public GameMap Map { get; private set; }
    public int AttackTimer { get; set; }
    public TargetType TargetType { get; set; }
    public int Target { get; set; }
    public bool CastedSpell { get; set; }
    public bool InParty { get; set; }
    public bool IsPartyStarter { get; set; }
    public GamePlayer? PartyMember { get; set; }
    public bool GettingMap { get; set; }


    public Map NewMap { get; private set; }


    public GamePlayer(int id, GameSession session, CharacterInfo character, Map map)
    {
        Id = id;
        Character = character;
        Map = GameState.GetMap(Character.MapId);

        _session = session;

        NewMap = map;
        NewMap.Add(this);


        // CheckEquippedItems();
        //
        // var color = Character.AccessLevel <= AccessLevel.Moderator ? Color.JoinLeftColor : Color.White;
        //
        // Network.SendGlobalMessage($"{Character.Name} has joined {Options.GameName}!", color);
        //
        // SendItems();
        // SendNpcs();
        // SendShops();
        // SendSpells();
        // Send(new PlayerInventory(Character.Inventory.Skip(1).ToArray()));
        // SendEquipment();
        // Send(new PlayerHp(Character.MaxHP, Character.HP));
        // Send(new PlayerMp(Character.MaxMP, Character.MP));
        // Send(new PlayerSp(Character.MaxSP, Character.SP));
        //
        // Send(new PlayerStats(
        //     Character.Strength,
        //     Character.Defense,
        //     Character.Speed,
        //     Character.Intelligence));
        //
        // WarpTo(Character.MapId, Character.X, Character.Y);
        // Tell($"Welcome to {Options.GameName}!  Programmed from scratch by yours truely Consty!  Version {Options.VersionMajor}.{Options.VersionMinor}.{Options.VersionBuild}", Color.BrightBlue);
        // Tell("Type /help for help on commands.  Use arrow keys to move, hold down shift to run, and use ctrl to attack.", Color.Cyan);
        //
        // if (File.Exists("Motd.txt"))
        // {
        //     var motd = File.ReadAllText("Motd.txt");
        //     if (!string.IsNullOrWhiteSpace(motd))
        //     {
        //         Tell("MOTD: " + motd.Trim(), Color.BrightCyan);
        //     }
        // }
        //
        // SendWhosOnline();

        Send<InGame>();
    }

    public void Destroy()
    {
        var mapId = Character.MapId;

        Map.PlayersOnMap = GameState.OnlinePlayerCount(mapId) > 1;

        if (Map.Info.BootMapId > 0)
        {
            Character.X = Map.Info.BootX;
            Character.Y = Map.Info.BootY;
            Character.MapId = Map.Info.BootMapId;
        }

        if (InParty && PartyMember is not null)
        {
            InParty = false;

            PartyMember.Tell($"{Character.Name} has left {Options.GameName}, disbanning party.", Color.Pink);
            PartyMember = null;
        }

        CharacterRepository.Save(Character);

        var color = Character.AccessLevel <= AccessLevel.Moderator ? Color.JoinLeftColor : Color.White;

        Network.SendToAll(new PlayerMessage($"{Character.Name} has left {Options.GameName}!", color));

        Log.Information("{CharacterName} has left {GameName}", Character.Name, Options.GameName);

        NewMap.Remove(this);
    }

    private bool TryPlayerMove(int x, int y, MovementType movementType)
    {
        if (x is >= 0 and <= Limits.MaxMapWidth && y is >= 0 and <= Limits.MaxMapHeight)
        {
            switch (Map.Info.Tiles[x, y].Type)
            {
                case TileType.Blocked:
                case TileType.Key when !Map.DoorOpen[x, y]:
                    return false;
            }

            Character.X = x;
            Character.Y = y;

            Map.Send(Id, new PlayerMove(Id, x, y, Character.Direction, movementType));

            return true;
        }

        var (targetMapId, targetX, targetY) = Map.Info.GetAdjacentMap(Character.Direction, Character.X, Character.Y);
        if (targetMapId == 0)
        {
            return false;
        }

        WarpTo(targetMapId, targetX, targetY);
        return true;
    }

    public void Move(Direction direction, MovementType movementType)
    {
        // Character.Direction = direction;
        //
        // var x = Character.X;
        // var y = Character.Y;
        //
        // var moved = direction switch
        // {
        //     Direction.Up => TryPlayerMove(x, y - 1, movementType),
        //     Direction.Down => TryPlayerMove(x, y + 1, movementType),
        //     Direction.Left => TryPlayerMove(x - 1, y, movementType),
        //     Direction.Right => TryPlayerMove(x + 1, y, movementType),
        //     _ => false
        // };
        //
        // x = Character.X;
        // y = Character.Y;
        //
        // if (Map.Info.Tiles[x, y].Type == TileType.Warp)
        // {
        //     var targetMapId = Map.Info.Tiles[x, y].Data1;
        //     var targetX = Map.Info.Tiles[x, y].Data2;
        //     var targetY = Map.Info.Tiles[x, y].Data3;
        //
        //     WarpTo(targetMapId, targetX, targetY);
        //
        //     moved = true;
        // }
        //
        // if (Map.Info.Tiles[x, y].Type == TileType.KeyOpen)
        // {
        //     x = Map.Info.Tiles[x, y].Data1;
        //     y = Map.Info.Tiles[x, y].Data2;
        //
        //     if (Map.Info.Tiles[x, y].Type == TileType.Key && !Map.DoorOpen[x, y])
        //     {
        //         Map.DoorOpen[x, y] = true;
        //         Map.DoorTimer = Environment.TickCount;
        //         Map.Send(new MapKey(x, y, true));
        //         Map.SendMessage("A door has been unlocked.", Color.White);
        //     }
        // }
        //
        // if (!moved)
        // {
        //     Network.ReportHackAttempt(Id, "Position Modification");
        // }
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
            Tell("Your inventory is full.", Color.BrightRed);
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

            case ItemType.Key:
                UseKey(slot, itemInfo);
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
                        Tell($"Your defense is to low to wear this armor!  Required DEF ({itemInfo.Data2})", Color.BrightRed);
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
                        Tell($"Your strength is to low to wear this armor!  Required STR ({itemInfo.Data2})", Color.BrightRed);
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
                        Tell($"Your speed coordination is to low to wear this helmet!  Required SPEED ({itemInfo.Data2})", Color.BrightRed);
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
                SendHP();
                break;

            case ItemType.PotionAddMp:
                Character.MP += value;
                ClearInventorySlot(inventorySlot);
                SendMP();
                break;

            case ItemType.PotionAddSp:
                Character.SP += value;
                ClearInventorySlot(inventorySlot);
                SendSP();
                break;

            case ItemType.PotionSubHp:
                Character.HP -= value;
                ClearInventorySlot(inventorySlot);
                SendHP();
                break;

            case ItemType.PotionSubMp:
                Character.MP -= value;
                ClearInventorySlot(inventorySlot);
                SendMP();
                break;

            case ItemType.PotionSubSp:
                Character.SP -= value;
                ClearInventorySlot(inventorySlot);
                SendSP();
                break;
        }
    }

    private void UseKey(int inventorySlot, ItemInfo itemInfo)
    {
        var (x, y) = GetAdjacentPosition(Character.X, Character.Y, Character.Direction);
        if (!Map.InBounds(x, y))
        {
            return;
        }

        var mapInfo = MapRepository.Get(Character.MapId);
        if (mapInfo is null)
        {
            return;
        }

        if (mapInfo.Tiles[x, y].Type != TileType.Key ||
            mapInfo.Tiles[x, y].Data1 != itemInfo.Id)
        {
            return;
        }

        Map.DoorOpen[x, y] = true;
        Map.DoorTimer = Environment.TickCount;

        Map.Send(new MapKey(x, y, true));
        Map.SendMessage("A door has been unlocked.", Color.White);

        if (Map.Info.Tiles[x, y].Data2 != 1)
        {
            return;
        }

        ClearInventorySlot(inventorySlot);

        Tell("The key disolves.", Color.Yellow);
    }

    private void UseSpell(int inventorySlot, ItemInfo itemInfo)
    {
        var spellInfo = SpellRepository.Get(itemInfo.Data1);
        if (spellInfo is null)
        {
            Tell("This scroll is not connected to a spell, please inform an admin!", Color.White);
            return;
        }

        if (!string.IsNullOrEmpty(spellInfo.RequiredClassId) && spellInfo.RequiredClassId != Character.JobId)
        {
            Tell($"This spell can only be learned by a {ClassRepository.GetName(spellInfo.RequiredClassId)}.", Color.White);
            return;
        }

        if (spellInfo.RequiredLevel > Character.Level)
        {
            Tell($"You must be level {spellInfo.RequiredLevel} to learn this spell.", Color.White);
            return;
        }

        var spellSlot = GetFreeSpellSlot();
        if (spellSlot <= 0)
        {
            Tell("You have learned all that you can learn!", Color.BrightRed);
            return;
        }

        ClearInventorySlot(inventorySlot);

        if (HasSpell(spellInfo.Id))
        {
            Tell("You have already learned this spell! The spells crumbles into dust.", Color.BrightRed);
            return;
        }

        Character.SpellIds[spellSlot] = spellInfo.Id;

        Tell("You study the spell carefully...", Color.Yellow);
        Tell("You have learned a new spell!", Color.White);
    }

    public void DropItem(int inventorySlot, int quantity = 0)
    {
        if (inventorySlot is <= 0 or > Limits.MaxInventory)
        {
            return;
        }

        var slotInfo = Character.Inventory[inventorySlot];

        var itemId = slotInfo.ItemId;
        var itemInfo = ItemRepository.Get(itemId);
        if (itemInfo is null)
        {
            return;
        }

        if (itemInfo.Type == ItemType.Currency && quantity <= 0)
        {
            Network.ReportHackAttempt(Id, "Trying to drop 0 amount of currency");
            return;
        }

        quantity = Math.Min(quantity, slotInfo.Quantity);

        var spawned = Map.SpawnItem(Character.X, Character.Y, itemInfo.Id, quantity, slotInfo.Durability);
        if (!spawned)
        {
            Tell("Too many items already on the ground.", Color.BrightRed);
            return;
        }

        if (itemInfo.Type != ItemType.Currency)
        {
            ClearInventorySlot(inventorySlot);
            Map.SendMessage(itemInfo.IsEquipment
                    ? $"{Character.Name} drops a {itemInfo.Name} {slotInfo.Durability}/{itemInfo.Data1}."
                    : $"{Character.Name} drops a {itemInfo}.",
                Color.Yellow);

            return;
        }

        if (quantity == slotInfo.Quantity)
        {
            ClearInventorySlot(inventorySlot);
            Map.SendMessage($"{Character.Name} drops {quantity} {itemInfo.Name}.", Color.Yellow);
            return;
        }

        slotInfo.Quantity -= quantity;

        Map.SendMessage($"{Character.Name} drops {quantity} {itemInfo.Name}.", Color.Yellow);

        SendInventoryUpdate(inventorySlot);
    }

    public void PickupItem()
    {
        for (var slot = 1; slot < Limits.MaxMapItems; slot++)
        {
            var item = Map.GetItem(slot);
            if (item is null || item.X != Character.X || item.Y != Character.Y)
            {
                continue;
            }

            var itemInfo = ItemRepository.Get(item.ItemId);
            if (itemInfo is null)
            {
                continue;
            }

            var inventorySlot = GetFreeInventorySlot(itemInfo);
            if (inventorySlot == 0)
            {
                Tell("Your inventory is full.", Color.BrightRed);
                return;
            }

            var slotInfo = Character.Inventory[inventorySlot];

            slotInfo.ItemId = itemInfo.Id;
            slotInfo.Durability = item.Dur;

            if (itemInfo.Type == ItemType.Currency)
            {
                slotInfo.Quantity += item.Value;
                Tell($"You picked up {item.Value} {itemInfo.Name}.", Color.Yellow);
            }
            else
            {
                slotInfo.Quantity = 0;
                Tell($"You picked up a {itemInfo.Name}.", Color.Yellow);
            }

            Map.ClearItem(slot);
            SendInventoryUpdate(inventorySlot);

            return;
        }
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

    public void WarpTo(int targetMapId, int x, int y)
    {
        var oldMap = Map;
        var oldShopId = Map.Info.ShopId;

        var targetMap = MapRepository.Get(targetMapId);
        if (targetMap is null)
        {
            return;
        }

        var shopInfo = ShopRepository.Get(oldShopId);
        if (shopInfo is not null && !string.IsNullOrWhiteSpace(shopInfo.LeaveSay))
        {
            Tell($"{shopInfo.Name} says, '{shopInfo.LeaveSay}'", Color.SayColor);
        }

        Map.Send(Id, PlayerData.ClearMap(Id, Character));
        Map = GameState.GetMap(targetMap.Id);
        Map.PlayersOnMap = true;

        var newMapId = Map.Info.Id;
        var newShopId = Map.Info.ShopId;

        Character.MapId = newMapId;
        Character.X = x;
        Character.Y = y;

        shopInfo = ShopRepository.Get(newShopId);
        if (shopInfo is not null && !string.IsNullOrWhiteSpace(shopInfo.JoinSay))
        {
            Tell($"{shopInfo.Name} says, '{shopInfo.JoinSay}'", Color.SayColor);
        }

        if (oldMap != Map)
        {
            oldMap.PlayersOnMap = GameState.OnlinePlayerCount(oldMap.Info.Id) > 0;
        }

        GettingMap = true;

        Send(new CheckForMap(targetMap.Id, targetMap.Revision));
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

        Network.SendGlobalMessage($"{Character.Name} has reached level {Character.Level}!", Color.Brown);

        Tell($"You have gained a level!  You now have {Character.StatPoints} stat points to distribute.", Color.BrightBlue);
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
                Tell($"Your {itemInfo.Name.Trim()} has broken!", Color.Red);
                ClearInventorySlot(inventorySlot);
                break;

            case <= 5:
                Tell($"Your {itemInfo.Name.Trim()} is about to break!", Color.Yellow);
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

    public bool CanAttackPlayer(GamePlayer victim)
    {
        const int minPvpLevel = 10;

        if (victim.GettingMap || victim.Character.HP <= 0)
        {
            return false;
        }

        if (Character.MapId != victim.Character.MapId || AttackTimer + 950 >= Environment.TickCount)
        {
            return false;
        }

        var (targetX, targetY) = GetAdjacentPosition(Character.X, Character.Y, Character.Direction);
        if (victim.Character.X != targetX || victim.Character.Y != targetY)
        {
            return false;
        }

        if (Character.AccessLevel > AccessLevel.Moderator)
        {
            Tell("You cannot attack any player for thou art an admin!", Color.BrightBlue);
            return false;
        }

        if (victim.Character.AccessLevel > AccessLevel.Moderator)
        {
            Tell($"You cannot attack {victim.Character.Name}!", Color.BrightRed);
            return false;
        }

        if (Map.Info.Moral == MapMoral.Safe && !victim.Character.PlayerKiller)
        {
            Tell("This is a safe zone!", Color.BrightRed);
            return false;
        }

        if (Character.Level < minPvpLevel)
        {
            Tell($"You are below level {minPvpLevel}, you cannot attack another player yet!", Color.BrightRed);
            return false;
        }

        if (victim.Character.Level >= minPvpLevel)
        {
            return true;
        }

        Tell($"{victim.Character.Name} is below level {minPvpLevel}, you cannot attack this player yet!", Color.BrightRed);
        return false;
    }

    public bool CanAttackNpc(GameNpc npc)
    {
        if (Environment.TickCount <= AttackTimer + 950)
        {
            return false;
        }

        var (targetX, targetY) = GetAdjacentPosition(Character.X, Character.Y, Character.Direction);
        if (targetX != npc.X || targetY != npc.Y)
        {
            return false;
        }

        if (npc.Info.Behavior != NpcBehavior.Friendly &&
            npc.Info.Behavior != NpcBehavior.Shopkeeper)
        {
            return true;
        }

        Tell($"You cannot attack a {npc.Info.Name}!", Color.BrightBlue);
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

    public void AttackPlayer(GamePlayer victim, int damage)
    {
        if (damage < 0)
        {
            return;
        }

        var weaponSlot = Character.WeaponSlot;
        var weaponItemId = weaponSlot > 0 ? Character.Inventory[weaponSlot].ItemId : 0;
        var weapon = ItemRepository.Get(weaponItemId);

        Map.Send(Id, new Attack(Id));
        AttackTimer = Environment.TickCount;

        if (damage < victim.Character.HP)
        {
            victim.Character.HP -= damage;
            victim.SendHP();

            if (weapon is null)
            {
                Tell($"You hit {victim.Character.Name} for {damage} hit points.", Color.White);

                victim.Tell($"{Character.Name} hit you for {damage} hit points.", Color.BrightRed);
            }
            else
            {
                Tell($"You hit {victim.Character.Name} with a {weapon.Name} for {damage} hit points.", Color.White);

                victim.Tell($"{Character.Name} hit you with a {weapon.Name} for {damage} hit points.", Color.BrightRed);
            }

            return;
        }

        victim.Character.HP = 0;

        if (weapon is null)
        {
            Tell($"You hit {victim.Character.Name} for {damage} hit points.", Color.White);

            victim.Tell($"{Character.Name} hit you for {damage} hit points.", Color.BrightRed);
        }
        else
        {
            Tell($"You hit {victim.Character.Name} with a {weapon.Name} for {damage} hit points.", Color.White);

            victim.Tell($"{Character.Name} hit you with a {weapon.Name} for {damage} hit points.", Color.BrightRed);
        }

        Network.SendGlobalMessage($"{victim.Character.Name} has been killed by {Character.Name}.", Color.BrightRed);

        victim.DropItem(victim.Character.WeaponSlot);
        victim.DropItem(victim.Character.ArmorSlot);
        victim.DropItem(victim.Character.HelmetSlot);
        victim.DropItem(victim.Character.ShieldSlot);

        var exp = Math.Max(0, victim.Character.Exp / 10);
        if (exp == 0)
        {
            victim.Tell("You lost no experience points.", Color.BrightRed);

            Tell("You received no experience points from that weak insignificant player.", Color.BrightBlue);
        }
        else
        {
            victim.Character.Exp -= exp;
            victim.Tell($"You lost {exp} experience points.", Color.BrightRed);

            Character.Exp += exp;
            Tell($"You got {exp} experience points for killing {victim.Character.Name}.", Color.BrightBlue);

            CheckLevelUp();
        }

        victim.WarpTo(Options.StartMapId, Options.StartX, Options.StartY);
        victim.Character.HP = victim.Character.MaxHP;
        victim.Character.MP = victim.Character.MaxMP;
        victim.Character.SP = victim.Character.MaxSP;
        victim.SendHP();
        victim.SendMP();
        victim.SendSP();

        if (TargetType == TargetType.Player && Target == victim.Id)
        {
            Target = 0;
            TargetType = 0;
        }

        if (!victim.Character.PlayerKiller)
        {
            if (Character.PlayerKiller)
            {
                return;
            }

            Character.PlayerKiller = true;
            SendPlayerData();

            Network.SendGlobalMessage($"{Character.Name} has been deemed a Player Killer!!!", Color.BrightRed);
        }
        else
        {
            victim.Character.PlayerKiller = false;
            victim.SendPlayerData();

            Network.SendGlobalMessage($"{victim.Character.Name} has paid the price for being a Player Killer!!!", Color.BrightRed);
        }
    }

    public void AttackNpc(GameNpc npc, int damage)
    {
        if (damage < 0)
        {
            return;
        }

        var mapId = Character.MapId;
        var mapInfo = MapRepository.Get(mapId);
        if (mapInfo is null)
        {
            return;
        }

        ItemInfo? weaponInfo = null;
        if (Character.WeaponSlot > 0)
        {
            weaponInfo = ItemRepository.Get(Character.Inventory[Character.WeaponSlot].ItemId);
        }

        Map.Send(Id, new Attack(Id));

        AttackTimer = Environment.TickCount;

        if (damage < npc.HP)
        {
            npc.HP -= damage;

            Tell(weaponInfo is null
                    ? $"You hit a {npc.Info.Name} for {damage} hit points."
                    : $"You hit a {npc.Info.Name} with a {weaponInfo.Name} for {damage} hit points.",
                Color.White);

            if (npc.Target == 0 && npc.Target != Id && !string.IsNullOrWhiteSpace(npc.Info.AttackSay))
            {
                Tell($"A {npc.Info.Name} says, '{npc.Info.AttackSay}' to you.", Color.SayColor);
            }

            npc.Target = Id;
            if (npc.Info.Behavior != NpcBehavior.Guard)
            {
                return;
            }

            foreach (var otherNpc in Map.AliveNpcs().Where(x => x.Info.Id == npc.Info.Id))
            {
                otherNpc.Target = Id;
            }

            return;
        }

        Tell(weaponInfo is null
                ? $"You hit a {npc.Info.Name} for {damage} hit points, killing it."
                : $"You hit a {npc.Info.Name} with a {weaponInfo.Name} for {damage} hit points, killing it.",
            Color.BrightRed);

        var exp = Math.Min(1, npc.Info.Strength * npc.Info.Defense * 2);

        if (!InParty)
        {
            Character.Exp += exp;
            Tell($"You have gained {exp} experience points.", Color.BrightBlue);
        }
        else
        {
            exp = Math.Min(1, exp / 2);

            Character.Exp += exp;
            Tell($"You have gained {exp} party experience points.", Color.BrightBlue);

            var partyPlayer = PartyMember;
            if (partyPlayer is not null)
            {
                partyPlayer.Character.Exp += exp;
                partyPlayer.Tell($"You have gained {exp} party experience points.", Color.BrightBlue);
            }
        }

        var dropChance = Random.Shared.Next(npc.Info.DropChance) + 1;
        if (dropChance == 1)
        {
            Map.SpawnItem(npc.X, npc.Y,
                npc.Info.DropItemId,
                npc.Info.DropItemQuantity);
        }

        npc.Kill();

        CheckLevelUp();

        if (InParty && PartyMember is not null)
        {
            PartyMember.CheckLevelUp();
        }

        if (TargetType != TargetType.Npc || Target != npc.Slot)
        {
            return;
        }

        Target = 0;
        TargetType = TargetType.Player;
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
            Tell("Not enough mana points!", Color.BrightRed);
            return;
        }

        if (spellInfo.RequiredLevel > Character.Level)
        {
            Tell($"You must be level {spellInfo.RequiredLevel} to cast this spell.", Color.BrightRed);
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
                Map.SendMessage($"{Character.Name} casts {spellInfo.Name}.", Color.BrightBlue);

                goto L_Casted;
            }

            Tell("Your inventory is full!", Color.BrightRed);
            return;
        }

        var mapId = Character.MapId;
        var mapInfo = MapRepository.Get(mapId);
        if (mapInfo is null)
        {
            return;
        }

        if (TargetType == TargetType.Player)
        {
            var targetPlayer = GameState.GetPlayer(Target);
            if (targetPlayer is null)
            {
                Tell("Could not cast spell!", Color.BrightRed);
                return;
            }

            if (mapInfo.Moral == MapMoral.None &&
                targetPlayer.Character.MapId == mapId &&
                targetPlayer.Character is {HP: > 0, Level: >= 10, AccessLevel: AccessLevel.Player} &&
                Character is {Level: >= 10, AccessLevel: AccessLevel.Player})
            {
                Map.SendMessage($"{Character.Name} casts {spellInfo.Name} on {targetPlayer.Character.Name}.", Color.BrightBlue);

                switch (spellInfo.Type)
                {
                    case SpellType.SubHp:
                        var damage = Character.Intelligence / 4 + spellInfo.Data1 - targetPlayer.CalculateProtection();
                        if (damage > 0)
                        {
                            AttackPlayer(targetPlayer, damage);
                        }
                        else
                        {
                            Tell($"The spell was to weak to hurt {targetPlayer.Character.Name}!", Color.BrightRed);
                        }

                        break;

                    case SpellType.SubMp:
                        targetPlayer.Character.MP -= spellInfo.Data1;
                        targetPlayer.SendMP();
                        break;

                    case SpellType.SubSp:
                        targetPlayer.Character.SP -= spellInfo.Data1;
                        targetPlayer.SendSP();
                        break;
                }

                goto L_Casted;
            }

            if (mapId == targetPlayer.Character.MapId && spellInfo.Type is >= SpellType.AddHp and <= SpellType.AddSp)
            {
                Map.SendMessage($"{Character.Name} casts {spellInfo.Name} on {targetPlayer.Character.Name}.", Color.BrightBlue);

                switch (spellInfo.Type)
                {
                    case SpellType.SubHp:
                        targetPlayer.Character.HP += spellInfo.Data1;
                        targetPlayer.SendHP();
                        break;

                    case SpellType.SubMp:
                        targetPlayer.Character.MP += spellInfo.Data1;
                        targetPlayer.SendMP();
                        break;

                    case SpellType.SubSp:

                        targetPlayer.Character.SP += spellInfo.Data1;
                        targetPlayer.SendMP();
                        break;
                }

                goto L_Casted;
            }

            Tell("Could not cast spell!", Color.BrightRed);
            return;
        }

        var npc = Map.GetNpc(Target);
        if (npc is not null && npc.Alive && npc.Info.Behavior != NpcBehavior.Friendly && npc.Info.Behavior != NpcBehavior.Shopkeeper)
        {
            Map.SendMessage($"{Character.Name} casts {spellInfo.Name} on a {npc.Info.Name}.", Color.BrightBlue);

            switch (spellInfo.Type)
            {
                case SpellType.AddHp:
                    npc.HP += spellInfo.Data1;
                    break;

                case SpellType.SubHp:
                    var damage = Character.Intelligence / 4 + spellInfo.Data1 - npc.Info.Defense / 2;
                    if (damage > 0)
                    {
                        AttackNpc(npc, damage);
                    }
                    else
                    {
                        Tell($"The spell was to weak to hurt {npc.Info.Name}!", Color.BrightRed);
                    }

                    break;

                case SpellType.AddMp:
                    npc.MP += spellInfo.Data1;
                    break;

                case SpellType.SubMp:
                    npc.MP -= spellInfo.Data1;
                    break;

                case SpellType.AddSp:
                    npc.SP += spellInfo.Data1;
                    break;

                case SpellType.SubSp:
                    npc.SP -= spellInfo.Data1;
                    break;
            }

            goto L_Casted;
        }

        Tell("Could not cast spell!", Color.BrightRed);
        return;

        L_Casted:
        Character.MP -= spellInfo.RequiredMp;
        SendMP();

        AttackTimer = Environment.TickCount;
        CastedSpell = true;
    }

    public void Send<TPacket>(TPacket packet) where TPacket : IPacket<TPacket>
    {
        _session.Send(packet);
    }

    public void Send<TPacket>() where TPacket : IPacket<TPacket>, new()
    {
        ((IPacketRecipient) _session).Send<TPacket>();
    }

    public void SendAlert(string alertMessage)
    {
        _session.SendAlert(alertMessage);
    }

    public void SendPlayerData()
    {
        Map.Send(new PlayerData(Id,
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

    public void SendHP()
    {
        Send(new PlayerHp(Character.MaxHP, Character.HP));
    }

    public void SendMP()
    {
        Send(new PlayerMp(Character.MaxMP, Character.HP));
    }

    public void SendSP()
    {
        Send(new PlayerSp(Character.MaxSP, Character.SP));
    }

    public void SendItems()
    {
        for (var itemId = 1; itemId <= Limits.MaxItems; itemId++)
        {
            var itemInfo = ItemRepository.Get(itemId);
            if (itemInfo is null || string.IsNullOrEmpty(itemInfo.Name))
            {
                return;
            }

            Send(new UpdateItem(itemInfo));
        }
    }

    public void SendSpells()
    {
        for (var spellId = 1; spellId <= Limits.MaxSpells; spellId++)
        {
            var spellInfo = SpellRepository.Get(spellId);
            if (spellInfo is null || string.IsNullOrEmpty(spellInfo.Name))
            {
                continue;
            }

            Send(new UpdateSpell(spellId, spellInfo.Name));
        }
    }

    public void SendNpcs()
    {
        for (var npcId = 1; npcId <= Limits.MaxNpcs; npcId++)
        {
            var npcInfo = NpcRepository.Get(npcId);
            if (npcInfo is null || string.IsNullOrEmpty(npcInfo.Name))
            {
                continue;
            }

            Send(new UpdateNpc(npcId, npcInfo.Name, npcInfo.Sprite));
        }
    }

    public void SendShops()
    {
        for (var shopId = 1; shopId <= Limits.MaxShops; shopId++)
        {
            var shopInfo = ShopRepository.Get(shopId);
            if (shopInfo is null || string.IsNullOrEmpty(shopInfo.Name))
            {
                continue;
            }

            Send(new UpdateShop(shopId, shopInfo.Name));
        }
    }

    public void SendWhosOnline()
    {
        var playerNames = GameState
            .OnlinePlayers()
            .Select(x => x.Character.Name)
            .ToList();

        var message = string.Join(", ", playerNames);

        message = message.Length == 0
            ? "There are no other players online."
            : $"There are {playerNames.Count} other players online: {message}";

        Tell(message, Color.WhoColor);
    }

    public void SendJoinMap()
    {
        foreach (var player in GameState.OnlinePlayers())
        {
            if (player != this && player.Character.MapId == Character.MapId)
            {
                Send(new PlayerData(
                    player.Id,
                    player.Character.Name,
                    player.Character.Sprite,
                    player.Character.MapId,
                    player.Character.X,
                    player.Character.Y,
                    player.Character.Direction,
                    player.Character.AccessLevel,
                    player.Character.PlayerKiller));
            }
        }

        Map.Send(new PlayerData(Id,
            Character.Name,
            Character.Sprite,
            Character.MapId,
            Character.X,
            Character.Y,
            Character.Direction,
            Character.AccessLevel,
            Character.PlayerKiller));
    }

    public void Tell(string message, int color)
    {
        Send(new PlayerMessage(message, color));
    }
}