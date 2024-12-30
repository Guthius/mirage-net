namespace Mirage.Modules;

public static class modGameLogic
{
    private static void ReduceDurability(int index, int invSlot)
    {
        var itemNum = modTypes.GetPlayerInvItemNum(index, invSlot);
        var itemDur = modTypes.GetPlayerInvItemDur(index, invSlot) - 1;

        modTypes.SetPlayerInvItemDur(index, invSlot, itemDur);
        if (itemDur <= 0)
        {
            modServerTCP.PlayerMsg(index, $"Your {modTypes.Item[itemNum].Name.Trim()} is about to break!", modText.Yellow);
            TakeItem(index, itemNum, 0);
        }
        else
        {
            if (itemDur <= 5)
            {
                modServerTCP.PlayerMsg(index, $"Your {modTypes.Item[itemNum].Name.Trim()} is about to break!", modText.Yellow);
            }
        }
    }

    public static int GetPlayerDamage(int index)
    {
        if (!modServerTCP.IsPlaying(index) || index <= 0 || index > modTypes.MAX_PLAYERS)
        {
            return 0;
        }

        var damage = modTypes.GetPlayerSTR(index) / 2;

        if (damage <= 0)
        {
            damage = 1;
        }

        var weaponSlot = modTypes.GetPlayerWeaponSlot(index);
        if (weaponSlot <= 0)
        {
            return damage;
        }

        var itemNum = modTypes.GetPlayerInvItemNum(index, weaponSlot);

        damage += modTypes.Item[itemNum].Data2;

        ReduceDurability(index, weaponSlot);

        return damage;
    }

    public static int GetPlayerProtection(int index)
    {
        if (!modServerTCP.IsPlaying(index) || index <= 0 || index > modTypes.MAX_PLAYERS)
        {
            return 0;
        }

        var protection = modTypes.GetPlayerDEF(index) / 5;

        var armorSlot = modTypes.GetPlayerArmorSlot(index);
        if (armorSlot > 0)
        {
            protection += modTypes.Item[modTypes.GetPlayerInvItemNum(index, armorSlot)].Data2;

            ReduceDurability(index, armorSlot);
        }

        var helmSlot = modTypes.GetPlayerHelmetSlot(index);
        if (helmSlot > 0)
        {
            protection += modTypes.Item[modTypes.GetPlayerInvItemNum(index, helmSlot)].Data2;

            ReduceDurability(index, helmSlot);
        }

        return protection;
    }

    public static int FindOpenInvSlot(int index, int itemNum)
    {
        // Check for subscript out of range
        if (!modServerTCP.IsPlaying(index) || itemNum <= 0 || itemNum > modTypes.MAX_ITEMS)
        {
            return 0;
        }

        if (modTypes.Item[itemNum].Type == modTypes.ITEM_TYPE_CURRENCY)
        {
            // If currency then check to see if they already have an instance of the item and add it to that
            for (var i = 1; i <= modTypes.MAX_INV; i++)
            {
                if (modTypes.GetPlayerInvItemNum(index, i) == itemNum)
                {
                    return i;
                }
            }
        }

        for (var i = 1; i <= modTypes.MAX_INV; i++)
        {
            // Try to find an open free slot
            if (modTypes.GetPlayerInvItemNum(index, i) == 0)
            {
                return i;
            }
        }

        return 0;
    }

    public static int FindOpenMapItemSlot(int mapNum)
    {
        if (mapNum <= 0 || mapNum > modTypes.MAX_MAPS)
        {
            return 0;
        }

        for (var i = 1; i <= modTypes.MAX_MAP_ITEMS; i++)
        {
            if (modTypes.MapItem[mapNum, i].Num == 0)
            {
                return i;
            }
        }

        return 0;
    }

    public static int FindOpenSpellSlot(int index)
    {
        for (var i = 1; i <= modTypes.MAX_PLAYER_SPELLS; i++)
        {
            if (modTypes.GetPlayerSpell(index, i) == 0)
            {
                return i;
            }
        }

        return 0;
    }

    public static bool HasSpell(int index, int spellNum)
    {
        for (var i = 1; i <= modTypes.MAX_PLAYER_SPELLS; i++)
        {
            if (modTypes.GetPlayerSpell(index, i) == spellNum)
            {
                return true;
            }
        }

        return false;
    }

    public static int TotalOnlinePlayers()
    {
        var total = 0;

        for (var i = 1; i <= modTypes.MAX_PLAYERS; i++)
        {
            if (modServerTCP.IsPlaying(i))
            {
                total++;
            }
        }

        return total;
    }

    public static int FindPlayer(string name)
    {
        name = name.Trim();

        for (var i = 1; i <= modTypes.MAX_PLAYERS; i++)
        {
            // Make sure we dont try to check a name thats to small
            if (modTypes.GetPlayerName(i).Length >= name.Length)
            {
                if (modTypes.GetPlayerName(i)[..name.Length].Equals(name, StringComparison.CurrentCultureIgnoreCase))
                {
                    return i;
                }
            }
        }

        return 0;
    }

    public static int HasItem(int index, int itemNum)
    {
        // Check for subscript out of range
        if (!modServerTCP.IsPlaying(index) || itemNum <= 0 || itemNum > modTypes.MAX_ITEMS)
        {
            return 0;
        }

        for (var i = 1; i <= modTypes.MAX_INV; i++)
        {
            if (modTypes.GetPlayerInvItemNum(index, i) != itemNum)
            {
                continue;
            }

            if (modTypes.Item[itemNum].Type == modTypes.ITEM_TYPE_CURRENCY)
            {
                return modTypes.GetPlayerInvItemValue(index, i);
            }

            return 1;
        }

        return 0;
    }

    public static void TakeItem(int index, int itemNum, int itemValue)
    {
        // Check for subscript out of range
        if (!modServerTCP.IsPlaying(index) || itemNum <= 0 || itemNum > modTypes.MAX_ITEMS)
        {
            return;
        }

        var takeItem = false;

        for (var i = 1; i <= modTypes.MAX_INV; i++)
        {
            // Check to see if the player has the item
            if (modTypes.GetPlayerInvItemNum(index, i) == itemNum)
            {
                if (modTypes.Item[itemNum].Type == modTypes.ITEM_TYPE_CURRENCY)
                {
                    if (itemValue >= modTypes.GetPlayerInvItemValue(index, i))
                    {
                        takeItem = true;
                    }
                    else
                    {
                        modTypes.SetPlayerInvItemValue(index, i, modTypes.GetPlayerInvItemValue(index, i) - itemValue);
                        modServerTCP.SendInventoryUpdate(index, i);
                    }
                }
                else
                {
                    // Check to see if its any sort of ArmorSlot/WeaponSlot
                    switch (modTypes.Item[itemNum].Type)
                    {
                        case modTypes.ITEM_TYPE_WEAPON:
                            var weaponSlot = modTypes.GetPlayerWeaponSlot(index);
                            if (weaponSlot > 0)
                            {
                                if (i == weaponSlot)
                                {
                                    modTypes.SetPlayerWeaponSlot(index, 0);
                                    modServerTCP.SendWornEquipment(index);
                                    takeItem = true;
                                }
                                else
                                {
                                    // Check if the item we are taking isn't already equipped
                                    if (itemNum != modTypes.GetPlayerInvItemNum(index, weaponSlot))
                                    {
                                        takeItem = true;
                                    }
                                }
                            }
                            else
                            {
                                takeItem = true;
                            }

                            break;

                        case modTypes.ITEM_TYPE_ARMOR:
                            var armorSlot = modTypes.GetPlayerArmorSlot(index);
                            if (armorSlot > 0)
                            {
                                if (i == armorSlot)
                                {
                                    modTypes.SetPlayerArmorSlot(index, 0);
                                    modServerTCP.SendWornEquipment(index);
                                    takeItem = true;
                                }
                                else
                                {
                                    // Check if the item we are taking isn't already equipped
                                    if (itemNum != modTypes.GetPlayerInvItemNum(index, armorSlot))
                                    {
                                        takeItem = true;
                                    }
                                }
                            }
                            else
                            {
                                takeItem = true;
                            }

                            break;

                        case modTypes.ITEM_TYPE_HELMET:
                            var helmetSlot = modTypes.GetPlayerHelmetSlot(index);
                            if (helmetSlot > 0)
                            {
                                if (i == helmetSlot)
                                {
                                    modTypes.SetPlayerHelmetSlot(index, 0);
                                    modServerTCP.SendWornEquipment(index);
                                    takeItem = true;
                                }
                                else
                                {
                                    // Check if the item we are taking isn't already equipped
                                    if (itemNum != modTypes.GetPlayerInvItemNum(index, helmetSlot))
                                    {
                                        takeItem = true;
                                    }
                                }
                            }
                            else
                            {
                                takeItem = true;
                            }

                            break;

                        case modTypes.ITEM_TYPE_SHIELD:
                            var shieldSlot = modTypes.GetPlayerShieldSlot(index);
                            if (shieldSlot > 0)
                            {
                                if (i == shieldSlot)
                                {
                                    modTypes.SetPlayerShieldSlot(index, 0);
                                    modServerTCP.SendWornEquipment(index);
                                    takeItem = true;
                                }
                                else
                                {
                                    // Check if the item we are taking isn't already equipped
                                    if (itemNum != modTypes.GetPlayerInvItemNum(index, shieldSlot))
                                    {
                                        takeItem = true;
                                    }
                                }
                            }
                            else
                            {
                                takeItem = true;
                            }

                            break;
                    }

                    var n = modTypes.Item[itemNum].Type;

                    // Check if its not an equipable weapon, and if it isn't then take it away
                    if (n != modTypes.ITEM_TYPE_WEAPON && n != modTypes.ITEM_TYPE_ARMOR && n != modTypes.ITEM_TYPE_HELMET && n != modTypes.ITEM_TYPE_SHIELD)
                    {
                        takeItem = true;
                    }
                }

                if (takeItem)
                {
                    modTypes.SetPlayerInvItemNum(index, i, 0);
                    modTypes.SetPlayerInvItemValue(index, i, 0);
                    modTypes.SetPlayerInvItemDur(index, i, 0);

                    modServerTCP.SendInventoryUpdate(index, i);
                    return;
                }
            }
        }
    }

    public static void GiveItem(int index, int itemNum, int itemVal)
    {
        // Check for subscript out of range
        if (!modServerTCP.IsPlaying(index) || itemNum <= 0 || itemNum > modTypes.MAX_ITEMS)
        {
            return;
        }

        var i = FindOpenInvSlot(index, itemNum);

        // Check to see if inventory is full
        if (i != 0)
        {
            modTypes.SetPlayerInvItemNum(index, i, itemNum);
            modTypes.SetPlayerInvItemValue(index, i, modTypes.GetPlayerInvItemValue(index, i) + itemVal);

            if (modTypes.Item[itemNum].Type == modTypes.ITEM_TYPE_ARMOR ||
                modTypes.Item[itemNum].Type == modTypes.ITEM_TYPE_WEAPON ||
                modTypes.Item[itemNum].Type == modTypes.ITEM_TYPE_HELMET ||
                modTypes.Item[itemNum].Type == modTypes.ITEM_TYPE_SHIELD)
            {
                modTypes.SetPlayerInvItemDur(index, i, modTypes.Item[itemNum].Data1);
            }

            modServerTCP.SendInventoryUpdate(index, i);
        }
        else
        {
            modServerTCP.PlayerMsg(index, "Your inventory is full.", modText.BrightRed);
        }
    }

    public static void SpawnItem(int itemNum, int itemVal, int mapNum, int x, int y)
    {
        // Check for subscript out of range
        if (itemNum <= 0 || itemNum > modTypes.MAX_ITEMS || mapNum <= 0 || mapNum > modTypes.MAX_MAPS)
        {
            return;
        }

        // Find open map item slot
        var i = FindOpenMapItemSlot(mapNum);

        SpawnItemSlot(i, itemNum, itemVal, modTypes.Item[itemNum].Data1, mapNum, x, y);
    }

    public static void SpawnItemSlot(int mapItemSlot, int itemNum, int itemVal, int itemDur, int mapNum, int x, int y)
    {
        // Check for subscript out of range
        if (mapItemSlot <= 0 || mapItemSlot > modTypes.MAX_MAP_ITEMS || itemNum < 0 || itemNum > modTypes.MAX_ITEMS || mapNum <= 0 || mapNum > modTypes.MAX_MAPS)
        {
            return;
        }

        modTypes.MapItem[mapNum, mapItemSlot].Num = itemNum;
        modTypes.MapItem[mapNum, mapItemSlot].Value = itemVal;

        if (itemNum != 0)
        {
            if (modTypes.Item[itemNum].Type >= modTypes.ITEM_TYPE_WEAPON && modTypes.Item[itemNum].Type <= modTypes.ITEM_TYPE_SHIELD)
            {
                modTypes.MapItem[mapNum, mapItemSlot].Dur = itemDur;
            }
            else
            {
                modTypes.MapItem[mapNum, mapItemSlot].Dur = 0;
            }
        }

        modTypes.MapItem[mapNum, mapItemSlot].X = x;
        modTypes.MapItem[mapNum, mapItemSlot].Y = y;

        var packet =
            "SPAWNITEM" +
            modTypes.SEP_CHAR + mapItemSlot +
            modTypes.SEP_CHAR + itemNum +
            modTypes.SEP_CHAR + itemVal +
            modTypes.SEP_CHAR + modTypes.MapItem[mapNum, mapItemSlot].Dur +
            modTypes.SEP_CHAR + x +
            modTypes.SEP_CHAR + y +
            modTypes.SEP_CHAR;

        modServerTCP.SendDataToMap(mapNum, packet);
    }

    public static void SpawnAllMapsItems()
    {
        for (var i = 1; i <= modTypes.MAX_MAPS; i++)
        {
            SpawnMapItems(i);
        }
    }

    public static void SpawnMapItems(int mapNum)
    {
        // Check for subscript out of range
        if (mapNum is <= 0 or > modTypes.MAX_MAPS)
        {
            return;
        }

        // Spawn what we have
        for (var y = 0; y <= modTypes.MAX_MAPY; y++)
        {
            for (var x = 0; x <= modTypes.MAX_MAPX; x++)
            {
                if (modTypes.Map[mapNum].Tile[x, y].Type != modTypes.TILE_TYPE_ITEM)
                {
                    continue;
                }

                // Check to see if its a currency and if they set the value to 0 set it to 1 automatically
                if (modTypes.Item[modTypes.Map[mapNum].Tile[x, y].Data1].Type == modTypes.ITEM_TYPE_CURRENCY && modTypes.Map[mapNum].Tile[x, y].Data2 <= 0)
                {
                    SpawnItem(modTypes.Map[mapNum].Tile[x, y].Data1, 1, mapNum, x, y);
                }
                else
                {
                    SpawnItem(modTypes.Map[mapNum].Tile[x, y].Data1, modTypes.Map[mapNum].Tile[x, y].Data2, mapNum, x, y);
                }
            }
        }
    }

    public static void PlayerMapGetItem(int index)
    {
        if (modServerTCP.IsPlaying(index))
        {
            return;
        }

        var mapNum = modTypes.GetPlayerMap(index);

        for (var i = 1; i <= modTypes.MAX_MAP_ITEMS; i++)
        {
            // See if theres even an item here
            if (modTypes.MapItem[mapNum, i].Num > 0 && modTypes.MapItem[mapNum, i].Num <= modTypes.MAX_ITEMS)
            {
                // Check if item is at the same location as the player
                if (modTypes.MapItem[mapNum, i].X == modTypes.GetPlayerX(index) &&
                    modTypes.MapItem[mapNum, i].Y == modTypes.GetPlayerY(index))
                {
                    // Find open slot
                    var n = FindOpenInvSlot(index, modTypes.MapItem[mapNum, i].Num);
                    if (n != 0)
                    {
                        string msg;

                        // Set item in players inventoru
                        modTypes.SetPlayerInvItemNum(index, n, modTypes.MapItem[mapNum, i].Num);
                        if (modTypes.Item[modTypes.GetPlayerInvItemNum(index, n)].Type == modTypes.ITEM_TYPE_CURRENCY)
                        {
                            modTypes.SetPlayerInvItemValue(index, n, modTypes.GetPlayerInvItemValue(index, n) + modTypes.MapItem[mapNum, i].Value);
                            msg = $"You picked up {modTypes.MapItem[mapNum, i].Value} {modTypes.Item[modTypes.GetPlayerInvItemNum(index, n)].Name.Trim()}.";
                        }
                        else
                        {
                            modTypes.SetPlayerInvItemValue(index, n, 0);
                            msg = $"You picked up a {modTypes.Item[modTypes.GetPlayerInvItemNum(index, n)].Name.Trim()}.";
                        }

                        modTypes.SetPlayerInvItemDur(index, n, modTypes.MapItem[mapNum, n].Dur);

                        // Erase item from the map
                        modTypes.MapItem[mapNum, n].Num = 0;
                        modTypes.MapItem[mapNum, n].Value = 0;
                        modTypes.MapItem[mapNum, n].Dur = 0;
                        modTypes.MapItem[mapNum, n].X = 0;
                        modTypes.MapItem[mapNum, n].Y = 0;

                        modServerTCP.SendInventoryUpdate(index, n);
                        SpawnItemSlot(i, 0, 0, 0, modTypes.GetPlayerMap(index), modTypes.GetPlayerX(index), modTypes.GetPlayerY(index));
                        modServerTCP.PlayerMsg(index, msg, modText.Yellow);
                        return;
                    }
                    else
                    {
                        modServerTCP.PlayerMsg(index, "Your inventory is full.", modText.BrightRed);
                        return;
                    }
                }
            }
        }
    }

    public static void PlayerMapDropItem(int index, int invNum, int amount)
    {
        // Check for subscript out of range
        if (!modServerTCP.IsPlaying(index) || invNum <= 0 || invNum > modTypes.MAX_ITEMS)
        {
            return;
        }

        var itemNum = modTypes.GetPlayerInvItemNum(index, invNum);
        if (itemNum is > 0 and <= modTypes.MAX_ITEMS)
        {
            var mapNum = modTypes.GetPlayerMap(index);
            var i = FindOpenMapItemSlot(mapNum);

            if (i != 0)
            {
                modTypes.MapItem[mapNum, i].Dur = 0;

                // Check to see if its any sort of ArmorSlot/WeaponSlot
                switch (modTypes.Item[itemNum].Type)
                {
                    case modTypes.ITEM_TYPE_ARMOR:
                        if (invNum == modTypes.GetPlayerArmorSlot(index))
                        {
                            modTypes.SetPlayerArmorSlot(index, 0);
                            modServerTCP.SendWornEquipment(index);
                        }

                        modTypes.MapItem[mapNum, i].Dur = modTypes.GetPlayerInvItemDur(index, invNum);
                        break;

                    case modTypes.ITEM_TYPE_WEAPON:
                        if (invNum == modTypes.GetPlayerWeaponSlot(index))
                        {
                            modTypes.SetPlayerArmorSlot(index, 0);
                            modServerTCP.SendWornEquipment(index);
                        }

                        modTypes.MapItem[mapNum, i].Dur = modTypes.GetPlayerInvItemDur(index, invNum);
                        break;

                    case modTypes.ITEM_TYPE_HELMET:
                        if (invNum == modTypes.GetPlayerHelmetSlot(index))
                        {
                            modTypes.SetPlayerHelmetSlot(index, 0);
                            modServerTCP.SendWornEquipment(index);
                        }

                        modTypes.MapItem[mapNum, i].Dur = modTypes.GetPlayerInvItemDur(index, invNum);
                        break;

                    case modTypes.ITEM_TYPE_SHIELD:
                        if (invNum == modTypes.GetPlayerShieldSlot(index))
                        {
                            modTypes.SetPlayerShieldSlot(index, 0);
                            modServerTCP.SendWornEquipment(index);
                        }

                        modTypes.MapItem[mapNum, i].Dur = modTypes.GetPlayerInvItemDur(index, invNum);
                        break;
                }

                modTypes.MapItem[mapNum, i].Num = itemNum;
                modTypes.MapItem[mapNum, i].X = modTypes.GetPlayerX(index);
                modTypes.MapItem[mapNum, i].Y = modTypes.GetPlayerY(index);

                if (modTypes.Item[itemNum].Type == modTypes.ITEM_TYPE_CURRENCY)
                {
                    var invItemValue = modTypes.GetPlayerInvItemValue(index, invNum);
                    if (amount >= invItemValue)
                    {
                        modTypes.MapItem[mapNum, i].Value = invItemValue;
                        modServerTCP.MapMsg(mapNum, $"{modTypes.GetPlayerName(index)} drops {invItemValue} {modTypes.Item[itemNum].Name.Trim()}.", modText.Yellow);
                        modTypes.SetPlayerInvItemNum(index, invNum, 0);
                        modTypes.SetPlayerInvItemValue(index, invNum, 0);
                        modTypes.SetPlayerInvItemDur(index, invNum, 0);
                    }
                    else
                    {
                        modTypes.MapItem[mapNum, i].Value = amount;
                        modServerTCP.MapMsg(mapNum, $"{modTypes.GetPlayerName(index)} drops {amount} {modTypes.Item[itemNum].Name.Trim()}.", modText.Yellow);
                        modTypes.SetPlayerInvItemValue(index, invNum, invItemValue - amount);
                    }
                }
                else
                {
                    // Its not a currency object so this is easy
                    modTypes.MapItem[mapNum, i].Num = itemNum;
                    if (modTypes.Item[itemNum].Type >= modTypes.ITEM_TYPE_CURRENCY && modTypes.Item[itemNum].Type <= modTypes.ITEM_TYPE_SHIELD)
                    {
                        modServerTCP.MapMsg(mapNum, $"{modTypes.GetPlayerName(index)} drops a {modTypes.Item[itemNum].Name.Trim()} {modTypes.GetPlayerInvItemDur(index, invNum)}/{modTypes.Item[itemNum].Data1}.", modText.Yellow);
                    }
                    else
                    {
                        modServerTCP.MapMsg(mapNum, $"{modTypes.GetPlayerName(index)} drops a {modTypes.Item[itemNum].Name.Trim()}.", modText.Yellow);
                    }

                    modTypes.SetPlayerInvItemNum(index, invNum, 0);
                    modTypes.SetPlayerInvItemValue(index, invNum, 0);
                    modTypes.SetPlayerInvItemDur(index, invNum, 0);
                }

                // Send inventory update
                modServerTCP.SendInventoryUpdate(index, invNum);

                // Spawn the item before we set the num or we'll get a different free map item slot
                SpawnItemSlot(i,
                    modTypes.MapItem[mapNum, i].Num, amount,
                    modTypes.MapItem[mapNum, i].Dur,
                    modTypes.GetPlayerMap(index),
                    modTypes.GetPlayerX(index),
                    modTypes.GetPlayerY(index));
            }
            else
            {
                modServerTCP.PlayerMsg(index, "Too many items already on the ground.", modText.BrightRed);
            }
        }
    }

    public static void SpawnNpc(int mapNpcNum, int mapNum)
    {
        // Check for subscript out of range
        if (mapNpcNum <= 0 || mapNpcNum > modTypes.MAX_MAP_NPCS || mapNum <= 0 || mapNum > modTypes.MAX_MAPS)
        {
            return;
        }

        var spawned = false;

        var npcNum = modTypes.Map[mapNum].Npc[mapNpcNum];
        if (npcNum > 0)
        {
            modTypes.MapNpc[mapNum, mapNpcNum].Num = npcNum;
            modTypes.MapNpc[mapNum, mapNpcNum].Target = 0;

            modTypes.MapNpc[mapNum, mapNpcNum].HP = GetNpcMaxHP(npcNum);
            modTypes.MapNpc[mapNum, mapNpcNum].MP = GetNpcMaxMP(npcNum);
            modTypes.MapNpc[mapNum, mapNpcNum].SP = GetNpcMaxSP(npcNum);

            modTypes.MapNpc[mapNum, mapNpcNum].Dir = Random.Shared.Next(0, 4);

            // Well try 100 times to randomly place the sprite
            for (var i = 1; i <= 100; i++)
            {
                var x = Random.Shared.Next(0, modTypes.MAX_MAPX + 1);
                var y = Random.Shared.Next(0, modTypes.MAX_MAPY + 1);

                if (modTypes.Map[mapNum].Tile[x, y].Type == modTypes.TILE_TYPE_WALKABLE)
                {
                    modTypes.MapNpc[mapNum, mapNpcNum].X = x;
                    modTypes.MapNpc[mapNum, mapNpcNum].Y = y;
                    spawned = true;
                    break;
                }
            }

            // Didn't spawn, so now we'll just try to find a free tile
            if (!spawned)
            {
                for (var y = 0; y <= modTypes.MAX_MAPY; y++)
                {
                    for (var x = 0; x <= modTypes.MAX_MAPX; x++)
                    {
                        if (modTypes.Map[mapNum].Tile[x, y].Type == modTypes.TILE_TYPE_WALKABLE)
                        {
                            modTypes.MapNpc[mapNum, mapNpcNum].X = x;
                            modTypes.MapNpc[mapNum, mapNpcNum].Y = y;
                            spawned = true;
                            break;
                        }
                    }
                }
            }

            // If we suceeded in spawning then send it to everyone
            if (spawned)
            {
                var packet = "SPAWNNPC" +
                             modTypes.SEP_CHAR + mapNpcNum +
                             modTypes.SEP_CHAR + modTypes.MapNpc[mapNum, mapNpcNum].Num +
                             modTypes.SEP_CHAR + modTypes.MapNpc[mapNum, mapNpcNum].X +
                             modTypes.SEP_CHAR + modTypes.MapNpc[mapNum, mapNpcNum].Y +
                             modTypes.SEP_CHAR + modTypes.MapNpc[mapNum, mapNpcNum].Dir +
                             modTypes.SEP_CHAR;

                modServerTCP.SendDataToMap(mapNum, packet);
            }
        }
    }

    public static void SpawnMapNpcs(int mapNum)
    {
        for (var i = 1; i <= modTypes.MAX_MAP_NPCS; i++)
        {
            SpawnNpc(i, mapNum);
        }
    }

    public static void SpawnAllMapNpcs()
    {
        for (var i = 1; i <= modTypes.MAX_MAPS; i++)
        {
            SpawnMapNpcs(i);
        }
    }

    private static bool TryAttackPlayer(int attacker, int victim)
    {
        const int minPvpLevel = 10;

        // Check to make sure that they dont have access
        if (modTypes.GetPlayerAccess(attacker) > modTypes.ADMIN_MONITER)
        {
            modServerTCP.PlayerMsg(attacker, "You cannot attack any player for thou art an admin!", modText.BrightBlue);
            return false;
        }

        // Check to make sure the victim isn't an admin
        if (modTypes.GetPlayerAccess(victim) > modTypes.ADMIN_MONITER)
        {
            modServerTCP.PlayerMsg(attacker, $"You cannot attack {modTypes.GetPlayerName(victim)}!", modText.BrightRed);
            return false;
        }

        // Check if map is attackable
        if (modTypes.Map[modTypes.GetPlayerMap(attacker)].Moral == modTypes.MAP_MORAL_SAFE && modTypes.GetPlayerPK(victim) == modTypes.NO)
        {
            modServerTCP.PlayerMsg(attacker, "This is a safe zone!", modText.BrightRed);
            return false;
        }

        // Make sure they are high enough level
        if (modTypes.GetPlayerLevel(attacker) < minPvpLevel)
        {
            modServerTCP.PlayerMsg(attacker, $"You are below level {minPvpLevel}, you cannot attack another player yet!", modText.BrightRed);
            return false;
        }

        if (modTypes.GetPlayerLevel(victim) >= minPvpLevel)
        {
            return true;
        }

        modServerTCP.PlayerMsg(attacker, $"{modTypes.GetPlayerName(victim)} is below level {minPvpLevel}, you cannot attack this player yet!", modText.BrightRed);
        return false;
    }

    public static bool CanAttackPlayer(int attacker, int victim)
    {
        // Check for subscript out of range
        if (!modServerTCP.IsPlaying(attacker) || !modServerTCP.IsPlaying(victim))
        {
            return false;
        }

        // Make sure they have more then 0 hp
        if (modTypes.GetPlayerHP(victim) <= 0)
        {
            return false;
        }

        // Make sure we dont attack the player if they are switching maps
        if (modTypes.Player[victim].GettingMap == modTypes.YES)
        {
            return false;
        }

        // Make sure they are on the same map
        if (modTypes.GetPlayerMap(attacker) != modTypes.GetPlayerMap(victim) || modGeneral.GetTickCount() <= modTypes.Player[attacker].AttackTimer + 950)
        {
            return false;
        }

        // Check if at same coordinates
        switch (modTypes.GetPlayerDir(attacker))
        {
            case modTypes.DIR_UP:
                if (modTypes.GetPlayerY(victim) + 1 == modTypes.GetPlayerY(attacker) && modTypes.GetPlayerX(victim) == modTypes.GetPlayerX(attacker))
                {
                    return TryAttackPlayer(attacker, victim);
                }

                break;

            case modTypes.DIR_DOWN:
                if (modTypes.GetPlayerY(victim) - 1 == modTypes.GetPlayerY(attacker) && modTypes.GetPlayerX(victim) == modTypes.GetPlayerX(attacker))
                {
                    return TryAttackPlayer(attacker, victim);
                }

                break;

            case modTypes.DIR_LEFT:
                if (modTypes.GetPlayerY(victim) == modTypes.GetPlayerY(attacker) && modTypes.GetPlayerX(victim) + 1 == modTypes.GetPlayerX(attacker))
                {
                    return TryAttackPlayer(attacker, victim);
                }

                break;

            case modTypes.DIR_RIGHT:
                if (modTypes.GetPlayerY(victim) == modTypes.GetPlayerY(attacker) && modTypes.GetPlayerX(victim) - 1 == modTypes.GetPlayerX(attacker))
                {
                    return TryAttackPlayer(attacker, victim);
                }

                break;
        }

        return false;
    }

    public static bool CanAttackNpc(int attacker, int mapNpcNum)
    {
        // Check for subscript out of range
        if (!modServerTCP.IsPlaying(attacker) || mapNpcNum <= 0 || mapNpcNum > modTypes.MAX_MAP_NPCS)
        {
            return false;
        }

        // Check for subscript out of range
        if (modTypes.MapNpc[modTypes.GetPlayerMap(attacker), mapNpcNum].Num <= 0)
        {
            return false;
        }

        var mapNum = modTypes.GetPlayerMap(attacker);
        var npcNum = modTypes.MapNpc[mapNum, mapNpcNum].Num;

        // Make sure the npc isn't already dead
        if (modTypes.MapNpc[mapNum, mapNpcNum].HP <= 0)
        {
            return false;
        }

        if (npcNum > 0 && modGeneral.GetTickCount() > modTypes.Player[attacker].AttackTimer + 950)
        {
            switch (modTypes.GetPlayerDir(attacker))
            {
                case modTypes.DIR_UP:
                    if (modTypes.MapNpc[mapNum, mapNpcNum].Y + 1 == modTypes.GetPlayerX(attacker) && modTypes.MapNpc[mapNum, mapNpcNum].X == modTypes.GetPlayerX(attacker))
                    {
                        if (modTypes.Npc[npcNum].Behavior == modTypes.NPC_BEHAVIOR_FRIENDLY || modTypes.Npc[npcNum].Behavior == modTypes.NPC_BEHAVIOR_SHOPKEEPER)
                        {
                            return true;
                        }

                        modServerTCP.PlayerMsg(attacker, $"You cannot attack a {modTypes.Npc[npcNum].Name.Trim()}!", modText.BrightBlue);
                    }

                    break;

                case modTypes.DIR_DOWN:
                    if (modTypes.MapNpc[mapNum, mapNpcNum].Y - 1 == modTypes.GetPlayerX(attacker) && modTypes.MapNpc[mapNum, mapNpcNum].X == modTypes.GetPlayerX(attacker))
                    {
                        if (modTypes.Npc[npcNum].Behavior == modTypes.NPC_BEHAVIOR_FRIENDLY || modTypes.Npc[npcNum].Behavior == modTypes.NPC_BEHAVIOR_SHOPKEEPER)
                        {
                            return true;
                        }

                        modServerTCP.PlayerMsg(attacker, $"You cannot attack a {modTypes.Npc[npcNum].Name.Trim()}!", modText.BrightBlue);
                    }

                    break;

                case modTypes.DIR_LEFT:
                    if (modTypes.MapNpc[mapNum, mapNpcNum].Y == modTypes.GetPlayerX(attacker) && modTypes.MapNpc[mapNum, mapNpcNum].X + 1 == modTypes.GetPlayerX(attacker))
                    {
                        if (modTypes.Npc[npcNum].Behavior == modTypes.NPC_BEHAVIOR_FRIENDLY || modTypes.Npc[npcNum].Behavior == modTypes.NPC_BEHAVIOR_SHOPKEEPER)
                        {
                            return true;
                        }

                        modServerTCP.PlayerMsg(attacker, $"You cannot attack a {modTypes.Npc[npcNum].Name.Trim()}!", modText.BrightBlue);
                    }

                    break;

                case modTypes.DIR_RIGHT:
                    if (modTypes.MapNpc[mapNum, mapNpcNum].Y == modTypes.GetPlayerX(attacker) && modTypes.MapNpc[mapNum, mapNpcNum].X - 1 == modTypes.GetPlayerX(attacker))
                    {
                        if (modTypes.Npc[npcNum].Behavior == modTypes.NPC_BEHAVIOR_FRIENDLY || modTypes.Npc[npcNum].Behavior == modTypes.NPC_BEHAVIOR_SHOPKEEPER)
                        {
                            return true;
                        }

                        modServerTCP.PlayerMsg(attacker, $"You cannot attack a {modTypes.Npc[npcNum].Name.Trim()}!", modText.BrightBlue);
                    }

                    break;
            }
        }

        return false;
    }

    public static bool CanNpcAttackPlayer(int mapNpcNum, int index)
    {
        // Check for subscript out of range
        if (mapNpcNum <= 0 || mapNpcNum > modTypes.MAX_MAP_NPCS || !modServerTCP.IsPlaying(index))
        {
            return false;
        }

        // Check for subscript out of range
        if (modTypes.MapNpc[modTypes.GetPlayerMap(index), mapNpcNum].Num <= 0)
        {
            return false;
        }

        var mapNum = modTypes.GetPlayerMap(index);
        var npcNum = modTypes.MapNpc[mapNum, mapNpcNum].Num;

        // Make sure the npc isn't already dead
        if (modTypes.MapNpc[mapNum, mapNpcNum].HP <= 0)
        {
            return false;
        }

        // Make sure npcs dont attack more then once a second
        if (modGeneral.GetTickCount() < modTypes.MapNpc[mapNum, mapNpcNum].AttackTimer + 1000)
        {
            return false;
        }

        // Make sure we dont attack the player if they are switching maps
        if (modTypes.Player[index].GettingMap == modTypes.YES)
        {
            return false;
        }

        modTypes.MapNpc[mapNum, mapNpcNum].AttackTimer = modGeneral.GetTickCount();

        var playerX = modTypes.GetPlayerX(index);
        var playerY = modTypes.GetPlayerY(index);

        if (npcNum > 0)
        {
            if (playerY + 1 == modTypes.MapNpc[mapNum, mapNpcNum].Y && playerX == modTypes.MapNpc[mapNum, mapNpcNum].X)
            {
                return true;
            }

            if (playerY - 1 == modTypes.MapNpc[mapNum, mapNpcNum].Y && playerX == modTypes.MapNpc[mapNum, mapNpcNum].X)
            {
                return true;
            }

            if (playerY == modTypes.MapNpc[mapNum, mapNpcNum].Y && playerX + 1 == modTypes.MapNpc[mapNum, mapNpcNum].X)
            {
                return true;
            }

            if (playerY == modTypes.MapNpc[mapNum, mapNpcNum].Y && playerX - 1 == modTypes.MapNpc[mapNum, mapNpcNum].X)
            {
                return true;
            }
        }

        return false;
    }

    public static void AttackPlayer(int attacker, int victim, int damage)
    {
        // Check for subscript out of range
        if (!modServerTCP.IsPlaying(attacker) || !modServerTCP.IsPlaying(victim) || damage < 0)
        {
            return;
        }

        // Check for weapon
        var weaponSlot = modTypes.GetPlayerWeaponSlot(attacker);
        var weaponItemNum = weaponSlot > 0 ? modTypes.GetPlayerInvItemNum(attacker, weaponSlot) : 0;

        // Send this packet so they can see the person attacking
        modServerTCP.SendDataToMapBut(attacker, modTypes.GetPlayerMap(attacker), "ATTACK" + modTypes.SEP_CHAR + attacker + modTypes.SEP_CHAR);

        if (damage >= modTypes.GetPlayerHP(victim))
        {
            // Set HP to nothing
            modTypes.SetPlayerHP(victim, 0);

            // Check for a weapon and say damage
            if (weaponItemNum == 0)
            {
                modServerTCP.PlayerMsg(attacker, $"You hit {modTypes.GetPlayerName(victim)} for {damage} hit points.", modText.White);
                modServerTCP.PlayerMsg(victim, $"{modTypes.GetPlayerName(attacker)} hit you for {damage} hit points.", modText.BrightRed);
            }
            else
            {
                modServerTCP.PlayerMsg(attacker, $"You hit {modTypes.GetPlayerName(victim)} with a {modTypes.Item[weaponItemNum].Name.Trim()} for {damage} hit points.", modText.White);
                modServerTCP.PlayerMsg(victim, $"{modTypes.GetPlayerName(attacker)} hit you with a {modTypes.Item[weaponItemNum].Name.Trim()} for {damage} hit points.", modText.BrightRed);
            }

            // Player is dead
            modServerTCP.GlobalMsg($"{modTypes.GetPlayerName(victim)} has been killed by {modTypes.GetPlayerName(attacker)}.", modText.BrightRed);

            // Drop all worn items by victim
            if (modTypes.GetPlayerWeaponSlot(victim) > 0)
            {
                PlayerMapDropItem(victim, modTypes.GetPlayerWeaponSlot(victim), 0);
            }

            if (modTypes.GetPlayerArmorSlot(victim) > 0)
            {
                PlayerMapDropItem(victim, modTypes.GetPlayerArmorSlot(victim), 0);
            }

            if (modTypes.GetPlayerHelmetSlot(victim) > 0)
            {
                PlayerMapDropItem(victim, modTypes.GetPlayerHelmetSlot(victim), 0);
            }

            if (modTypes.GetPlayerShieldSlot(victim) > 0)
            {
                PlayerMapDropItem(victim, modTypes.GetPlayerShieldSlot(victim), 0);
            }

            // Calculate exp to give attacker
            var exp = modTypes.GetPlayerExp(victim) / 10;

            // Make sure we dont get less then 0
            if (exp < 0)
            {
                exp = 0;
            }

            if (exp == 0)
            {
                modServerTCP.PlayerMsg(victim, "You lost no experience points.", modText.BrightRed);
                modServerTCP.PlayerMsg(attacker, "You received no experience points from that weak insignificant player.", modText.BrightBlue);
            }
            else
            {
                modTypes.SetPlayerExp(victim, modTypes.GetPlayerExp(victim) - exp);
                modServerTCP.PlayerMsg(victim, $"You lost {exp} experience points.", modText.BrightRed);
                modTypes.SetPlayerExp(attacker, modTypes.GetPlayerExp(attacker) + exp);
                modServerTCP.PlayerMsg(attacker, $"You got {exp} experience points for killing {modTypes.GetPlayerName(victim)}.", modText.BrightBlue);
            }

            // Warp player away
            PlayerWarp(victim, modDatabase.START_MAP, modDatabase.START_X, modDatabase.START_Y);

            // Restore vitals
            modTypes.SetPlayerHP(victim, modTypes.GetPlayerMaxHP(victim));
            modTypes.SetPlayerMP(victim, modTypes.GetPlayerMaxMP(victim));
            modTypes.SetPlayerSP(victim, modTypes.GetPlayerMaxSP(victim));
            modServerTCP.SendHP(victim);
            modServerTCP.SendMP(victim);
            modServerTCP.SendSP(victim);

            // Check for a level up
            CheckPlayerLevelUp(attacker);

            // Check if target is player who died and if so set target to 0
            if (modTypes.Player[attacker].TargetType == modTypes.TARGET_TYPE_PLAYER && modTypes.Player[victim].Target == victim)
            {
                modTypes.Player[victim].Target = 0;
                modTypes.Player[victim].TargetType = 0;
            }

            if (modTypes.GetPlayerPK(victim) == modTypes.NO)
            {
                if (modTypes.GetPlayerPK(attacker) == modTypes.NO)
                {
                    modTypes.SetPlayerPK(attacker, modTypes.YES);
                    modServerTCP.SendPlayerData(attacker);
                    modServerTCP.GlobalMsg($"{modTypes.GetPlayerName(attacker)} has been deemed a Player Killer!!!", modText.BrightRed);
                }
            }
            else
            {
                modTypes.SetPlayerPK(victim, modTypes.NO);
                modServerTCP.SendPlayerData(victim);
                modServerTCP.GlobalMsg($"{modTypes.GetPlayerName(victim)} has paid the price for being a Player Killer!!!", modText.BrightRed);
            }
        }
        else
        {
            // Player not dead, just do the damage
            modTypes.SetPlayerHP(victim, modTypes.GetPlayerHP(victim) - damage);
            modServerTCP.SendHP(victim);

            // Check for a weapon and say damage
            if (weaponItemNum == 0)
            {
                modServerTCP.PlayerMsg(attacker, $"You hit {modTypes.GetPlayerName(victim)} for {damage} hit points.", modText.White);
                modServerTCP.PlayerMsg(victim, $"{modTypes.GetPlayerName(attacker)} hit you for {damage} hit points.", modText.BrightRed);
            }
            else
            {
                modServerTCP.PlayerMsg(attacker, $"You hit {modTypes.GetPlayerName(victim)} with a {modTypes.Item[weaponItemNum].Name.Trim()} for {damage} hit points.", modText.White);
                modServerTCP.PlayerMsg(victim, $"{modTypes.GetPlayerName(attacker)} hit you with a {modTypes.Item[weaponItemNum].Name.Trim()} for {damage} hit points.", modText.BrightRed);
            }
        }

        // Reset timer for attacking
        modTypes.Player[attacker].AttackTimer = modGeneral.GetTickCount();
    }

    public static void NpcAttackPlayer(int mapNpcNum, int victim, int damage)
    {
        // Check for subscript out of range
        if (mapNpcNum <= 0 || mapNpcNum > modTypes.MAX_MAP_NPCS || !modServerTCP.IsPlaying(victim) || damage < 0)
        {
            return;
        }

        // Check for subscript out of range
        if (modTypes.MapNpc[modTypes.GetPlayerMap(victim), mapNpcNum].Num <= 0)
        {
            return;
        }

        // Send this packet so they can see the person attacking
        modServerTCP.SendDataToMap(modTypes.GetPlayerMap(victim), "NPCATTACK" + modTypes.SEP_CHAR + mapNpcNum + modTypes.SEP_CHAR);

        var mapNum = modTypes.GetPlayerMap(victim);
        var name = modTypes.Npc[modTypes.MapNpc[mapNum, mapNpcNum].Num].Name.Trim();

        if (damage >= modTypes.GetPlayerHP(victim))
        {
            // Say damage
            modServerTCP.PlayerMsg(victim, $"A {name} hit you for {damage} hit points.", modText.BrightRed);

            // Player is dead
            modServerTCP.GlobalMsg($"{modTypes.GetPlayerName(victim)} has been killed by a {name}.", modText.BrightRed);

            // Drop all worn items by victim
            if (modTypes.GetPlayerWeaponSlot(victim) > 0)
            {
                PlayerMapDropItem(victim, modTypes.GetPlayerWeaponSlot(victim), 0);
            }

            if (modTypes.GetPlayerArmorSlot(victim) > 0)
            {
                PlayerMapDropItem(victim, modTypes.GetPlayerArmorSlot(victim), 0);
            }

            if (modTypes.GetPlayerHelmetSlot(victim) > 0)
            {
                PlayerMapDropItem(victim, modTypes.GetPlayerHelmetSlot(victim), 0);
            }

            if (modTypes.GetPlayerShieldSlot(victim) > 0)
            {
                PlayerMapDropItem(victim, modTypes.GetPlayerShieldSlot(victim), 0);
            }

            // Calculate exp to give attacker
            var exp = modTypes.GetPlayerExp(victim) / 3;

            // Make sure we dont get less then 0
            if (exp < 0)
            {
                exp = 0;
            }

            if (exp == 0)
            {
                modServerTCP.PlayerMsg(victim, "You lost no experience points.", modText.BrightRed);
            }
            else
            {
                modTypes.SetPlayerExp(victim, modTypes.GetPlayerExp(victim) - exp);
                modServerTCP.PlayerMsg(victim, $"You lost {exp} experience points.", modText.BrightRed);
            }

            // Warp player away
            PlayerWarp(victim, modDatabase.START_MAP, modDatabase.START_X, modDatabase.START_Y);

            // Restore vitals
            modTypes.SetPlayerHP(victim, modTypes.GetPlayerMaxHP(victim));
            modTypes.SetPlayerMP(victim, modTypes.GetPlayerMaxMP(victim));
            modTypes.SetPlayerSP(victim, modTypes.GetPlayerMaxSP(victim));
            modServerTCP.SendHP(victim);
            modServerTCP.SendMP(victim);
            modServerTCP.SendSP(victim);

            // Set NPC target to 0
            modTypes.MapNpc[mapNum, mapNpcNum].Target = 0;

            // If the player the attacker killed was a pk then take it away
            if (modTypes.GetPlayerPK(victim) == modTypes.YES)
            {
                modTypes.SetPlayerPK(victim, modTypes.NO);
                modServerTCP.SendPlayerData(victim);
            }
        }
        else
        {
            // Player not dead, just do the damage
            modTypes.SetPlayerHP(victim, modTypes.GetPlayerHP(victim) - damage);
            modServerTCP.SendHP(victim);

            // Say damage
            modServerTCP.PlayerMsg(victim, $"A {name} hit you for {damage} hit points.", modText.BrightRed);
        }
    }

    public static void AttackNpc(int attacker, int mapNpcNum, int damage)
    {
        // Check for subscript out of range
        if (!modServerTCP.IsPlaying(attacker) || mapNpcNum <= 0 || mapNpcNum > modTypes.MAX_MAP_NPCS || damage < 0)
        {
            return;
        }

        // Check for weapon
        var n = 0;
        if (modTypes.GetPlayerWeaponSlot(attacker) > 0)
        {
            n = modTypes.GetPlayerInvItemNum(attacker, modTypes.GetPlayerWeaponSlot(attacker));
        }

        // Send this packet so they can see the person attacking
        modServerTCP.SendDataToMapBut(attacker, modTypes.GetPlayerMap(attacker), "ATTACK" + modTypes.SEP_CHAR + attacker + modTypes.SEP_CHAR);

        var mapNum = modTypes.GetPlayerMap(attacker);
        var npcNum = modTypes.MapNpc[mapNum, mapNpcNum].Num;
        var name = modTypes.Npc[npcNum].Name.Trim();

        if (damage >= modTypes.MapNpc[mapNum, mapNpcNum].HP)
        {
            // Check for a weapon and say damage
            if (n == 0)
            {
                modServerTCP.PlayerMsg(attacker, $"You hit a {name} for {damage} hit points, killing it.", modText.BrightRed);
            }
            else
            {
                modServerTCP.PlayerMsg(attacker, $"You hit a {name} with a {modTypes.Item[n].Name.Trim()} for {damage} hit points, killing it.", modText.BrightRed);
            }

            // Calculate exp to give attacker
            var str = modTypes.Npc[npcNum].STR;
            var def = modTypes.Npc[npcNum].DEF;
            var exp = str * def * 2;

            // Make sure we dont get less then 0
            if (exp < 0)
            {
                exp = 1;
            }

            // Check if in party, if so divide the exp up by 2
            if (modTypes.Player[attacker].InParty == modTypes.NO)
            {
                modTypes.SetPlayerExp(attacker, modTypes.GetPlayerExp(attacker) + exp);
                modServerTCP.PlayerMsg(attacker, $"You have gained {exp} experience points.", modText.BrightBlue);
            }
            else
            {
                exp /= 2;

                if (exp <= 0)
                {
                    exp = 1;
                }

                modTypes.SetPlayerExp(attacker, modTypes.GetPlayerExp(attacker) + exp);
                modServerTCP.PlayerMsg(attacker, $"You have gained {exp} party experience points.", modText.BrightBlue);

                var partyPlayer = modTypes.Player[attacker].PartyPlayer;
                if (partyPlayer > 0)
                {
                    modTypes.SetPlayerExp(partyPlayer, modTypes.GetPlayerExp(attacker) + exp);
                    modServerTCP.PlayerMsg(partyPlayer, $"You have gained {exp} party experience points.", modText.BrightBlue);
                }
            }

            // Drop the goods if they get it
            n = Random.Shared.Next(modTypes.Npc[npcNum].DropChance) + 1;
            if (n == 1)
            {
                SpawnItem(
                    modTypes.Npc[npcNum].DropItem,
                    modTypes.Npc[npcNum].DropItemValue,
                    mapNum,
                    modTypes.MapNpc[mapNum, mapNpcNum].X,
                    modTypes.MapNpc[mapNum, mapNpcNum].Y);
            }

            // Now set HP to 0 so we know to actually kill them in the server loop (this prevents subscript out of range)
            modTypes.MapNpc[mapNum, mapNpcNum].Num = 0;
            modTypes.MapNpc[mapNum, mapNpcNum].SpawnWait = modGeneral.GetTickCount();
            modTypes.MapNpc[mapNum, mapNpcNum].HP = 0;
            modServerTCP.SendDataToMap(mapNum, "NPCDEAD" + modTypes.SEP_CHAR + mapNpcNum + modTypes.SEP_CHAR);

            // Check for level up
            CheckPlayerLevelUp(attacker);

            // Check for level up party member
            if (modTypes.Player[attacker].InParty == modTypes.YES)
            {
                CheckPlayerLevelUp(modTypes.Player[attacker].PartyPlayer);
            }

            // Check if target is npc that died and if so set target to 0
            if (modTypes.Player[attacker].TargetType == modTypes.TARGET_TYPE_NPC && modTypes.Player[attacker].Target == mapNpcNum)
            {
                modTypes.Player[attacker].Target = 0;
                modTypes.Player[attacker].TargetType = 0;
            }
        }
        else
        {
            // NPC not dead, just do the damage
            modTypes.MapNpc[mapNum, mapNpcNum].HP -= damage;

            // Check for a weapon and say damage
            if (n == 0)
            {
                modServerTCP.PlayerMsg(attacker, $"You hit a {name} for {damage} hit points.", modText.White);
            }
            else
            {
                modServerTCP.PlayerMsg(attacker, $"You hit a {name} with a {modTypes.Item[n].Name.Trim()} for {damage} hit points.", modText.White);
            }

            // Check if we should send a message
            if (modTypes.MapNpc[mapNum, mapNpcNum].Target == 0 && modTypes.MapNpc[mapNum, mapNpcNum].Target != attacker)
            {
                if (!string.IsNullOrWhiteSpace(modTypes.Npc[npcNum].AttackSay))
                {
                    modServerTCP.PlayerMsg(attacker, $"A {modTypes.Npc[npcNum].Name.Trim()} says, '{modTypes.Npc[npcNum].AttackSay.Trim()}' to you.", modText.SayColor);
                }
            }

            // Set the NPC target to the player
            modTypes.MapNpc[mapNum, mapNpcNum].Target = attacker;

            // Now check for guard ai and if so have all onmap guards come after'm
            if (modTypes.Npc[modTypes.MapNpc[mapNum, mapNpcNum].Num].Behavior == modTypes.NPC_BEHAVIOR_GUARD)
            {
                for (var i = 1; i <= modTypes.MAX_MAP_NPCS; i++)
                {
                    if (modTypes.MapNpc[mapNum, i].Num == modTypes.MapNpc[mapNum, mapNpcNum].Num)
                    {
                        modTypes.MapNpc[mapNum, i].Target = attacker;
                    }
                }
            }
        }

        // Reset attack timer
        modTypes.Player[attacker].AttackTimer = modGeneral.GetTickCount();
    }

    public static void PlayerWarp(int index, int mapNum, int x, int y)
    {
        // Check for subscript out of range
        if (!modServerTCP.IsPlaying(index) || mapNum <= 0 || mapNum > modTypes.MAX_MAPS)
        {
            return;
        }

        //  Check if there was an npc on the map the player is leaving, and if so say goodbye
        var shopNum = modTypes.Map[mapNum].Shop;
        if (shopNum > 0)
        {
            if (!string.IsNullOrWhiteSpace(modTypes.Shop[shopNum].LeaveSay))
            {
                modServerTCP.PlayerMsg(index, $"{modTypes.Shop[shopNum].Name} says, '{modTypes.Shop[shopNum].LeaveSay}'", modText.SayColor);
            }
        }

        // Save old map to send erase player data to
        var oldMap = modTypes.GetPlayerMap(index);
        modServerTCP.SendLeaveMap(index, oldMap);

        modTypes.SetPlayerMap(index, mapNum);
        modTypes.SetPlayerX(index, x);
        modTypes.SetPlayerY(index, y);

        // Check if there is an npc on the map and say hello if so
        shopNum = modTypes.Map[mapNum].Shop;
        if (shopNum > 0)
        {
            if (!string.IsNullOrWhiteSpace(modTypes.Shop[shopNum].JoinSay))
            {
                modServerTCP.PlayerMsg(index, $"{modTypes.Shop[shopNum].Name} says, '{modTypes.Shop[shopNum].JoinSay}'", modText.SayColor);
            }
        }

        // Now we check if there were any players left on the map the player just left, and if not stop processing npcs
        if (GetTotalMapPlayers(oldMap) == 0)
        {
            modTypes.PlayersOnMap[oldMap] = modTypes.NO;
        }

        modTypes.PlayersOnMap[mapNum] = modTypes.YES;

        modTypes.Player[index].GettingMap = modTypes.YES;
        modServerTCP.SendDataTo(index,
            "CHECKFORMAP" +
            modTypes.SEP_CHAR + mapNum +
            modTypes.SEP_CHAR + modTypes.Map[mapNum].Revision +
            modTypes.SEP_CHAR);
    }

    private static bool TryPlayerMove(int index, int x, int y, int movement)
    {
        var mapNum = modTypes.GetPlayerMap(index);

        // Check to make sure not outside of boundries
        if (x is >= 0 and <= modTypes.MAX_MAPX && y is >= 0 and <= modTypes.MAX_MAPY)
        {
            // Check to make sure that the tile is walkable
            switch (modTypes.Map[mapNum].Tile[x, y].Type)
            {
                case modTypes.TILE_TYPE_BLOCKED:
                case modTypes.TILE_TYPE_KEY when modTypes.TempTile[mapNum].DoorOpen[x, y - 1] != modTypes.YES:
                    return false;
            }

            modTypes.SetPlayerX(index, x);
            modTypes.SetPlayerY(index, y);

            var packet = "PLAYERMOVE" +
                         modTypes.SEP_CHAR + index +
                         modTypes.SEP_CHAR + x +
                         modTypes.SEP_CHAR + y +
                         modTypes.SEP_CHAR + modTypes.GetPlayerDir(index) +
                         modTypes.SEP_CHAR + movement +
                         modTypes.SEP_CHAR;

            modServerTCP.SendDataToMapBut(index, mapNum, packet);
            return true;
        }

        // Check to see if we can move them to the another map
        var targetMapNum = modTypes.Map[mapNum].Up;
        if (targetMapNum <= 0)
        {
            return false;
        }

        PlayerWarp(index, targetMapNum, modTypes.GetPlayerX(index), modTypes.MAX_MAPY);
        return true;
    }

    public static void PlayerMove(int index, int dir, int movement)
    {
        // Check for subscript out of range
        if (!modServerTCP.IsPlaying(index) || dir < modTypes.DIR_UP || dir > modTypes.DIR_RIGHT || movement < 1 || movement > 2)
        {
            return;
        }

        modTypes.SetPlayerDir(index, dir);

        var moved = false;
        var x = modTypes.GetPlayerX(index);
        var y = modTypes.GetPlayerY(index);

        switch (dir)
        {
            case modTypes.DIR_UP:
                moved = TryPlayerMove(index, x, y - 1, movement);
                break;
            case modTypes.DIR_DOWN:
                moved = TryPlayerMove(index, x, y + 1, movement);
                break;
            case modTypes.DIR_LEFT:
                moved = TryPlayerMove(index, x - 1, y, movement);
                break;
            case modTypes.DIR_RIGHT:
                moved = TryPlayerMove(index, x + 1, y, movement);
                break;
        }

        var mapNum = modTypes.GetPlayerMap(index);
        x = modTypes.GetPlayerX(index);
        y = modTypes.GetPlayerY(index);

        // Check to see if the tile is a warp tile, and if so warp them
        if (modTypes.Map[mapNum].Tile[x, y].Type == modTypes.TILE_TYPE_WARP)
        {
            mapNum = modTypes.Map[mapNum].Tile[x, y].Data1;
            x = modTypes.Map[mapNum].Tile[x, y].Data2;
            y = modTypes.Map[mapNum].Tile[x, y].Data3;

            PlayerWarp(index, mapNum, x, y);
            moved = true;
        }

        // Check for key trigger open
        if (modTypes.Map[mapNum].Tile[x, y].Type == modTypes.TILE_TYPE_KEYOPEN)
        {
            x = modTypes.Map[mapNum].Tile[x, y].Data1;
            y = modTypes.Map[mapNum].Tile[x, y].Data2;

            if (modTypes.Map[mapNum].Tile[x, y].Type == modTypes.TILE_TYPE_KEY && modTypes.TempTile[mapNum].DoorOpen[x, y] == modTypes.NO)
            {
                modTypes.TempTile[mapNum].DoorOpen[x, y] = modTypes.YES;
                modTypes.TempTile[mapNum].DoorTimer = modGeneral.GetTickCount();

                modServerTCP.SendDataToMap(mapNum,
                    "MAPKEY" +
                    modTypes.SEP_CHAR + x +
                    modTypes.SEP_CHAR + y +
                    modTypes.SEP_CHAR + 1 +
                    modTypes.SEP_CHAR);

                modServerTCP.MapMsg(mapNum, "A door has been unlocked.", modText.White);
            }
        }

        // They tried to hack
        if (!moved)
        {
            modServerTCP.HackingAttempt(index, "Position Modification");
        }
    }

    private static bool IsPassable(int mapNum, int mapNpcNum, int x, int y)
    {
        // Check to make sure not outside of boundries
        if (x < 0 || y < 0 || x > modTypes.MAX_MAPX || y > modTypes.MAX_MAPY)
        {
            return false;
        }

        var n = modTypes.Map[mapNum].Tile[x, y].Type;

        // Check to make sure that the tile is walkable
        if (n != modTypes.TILE_TYPE_WALKABLE && n != modTypes.TILE_TYPE_ITEM)
        {
            return false;
        }

        // Check to make sure that there is not a player in the way
        for (var i = 1; i <= modTypes.MAX_PLAYERS; i++)
        {
            if (!modServerTCP.IsPlaying(i))
            {
                continue;
            }

            if (modTypes.GetPlayerMap(i) == mapNum && modTypes.GetPlayerX(i) == x && modTypes.GetPlayerY(i) == y)
            {
                return false;
            }
        }

        // Check to make sure that there is not another npc in the way
        for (var i = 1; i <= modTypes.MAX_MAP_NPCS; i++)
        {
            if (i != mapNpcNum && modTypes.MapNpc[mapNum, i].X == x && modTypes.MapNpc[mapNum, i].Y == y)
            {
                return false;
            }
        }

        return true;
    }

    public static bool CanNpcMove(int mapNum, int mapNpcNum, int dir)
    {
        if (mapNum <= 0 || mapNum > modTypes.MAX_MAPS || mapNpcNum <= 0 || mapNpcNum > modTypes.MAX_MAP_NPCS || dir < modTypes.DIR_UP || dir > modTypes.DIR_RIGHT)
        {
            return false;
        }

        var x = modTypes.MapNpc[mapNum, mapNpcNum].X;
        var y = modTypes.MapNpc[mapNum, mapNpcNum].Y;

        return dir switch
        {
            modTypes.DIR_UP => IsPassable(mapNum, mapNpcNum, x, y - 1),
            modTypes.DIR_DOWN => IsPassable(mapNum, mapNpcNum, x, y + 1),
            modTypes.DIR_LEFT => IsPassable(mapNum, mapNpcNum, x - 1, y),
            modTypes.DIR_RIGHT => IsPassable(mapNum, mapNpcNum, x + 1, y),
            _ => false
        };
    }

    public static void NpcMove(int mapNum, int mapNpcNum, int dir, int movement)
    {
        if (mapNum <= 0 || mapNum > modTypes.MAX_MAPS || mapNpcNum <= 0 || mapNpcNum > modTypes.MAX_MAP_NPCS || dir < modTypes.DIR_UP || dir > modTypes.DIR_RIGHT || movement < 1 || movement > 2)
        {
            return;
        }

        modTypes.MapNpc[mapNum, mapNpcNum].Dir = dir;

        switch (dir)
        {
            case modTypes.DIR_UP:
                modTypes.MapNpc[mapNum, mapNpcNum].Y -= 1;
                modServerTCP.SendDataToMap(mapNum,
                    "NPCMOVE" +
                    modTypes.SEP_CHAR + mapNpcNum +
                    modTypes.SEP_CHAR + modTypes.MapNpc[mapNum, mapNpcNum].X +
                    modTypes.SEP_CHAR + modTypes.MapNpc[mapNum, mapNpcNum].Y +
                    modTypes.SEP_CHAR + modTypes.MapNpc[mapNum, mapNpcNum].Dir +
                    modTypes.SEP_CHAR + movement +
                    modTypes.SEP_CHAR);
                break;

            case modTypes.DIR_DOWN:
                modTypes.MapNpc[mapNum, mapNpcNum].Y += 1;
                modServerTCP.SendDataToMap(mapNum,
                    "NPCMOVE" +
                    modTypes.SEP_CHAR + mapNpcNum +
                    modTypes.SEP_CHAR + modTypes.MapNpc[mapNum, mapNpcNum].X +
                    modTypes.SEP_CHAR + modTypes.MapNpc[mapNum, mapNpcNum].Y +
                    modTypes.SEP_CHAR + modTypes.MapNpc[mapNum, mapNpcNum].Dir +
                    modTypes.SEP_CHAR + movement +
                    modTypes.SEP_CHAR);
                break;

            case modTypes.DIR_LEFT:
                modTypes.MapNpc[mapNum, mapNpcNum].X -= 1;
                modServerTCP.SendDataToMap(mapNum,
                    "NPCMOVE" +
                    modTypes.SEP_CHAR + mapNpcNum +
                    modTypes.SEP_CHAR + modTypes.MapNpc[mapNum, mapNpcNum].X +
                    modTypes.SEP_CHAR + modTypes.MapNpc[mapNum, mapNpcNum].Y +
                    modTypes.SEP_CHAR + modTypes.MapNpc[mapNum, mapNpcNum].Dir +
                    modTypes.SEP_CHAR + movement +
                    modTypes.SEP_CHAR);
                break;

            case modTypes.DIR_RIGHT:
                modTypes.MapNpc[mapNum, mapNpcNum].X += 1;
                modServerTCP.SendDataToMap(mapNum,
                    "NPCMOVE" +
                    modTypes.SEP_CHAR + mapNpcNum +
                    modTypes.SEP_CHAR + modTypes.MapNpc[mapNum, mapNpcNum].X +
                    modTypes.SEP_CHAR + modTypes.MapNpc[mapNum, mapNpcNum].Y +
                    modTypes.SEP_CHAR + modTypes.MapNpc[mapNum, mapNpcNum].Dir +
                    modTypes.SEP_CHAR + movement +
                    modTypes.SEP_CHAR);
                break;
        }
    }

    public static void NpcDir(int mapNum, int mapNpcNum, int dir)
    {
        // Check for subscript out of range
        if (mapNum <= 0 || mapNum > modTypes.MAX_MAPS || mapNpcNum <= 0 || mapNpcNum > modTypes.MAX_MAPS || dir < modTypes.DIR_UP || dir > modTypes.DIR_RIGHT)
        {
            return;
        }

        modTypes.MapNpc[mapNum, mapNpcNum].Dir = dir;
        var packet = "NPCDIR" + modTypes.SEP_CHAR + mapNpcNum + modTypes.SEP_CHAR + dir + modTypes.SEP_CHAR;
        modServerTCP.SendDataToMap(mapNum, packet);
    }

    public static void JoinGame(int index)
    {
        // Set the flag so we know the person is in the game
        modTypes.Player[index].InGame = true;

        //  Send a global message that he/she joined
        if (modTypes.GetPlayerAccess(index) <= modTypes.ADMIN_MONITER)
        {
            modServerTCP.GlobalMsg($"{modTypes.GetPlayerName(index)} has joined {modTypes.GAME_NAME}!", modText.JoinLeftColor);
        }
        else
        {
            modServerTCP.GlobalMsg($"{modTypes.GetPlayerName(index)} has joined {modTypes.GAME_NAME}!", modText.White);
        }

        // Send an ok to client to start receiving in game data
        modServerTCP.SendDataTo(index, "LOGINOK" + modTypes.SEP_CHAR + index + modTypes.SEP_CHAR);

        // Send some more little goodies, no need to explain these
        CheckEquippedItems(index);
        modServerTCP.SendClasses(index);
        modServerTCP.SendItems(index);
        modServerTCP.SendNpcs(index);
        modServerTCP.SendShops(index);
        modServerTCP.SendSpells(index);
        modServerTCP.SendInventory(index);
        modServerTCP.SendWornEquipment(index);
        modServerTCP.SendHP(index);
        modServerTCP.SendMP(index);
        modServerTCP.SendSP(index);
        modServerTCP.SendStats(index);
        modServerTCP.SendWeatherTo(index);
        modServerTCP.SendTimeTo(index);

        // Warp the player to his saved location
        PlayerWarp(index, modTypes.GetPlayerMap(index), modTypes.GetPlayerX(index), modTypes.GetPlayerY(index));

        // Send welcome messages
        modServerTCP.SendWelcome(index);

        // Send the flag so they know they can start doing stuff
        modServerTCP.SendDataTo(index, "INGAME" + modTypes.SEP_CHAR);
    }

    public static void LeftGame(int index)
    {
        if (modTypes.Player[index].InGame)
        {
            modTypes.Player[index].InGame = false;

            var mapNum = modTypes.GetPlayerMap(index);

            // Check if player was the only player on the map and stop npc processing if so
            if (GetTotalMapPlayers(mapNum) == 1)
            {
                modTypes.PlayersOnMap[mapNum] = modTypes.NO;
            }

            // Check for boot map
            if (modTypes.Map[mapNum].BootMap > 0)
            {
                modTypes.SetPlayerX(index, modTypes.Map[mapNum].BootX);
                modTypes.SetPlayerY(index, modTypes.Map[mapNum].BootY);
                modTypes.SetPlayerMap(index, modTypes.Map[mapNum].BootMap);
            }

            // Check if the player was in a party, and if so cancel it out so the other player doesn't continue to get half exp
            if (modTypes.Player[index].InParty == modTypes.YES)
            {
                var n = modTypes.Player[index].PartyPlayer;

                modServerTCP.PlayerMsg(n, $"{modTypes.GetPlayerName(index)} has left {modTypes.GAME_NAME}, disbanning party.", modText.Pink);

                modTypes.Player[n].InParty = modTypes.NO;
                modTypes.Player[n].PartyPlayer = 0;
            }

            modDatabase.SavePlayer(index);

            // Send a global message that he/she left
            if (modTypes.GetPlayerAccess(index) <= modTypes.ADMIN_MONITER)
            {
                modServerTCP.GlobalMsg($"{modTypes.GetPlayerName(index)} has left {modTypes.GAME_NAME}!", modText.JoinLeftColor);
            }
            else
            {
                modServerTCP.GlobalMsg($"{modTypes.GetPlayerName(index)} has left {modTypes.GAME_NAME}!", modText.White);
            }

            Console.WriteLine($"{modTypes.GetPlayerName(index)} has disconnected from {modTypes.GAME_NAME}.");
            modServerTCP.SendLeftGame(index);
        }

        modTypes.ClearPlayer(index);
    }

    public static int GetTotalMapPlayers(int mapNum)
    {
        var n = 0;

        for (var i = 1; i <= modTypes.MAX_PLAYERS; i++)
        {
            if (modServerTCP.IsPlaying(i) && modTypes.GetPlayerMap(i) == mapNum)
            {
                n++;
            }
        }

        return n;
    }

    public static int GetNpcMaxHP(int npcNum)
    {
        if (npcNum is <= 0 or > modTypes.MAX_NPCS)
        {
            return 0;
        }

        var x = modTypes.Npc[npcNum].STR;
        var y = modTypes.Npc[npcNum].DEF;

        return x * y;
    }

    public static int GetNpcMaxMP(int npcNum)
    {
        if (npcNum is <= 0 or > modTypes.MAX_NPCS)
        {
            return 0;
        }

        return modTypes.Npc[npcNum].MAGI * 2;
    }

    public static int GetNpcMaxSP(int npcNum)
    {
        if (npcNum is <= 0 or > modTypes.MAX_NPCS)
        {
            return 0;
        }

        return modTypes.Npc[npcNum].SPEED * 2;
    }

    public static int GetPlayerHPRegen(int index)
    {
        if (index <= 0 || index > modTypes.MAX_PLAYERS || !modServerTCP.IsPlaying(index))
        {
            return 0;
        }

        var i = modTypes.GetPlayerDEF(index) / 2;
        if (i < 2) i = 2;

        return i;
    }

    public static int GetPlayerMPRegen(int index)
    {
        if (index <= 0 || index > modTypes.MAX_PLAYERS || !modServerTCP.IsPlaying(index))
        {
            return 0;
        }

        var i = modTypes.GetPlayerMAGI(index) / 2;
        if (i < 2) i = 2;

        return i;
    }

    public static int GetPlayerSPRegen(int index)
    {
        if (index <= 0 || index > modTypes.MAX_PLAYERS || !modServerTCP.IsPlaying(index))
        {
            return 0;
        }

        var i = modTypes.GetPlayerSPEED(index) / 2;
        if (i < 2) i = 2;

        return i;
    }

    public static int GetNpcHPRegen(int npcNum)
    {
        // Prevent subscript out of range
        if (npcNum is <= 0 or > modTypes.MAX_NPCS)
        {
            return 0;
        }

        var i = modTypes.Npc[npcNum].DEF / 3;
        if (i < 1) i = 1;

        return i;
    }

    public static void CheckPlayerLevelUp(int index)
    {
        if (modTypes.GetPlayerExp(index) >= modTypes.GetPlayerNextLevel(index))
        {
            modTypes.SetPlayerLevel(index, modTypes.GetPlayerLevel(index) + 1);

            var i = modTypes.GetPlayerSPEED(index) / 10;
            if (i < 1) i = 1;
            if (i > 3) i = 3;

            modTypes.SetPlayerPOINTS(index, modTypes.GetPlayerPOINTS(index) + i);
            modTypes.SetPlayerExp(index, 0);
            modServerTCP.GlobalMsg($"{modTypes.GetPlayerName(index)} has reached level {modTypes.GetPlayerLevel(index)}!", modText.Brown);
            modServerTCP.PlayerMsg(index, $"You have gained a level!  You now have {modTypes.GetPlayerPOINTS(index)} stat points to distribute.", modText.BrightBlue);
        }
    }

    public static void CastSpell(int index, int spellSlot)
    {
        // Prevent subscript out of range
        if (spellSlot is <= 0 or > modTypes.MAX_PLAYER_SPELLS)
        {
            return;
        }

        var spellNum = modTypes.GetPlayerSpell(index, spellSlot);

        // Make sure player has the spell
        if (!HasSpell(index, spellNum))
        {
            modServerTCP.PlayerMsg(index, "You do not have this spell!", modText.BrightRed);
            return;
        }

        var i = GetSpellReqLevel(index, spellNum);
        var mpReq = i + modTypes.Spell[spellNum].Data1 + modTypes.Spell[spellNum].Data2 + modTypes.Spell[spellNum].Data3;

        // Check if they have enough MP
        if (modTypes.GetPlayerMP(index) < mpReq)
        {
            modServerTCP.PlayerMsg(index, "Not enough mana points!", modText.BrightRed);
            return;
        }

        // Make sure they are the right level
        if (i > modTypes.GetPlayerLevel(index))
        {
            modServerTCP.PlayerMsg(index, $"You must be level {i} to cast this spell.", modText.BrightRed);
            return;
        }

        // Check if timer is ok
        if (modGeneral.GetTickCount() < modTypes.Player[index].AttackTimer + 1000)
        {
            return;
        }

        // Check if the spell is a give item and do that instead of a stat modification
        if (modTypes.Spell[spellNum].Type == modTypes.SPELL_TYPE_GIVEITEM)
        {
            var invSlot = FindOpenInvSlot(index, modTypes.Spell[spellNum].Data1);
            if (invSlot > 0)
            {
                GiveItem(index, modTypes.Spell[spellNum].Data1, modTypes.Spell[spellNum].Data2);
                modServerTCP.MapMsg(modTypes.GetPlayerMap(index), $"{modTypes.GetPlayerName(index)} casts {modTypes.Spell[spellNum].Name.Trim()}.", modText.BrightBlue);

                // Take away the mana points
                modTypes.SetPlayerMP(index, modTypes.GetPlayerMP(index) - mpReq);
                modServerTCP.SendMP(index);
                goto L_Casted;
            }

            modServerTCP.PlayerMsg(index, "Your inventory is full!", modText.BrightRed);
            return;
        }

        var n = modTypes.Player[index].Target;
        if (modTypes.Player[index].TargetType == modTypes.TARGET_TYPE_PLAYER)
        {
            if (modServerTCP.IsPlaying(n))
            {
                if (modTypes.GetPlayerHP(n) > 0 &&
                    modTypes.GetPlayerMap(n) == modTypes.GetPlayerMap(index) &&
                    modTypes.GetPlayerLevel(index) >= 10 &&
                    modTypes.GetPlayerLevel(n) >= 10 &&
                    modTypes.Map[modTypes.GetPlayerMap(index)].Moral == modTypes.MAP_MORAL_NONE &&
                    modTypes.GetPlayerAccess(index) <= 0 &&
                    modTypes.GetPlayerAccess(n) <= 0)
                {
                    modServerTCP.MapMsg(modTypes.GetPlayerMap(index), $"{modTypes.GetPlayerName(index)} casts {modTypes.Spell[spellNum].Name.Trim()} on {modTypes.GetPlayerName(n)}.", modText.BrightBlue);

                    switch (modTypes.Spell[spellNum].Type)
                    {
                        case modTypes.SPELL_TYPE_SUBHP:
                            var damage = modTypes.GetPlayerMAGI(index) / 4 + modTypes.Spell[spellNum].Data1 - GetPlayerProtection(n);
                            if (damage > 0)
                            {
                                AttackPlayer(index, n, damage);
                            }
                            else
                            {
                                modServerTCP.PlayerMsg(index, $"The spell was to weak to hurt {modTypes.GetPlayerName(n)}!", modText.BrightRed);
                            }

                            break;

                        case modTypes.SPELL_TYPE_SUBMP:
                            modTypes.SetPlayerMP(n, modTypes.GetPlayerMP(n) - modTypes.Spell[spellNum].Data1);
                            modServerTCP.SendMP(n);
                            break;

                        case modTypes.SPELL_TYPE_SUBSP:
                            modTypes.SetPlayerSP(n, modTypes.GetPlayerSP(n) - modTypes.Spell[spellNum].Data1);
                            modServerTCP.SendSP(n);
                            break;
                    }

                    // Take away the mana points
                    modTypes.SetPlayerMP(index, modTypes.GetPlayerMP(index) - mpReq);
                    modServerTCP.SendMP(index);
                    goto L_Casted;
                }

                if (modTypes.GetPlayerMap(index) == modTypes.GetPlayerMap(n) && modTypes.Spell[spellNum].Type >= modTypes.SPELL_TYPE_ADDHP && modTypes.Spell[spellNum].Type <= modTypes.SPELL_TYPE_ADDSP)
                {
                    switch (modTypes.Spell[spellNum].Type)
                    {
                        case modTypes.SPELL_TYPE_SUBHP:
                            modServerTCP.MapMsg(modTypes.GetPlayerMap(index), $"{modTypes.GetPlayerName(index)} casts {modTypes.Spell[spellNum].Name.Trim()} on {modTypes.GetPlayerName(n)}.", modText.BrightBlue);
                            modTypes.SetPlayerHP(n, modTypes.GetPlayerHP(n) + modTypes.Spell[spellNum].Data1);
                            modServerTCP.SendHP(n);
                            break;

                        case modTypes.SPELL_TYPE_SUBMP:
                            modServerTCP.MapMsg(modTypes.GetPlayerMap(index), $"{modTypes.GetPlayerName(index)} casts {modTypes.Spell[spellNum].Name.Trim()} on {modTypes.GetPlayerName(n)}.", modText.BrightBlue);
                            modTypes.SetPlayerMP(n, modTypes.GetPlayerMP(n) + modTypes.Spell[spellNum].Data1);
                            modServerTCP.SendMP(n);
                            break;

                        case modTypes.SPELL_TYPE_SUBSP:
                            modServerTCP.MapMsg(modTypes.GetPlayerMap(index), $"{modTypes.GetPlayerName(index)} casts {modTypes.Spell[spellNum].Name.Trim()} on {modTypes.GetPlayerName(n)}.", modText.BrightBlue);
                            modTypes.SetPlayerMP(n, modTypes.GetPlayerSP(n) + modTypes.Spell[spellNum].Data1);
                            modServerTCP.SendMP(n);
                            break;
                    }

                    // Take away the mana points
                    modTypes.SetPlayerMP(index, modTypes.GetPlayerMP(index) - mpReq);
                    modServerTCP.SendMP(index);
                    goto L_Casted;
                }

                modServerTCP.PlayerMsg(index, "Could not cast spell!", modText.BrightRed);
                return;
            }

            modServerTCP.PlayerMsg(index, "Could not cast spell!", modText.BrightRed);
            return;
        }

        var npcNum = modTypes.MapNpc[modTypes.GetPlayerMap(index), n].Num;
        if (modTypes.Npc[npcNum].Behavior != modTypes.NPC_BEHAVIOR_FRIENDLY && modTypes.Npc[npcNum].Behavior != modTypes.NPC_BEHAVIOR_SHOPKEEPER)
        {
            modServerTCP.MapMsg(modTypes.GetPlayerMap(index), $"{modTypes.GetPlayerName(index)} casts {modTypes.Spell[spellNum].Name.Trim()} on a {modTypes.Npc[npcNum].Name.Trim()}.", modText.BrightBlue);

            switch (modTypes.Spell[spellNum].Type)
            {
                case modTypes.SPELL_TYPE_ADDHP:
                    modTypes.MapNpc[modTypes.GetPlayerMap(index), n].HP = modTypes.MapNpc[modTypes.GetPlayerMap(index), n].HP + modTypes.Spell[spellNum].Data1;
                    break;

                case modTypes.SPELL_TYPE_SUBHP:
                    var damage = (modTypes.GetPlayerMAGI(index) / 4 + modTypes.Spell[spellNum].Data1) - (modTypes.Npc[npcNum].DEF / 2);
                    if (damage > 0)
                    {
                        AttackNpc(index, n, damage);
                    }
                    else
                    {
                        modServerTCP.PlayerMsg(index, $"The spell was to weak to hurt {modTypes.Npc[npcNum].Name.Trim()}!", modText.BrightRed);
                    }

                    break;

                case modTypes.SPELL_TYPE_ADDMP:
                    modTypes.MapNpc[modTypes.GetPlayerMap(index), n].MP = modTypes.MapNpc[modTypes.GetPlayerMap(index), n].MP + modTypes.Spell[spellNum].Data1;
                    break;

                case modTypes.SPELL_TYPE_SUBMP:
                    modTypes.MapNpc[modTypes.GetPlayerMap(index), n].MP = modTypes.MapNpc[modTypes.GetPlayerMap(index), n].MP - modTypes.Spell[spellNum].Data1;
                    break;

                case modTypes.SPELL_TYPE_ADDSP:
                    modTypes.MapNpc[modTypes.GetPlayerMap(index), n].SP = modTypes.MapNpc[modTypes.GetPlayerMap(index), n].SP + modTypes.Spell[spellNum].Data1;
                    break;

                case modTypes.SPELL_TYPE_SUBSP:
                    modTypes.MapNpc[modTypes.GetPlayerMap(index), n].SP = modTypes.MapNpc[modTypes.GetPlayerMap(index), n].SP - modTypes.Spell[spellNum].Data1;
                    break;
            }

            // Take away the mana points
            modTypes.SetPlayerMP(index, modTypes.GetPlayerMP(index) - mpReq);
            modServerTCP.SendMP(index);
            goto L_Casted;
        }

        modServerTCP.PlayerMsg(index, "Could not cast spell!", modText.BrightRed);
        return;

        L_Casted:
        modTypes.Player[index].AttackTimer = modGeneral.GetTickCount();
        modTypes.Player[index].CastedSpell = modTypes.YES;
    }

    public static int GetSpellReqLevel(int index, int spellNum)
    {
        return modTypes.Spell[spellNum].Data1 + modTypes.GetClassMAGI(modTypes.GetPlayerClass(index)) / 4;
    }

    public static bool CanPlayerCriticalHit(int index)
    {
        if (modTypes.GetPlayerWeaponSlot(index) > 0)
        {
            var n = Random.Shared.Next(0, 2);
            if (n == 1)
            {
                var i = modTypes.GetPlayerSTR(index) / 2 + modTypes.GetPlayerLevel(index) / 2;

                n = Random.Shared.Next(0, 100) + 1;
                if (n <= i)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static bool CanPlayerBlockHit(int index)
    {
        if (modTypes.GetPlayerShieldSlot(index) > 0)
        {
            var n = Random.Shared.Next(0, 2);
            if (n == 1)
            {
                var i = modTypes.GetPlayerDEF(index) / 2 + modTypes.GetPlayerLevel(index) / 2;

                n = Random.Shared.Next(1, 100);
                if (n <= i)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static void CheckEquippedItems(int index)
    {
        // We want to check incase an admin takes away an object but they had it equipped
        var slot = modTypes.GetPlayerWeaponSlot(index);
        if (slot > 0)
        {
            var itemNum = modTypes.GetPlayerInvItemNum(index, slot);

            if (itemNum > 0)
            {
                if (modTypes.Item[itemNum].Type != modTypes.ITEM_TYPE_WEAPON)
                {
                    modTypes.SetPlayerWeaponSlot(index, 0);
                }
            }
            else
            {
                modTypes.SetPlayerWeaponSlot(index, 0);
            }
        }

        slot = modTypes.GetPlayerArmorSlot(index);
        if (slot > 0)
        {
            var itemNum = modTypes.GetPlayerInvItemNum(index, slot);

            if (itemNum > 0)
            {
                if (modTypes.Item[itemNum].Type != modTypes.ITEM_TYPE_ARMOR)
                {
                    modTypes.SetPlayerArmorSlot(index, 0);
                }
            }
            else
            {
                modTypes.SetPlayerArmorSlot(index, 0);
            }
        }

        slot = modTypes.GetPlayerHelmetSlot(index);
        if (slot > 0)
        {
            var itemNum = modTypes.GetPlayerInvItemNum(index, slot);

            if (itemNum > 0)
            {
                if (modTypes.Item[itemNum].Type != modTypes.ITEM_TYPE_HELMET)
                {
                    modTypes.SetPlayerHelmetSlot(index, 0);
                }
            }
            else
            {
                modTypes.SetPlayerHelmetSlot(index, 0);
            }
        }

        slot = modTypes.GetPlayerShieldSlot(index);
        if (slot > 0)
        {
            var itemNum = modTypes.GetPlayerInvItemNum(index, slot);

            if (itemNum > 0)
            {
                if (modTypes.Item[itemNum].Type != modTypes.ITEM_TYPE_SHIELD)
                {
                    modTypes.SetPlayerShieldSlot(index, 0);
                }
            }
            else
            {
                modTypes.SetPlayerShieldSlot(index, 0);
            }
        }
    }
}