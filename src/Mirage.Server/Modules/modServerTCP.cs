using System.Text;

namespace Mirage.Modules;

public static class modServerTCP
{
    public static bool IsPlaying(int index)
    {
        return modNetwork.IsConnected(index) && modTypes.Player[index].InGame;
    }

    public static bool IsLoggedIn(int index)
    {
        return modNetwork.IsConnected(index) && modTypes.Player[index].Login != "";
    }

    public static bool IsMultiAccounts(string login)
    {
        for (var i = 1; i <= modTypes.MAX_PLAYERS; i++)
        {
            if (modNetwork.IsConnected(i) && modTypes.Player[i].Login.Equals(login, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsMultiIPOnline(string ip)
    {
        var n = 0;
        for (var i = 1; i <= modTypes.MAX_PLAYERS; i++)
        {
            if (modNetwork.IsConnected(i) && modTypes.GetPlayerIP(i).Equals(ip, StringComparison.CurrentCultureIgnoreCase))
            {
                n++;
                if (n > 1)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static bool IsBanned(string ip)
    {
        const string filename = "Banlist.txt";

        if (!File.Exists(filename))
        {
            return false;
        }

        using var streamReader = File.OpenText(filename);
        while (!streamReader.EndOfStream)
        {
            var ipToCheck = streamReader.ReadLine();
            if (ipToCheck is null)
            {
                continue;
            }

            var comma = ipToCheck.IndexOf(',');
            if (comma != -1)
            {
                ipToCheck = ipToCheck[..comma];
            }

            if (ipToCheck.Length >= ip.Length && ipToCheck[..ip.Length].Equals(ip, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    public static void SendDataTo(int index, string data)
    {
        if (!modNetwork.IsConnected(index))
        {
            return;
        }

        var len = Encoding.UTF8.GetByteCount(data);
        var packet = new byte[len + 1];
        Encoding.UTF8.GetBytes(data, packet);
        packet[len] = 237;

        modNetwork.SendData(index, packet);
    }

    public static void SendDataToAll(string data)
    {
        for (var i = 1; i <= modTypes.MAX_PLAYERS; i++)
        {
            if (IsPlaying(i))
            {
                SendDataTo(i, data);
            }
        }
    }

    public static void SendDataToAllBut(int index, string data)
    {
        for (var i = 1; i <= modTypes.MAX_PLAYERS; i++)
        {
            if (i != index && IsPlaying(i))
            {
                SendDataTo(i, data);
            }
        }
    }

    public static void SendDataToMap(int mapNum, string data)
    {
        for (var i = 1; i <= modTypes.MAX_PLAYERS; i++)
        {
            if (IsPlaying(i) && modTypes.GetPlayerMap(i) == mapNum)
            {
                SendDataTo(i, data);
            }
        }
    }

    public static void SendDataToMapBut(int index, int mapNum, string data)
    {
        for (var i = 1; i <= modTypes.MAX_PLAYERS; i++)
        {
            if (i != index && IsPlaying(i) && modTypes.GetPlayerMap(i) == mapNum)
            {
                SendDataTo(i, data);
            }
        }
    }

    public static void GlobalMsg(string msg, int color)
    {
        var packet = "GLOBALMSG" + modTypes.SEP_CHAR + msg + modTypes.SEP_CHAR + color + modTypes.SEP_CHAR;

        SendDataToAll(packet);
    }

    public static void AdminMsg(string msg, int color)
    {
        var packet = "ADMINMSG" + modTypes.SEP_CHAR + msg + modTypes.SEP_CHAR + color + modTypes.SEP_CHAR;
        for (var i = 1; i <= modTypes.MAX_PLAYERS; i++)
        {
            if (IsPlaying(i) && modTypes.GetPlayerAccess(i) > 0)
            {
                SendDataTo(i, packet);
            }
        }
    }

    public static void PlayerMsg(int index, string msg, int color)
    {
        var packet = "PLAYERMSG" + modTypes.SEP_CHAR + msg + modTypes.SEP_CHAR + color + modTypes.SEP_CHAR;

        SendDataTo(index, packet);
    }

    public static void MapMsg(int mapNum, string msg, int color)
    {
        var packet = "MAPMSG" + modTypes.SEP_CHAR + msg + modTypes.SEP_CHAR + color + modTypes.SEP_CHAR;

        SendDataToMap(mapNum, packet);
    }

    public static void AlertMsg(int index, string msg)
    {
        var packet = "ALERTMSG" + modTypes.SEP_CHAR + msg + modTypes.SEP_CHAR;

        SendDataTo(index, packet);
        CloseSocket(index);
    }

    public static void HackingAttempt(int index, string reason)
    {
        if (index > 0)
        {
            if (IsPlaying(index))
            {
                GlobalMsg($"{modTypes.GetPlayerLogin(index)}/{modTypes.GetPlayerName(index)} has been booted for ({reason})", modText.White);
            }

            AlertMsg(index, $"You have lost your connection with {modTypes.GAME_NAME}.");
        }
    }

    public static void SocketConnected(int index)
    {
        if (index != 0)
        {
            if (!IsBanned(modTypes.GetPlayerIP(index)))
            {
                Console.WriteLine($"Received connection from {modTypes.GetPlayerIP(index)}.");
            }
            else
            {
                AlertMsg(index, $"You have been banned from {modTypes.GAME_NAME}, and can no longer play.");
            }
        }
    }

    public static void IncomingData(int index, ReadOnlySpan<byte> bytes)
    {
        var byteCount = bytes.Length;

        ref var player = ref modTypes.Player[index];

        bytes.CopyTo(player.Buffer.AsSpan(player.BufferPos));
        player.BufferPos += bytes.Length;

        var buffer = player.Buffer.AsSpan(0, player.BufferPos);
        while (buffer.Length > 0)
        {
            var end = buffer.IndexOf((byte) 237);
            if (end == -1)
            {
                break;
            }

            var packet = Encoding.UTF8.GetString(buffer[..end]);

            modHandleData.HandleData(index, packet);

            buffer = buffer[(end + 1)..];
        }

        buffer.CopyTo(player.Buffer.AsSpan());
        player.BufferPos = buffer.Length;

        // Check if elapsed time has passed
        modTypes.Player[index].DataBytes += byteCount;
        if (modGeneral.GetTickCount() >= modTypes.Player[index].DataTimer + 1000)
        {
            modTypes.Player[index].DataTimer = modGeneral.GetTickCount();
            modTypes.Player[index].DataBytes = 0;
            modTypes.Player[index].DataPackets = 0;
        }

        // Check for data flooding
        if (modTypes.Player[index].DataBytes > 1000 && modTypes.GetPlayerAccess(index) <= 0)
        {
            HackingAttempt(index, "Data Flooding");
            return;
        }

        // Check for packet flooding
        if (modTypes.Player[index].DataPackets > 25 && modTypes.GetPlayerAccess(index) <= 0)
        {
            HackingAttempt(index, "Packet Flooding");
        }
    }

    public static void CloseSocket(int index)
    {
        // Make sure player was/is playing the game, and if so, save'm.
        if (index < 1)
        {
            return;
        }

        modGameLogic.LeftGame(index);
        Console.WriteLine($"Connection from {modTypes.GetPlayerIP(index)} has been terminated.");

        modNetwork.Close(index);

        modTypes.ClearPlayer(index);
    }

    public static void SendWhosOnline(int index)
    {
        var playerNames = new List<string>();
        for (var i = 1; i <= modTypes.MAX_PLAYERS; i++)
        {
            if (i != index && IsPlaying(i))
            {
                playerNames.Add(modTypes.GetPlayerName(i));
            }
        }

        var str = string.Join(", ", playerNames);

        str = str.Length == 0
            ? "There are no other players online."
            : $"There are {playerNames.Count} other players online: {str}";

        PlayerMsg(index, str, modText.WhoColor);
    }

    public static void SendChars(int index)
    {
        var packet = "ALLCHARS" + modTypes.SEP_CHAR;

        for (var i = 1; i <= modTypes.MAX_CHARS; i++)
        {
            packet +=
                modTypes.Player[index].Char[i].Name.Trim() + modTypes.SEP_CHAR +
                modTypes.Class[modTypes.Player[index].Char[i].Class].Name.Trim() + modTypes.SEP_CHAR +
                modTypes.Player[index].Char[i].Level + modTypes.SEP_CHAR;
        }

        SendDataTo(index, packet);
    }

    public static void SendJoinMap(int index)
    {
        string packet;

        // Send all players on current map to index
        for (var i = 1; i <= modTypes.MAX_PLAYERS; i++)
        {
            if (i != index && IsPlaying(i) && modTypes.GetPlayerMap(i) == modTypes.GetPlayerMap(index))
            {
                packet = "PLAYERDATA" + modTypes.SEP_CHAR + i + modTypes.SEP_CHAR +
                         modTypes.GetPlayerName(i) + modTypes.SEP_CHAR +
                         modTypes.GetPlayerSprite(i) + modTypes.SEP_CHAR +
                         modTypes.GetPlayerMap(i) + modTypes.SEP_CHAR +
                         modTypes.GetPlayerX(i) + modTypes.SEP_CHAR +
                         modTypes.GetPlayerY(i) + modTypes.SEP_CHAR +
                         modTypes.GetPlayerDir(i) + modTypes.SEP_CHAR +
                         modTypes.GetPlayerAccess(i) + modTypes.SEP_CHAR +
                         modTypes.GetPlayerPK(i) + modTypes.SEP_CHAR;

                SendDataTo(index, packet);
            }
        }

        // Send index's player data to everyone on the map including himself
        packet = "PLAYERDATA" + modTypes.SEP_CHAR + index + modTypes.SEP_CHAR +
                 modTypes.GetPlayerName(index) + modTypes.SEP_CHAR +
                 modTypes.GetPlayerSprite(index) + modTypes.SEP_CHAR +
                 modTypes.GetPlayerMap(index) + modTypes.SEP_CHAR +
                 modTypes.GetPlayerX(index) + modTypes.SEP_CHAR +
                 modTypes.GetPlayerY(index) + modTypes.SEP_CHAR +
                 modTypes.GetPlayerDir(index) + modTypes.SEP_CHAR +
                 modTypes.GetPlayerAccess(index) + modTypes.SEP_CHAR +
                 modTypes.GetPlayerPK(index) + modTypes.SEP_CHAR;

        SendDataToMap(modTypes.GetPlayerMap(index), packet);
    }

    public static void SendLeaveMap(int index, int mapNum)
    {
        var packet =
            "PLAYERDATA" + modTypes.SEP_CHAR + index + modTypes.SEP_CHAR +
            modTypes.GetPlayerName(index) + modTypes.SEP_CHAR +
            modTypes.GetPlayerSprite(index) + modTypes.SEP_CHAR +
            0 + modTypes.SEP_CHAR +
            modTypes.GetPlayerX(index) + modTypes.SEP_CHAR +
            modTypes.GetPlayerY(index) + modTypes.SEP_CHAR +
            modTypes.GetPlayerDir(index) + modTypes.SEP_CHAR +
            modTypes.GetPlayerAccess(index) + modTypes.SEP_CHAR +
            modTypes.GetPlayerPK(index) + modTypes.SEP_CHAR;

        SendDataToMapBut(index, mapNum, packet);
    }

    public static void SendPlayerData(int index)
    {
        var packet =
            "PLAYERDATA" + modTypes.SEP_CHAR + index + modTypes.SEP_CHAR +
            modTypes.GetPlayerName(index) + modTypes.SEP_CHAR +
            modTypes.GetPlayerSprite(index) + modTypes.SEP_CHAR +
            modTypes.GetPlayerMap(index) + modTypes.SEP_CHAR +
            modTypes.GetPlayerX(index) + modTypes.SEP_CHAR +
            modTypes.GetPlayerY(index) + modTypes.SEP_CHAR +
            modTypes.GetPlayerDir(index) + modTypes.SEP_CHAR +
            modTypes.GetPlayerAccess(index) + modTypes.SEP_CHAR +
            modTypes.GetPlayerPK(index) + modTypes.SEP_CHAR;

        SendDataToMap(modTypes.GetPlayerMap(index), packet);
    }

    public static void SendMap(int index, int mapNum)
    {
        var packet =
            "MAPDATA" +
            modTypes.SEP_CHAR + mapNum +
            modTypes.SEP_CHAR + modTypes.Map[mapNum].Name.Trim() +
            modTypes.SEP_CHAR + modTypes.Map[mapNum].Revision +
            modTypes.SEP_CHAR + modTypes.Map[mapNum].Moral +
            modTypes.SEP_CHAR + modTypes.Map[mapNum].Up +
            modTypes.SEP_CHAR + modTypes.Map[mapNum].Down +
            modTypes.SEP_CHAR + modTypes.Map[mapNum].Left +
            modTypes.SEP_CHAR + modTypes.Map[mapNum].Right +
            modTypes.SEP_CHAR + modTypes.Map[mapNum].Music +
            modTypes.SEP_CHAR + modTypes.Map[mapNum].BootMap +
            modTypes.SEP_CHAR + modTypes.Map[mapNum].BootX +
            modTypes.SEP_CHAR + modTypes.Map[mapNum].BootY +
            modTypes.SEP_CHAR + modTypes.Map[mapNum].Shop;

        for (var y = 0; y <= modTypes.MAX_MAPY; y++)
        {
            for (var x = 0; x <= modTypes.MAX_MAPX; x++)
            {
                ref var tile = ref modTypes.Map[mapNum].Tile[x, y];

                packet = packet +
                         modTypes.SEP_CHAR + tile.Ground +
                         modTypes.SEP_CHAR + tile.Mask +
                         modTypes.SEP_CHAR + tile.Anim +
                         modTypes.SEP_CHAR + tile.Fringe +
                         modTypes.SEP_CHAR + tile.Type +
                         modTypes.SEP_CHAR + tile.Data1 +
                         modTypes.SEP_CHAR + tile.Data2 +
                         modTypes.SEP_CHAR + tile.Data3;
            }
        }

        for (var i = 1; i <= modTypes.MAX_MAP_NPCS; i++)
        {
            packet = packet + modTypes.SEP_CHAR + modTypes.Map[mapNum].Npc[i];
        }

        SendDataTo(index, packet + modTypes.SEP_CHAR);
    }

    public static void SendMapItemsTo(int index, int mapNum)
    {
        var packet = "MAPITEMDATA" + modTypes.SEP_CHAR;

        for (var i = 1; i <= modTypes.MAX_MAP_ITEMS; i++)
        {
            packet = packet +
                     modTypes.MapItem[mapNum, i].Num + modTypes.SEP_CHAR +
                     modTypes.MapItem[mapNum, i].Value + modTypes.SEP_CHAR +
                     modTypes.MapItem[mapNum, i].Dur + modTypes.SEP_CHAR +
                     modTypes.MapItem[mapNum, i].X + modTypes.SEP_CHAR +
                     modTypes.MapItem[mapNum, i].Y + modTypes.SEP_CHAR;
        }

        SendDataTo(index, packet);
    }

    public static void SendMapItemsToAll(int mapNum)
    {
        var packet = "MAPITEMDATA" + modTypes.SEP_CHAR;

        for (var i = 1; i <= modTypes.MAX_MAP_ITEMS; i++)
        {
            packet = packet +
                     modTypes.MapItem[mapNum, i].Num + modTypes.SEP_CHAR +
                     modTypes.MapItem[mapNum, i].Value + modTypes.SEP_CHAR +
                     modTypes.MapItem[mapNum, i].Dur + modTypes.SEP_CHAR +
                     modTypes.MapItem[mapNum, i].X + modTypes.SEP_CHAR +
                     modTypes.MapItem[mapNum, i].Y + modTypes.SEP_CHAR;
        }

        SendDataToMap(mapNum, packet);
    }

    public static void SendMapNpcsTo(int index, int mapNum)
    {
        var packet = "MAPNPCDATA" + modTypes.SEP_CHAR;

        for (var i = 1; i <= modTypes.MAX_MAP_NPCS; i++)
        {
            packet = packet +
                     modTypes.MapNpc[mapNum, i].Num + modTypes.SEP_CHAR +
                     modTypes.MapNpc[mapNum, i].X + modTypes.SEP_CHAR +
                     modTypes.MapNpc[mapNum, i].Y + modTypes.SEP_CHAR +
                     modTypes.MapNpc[mapNum, i].Dir + modTypes.SEP_CHAR;
        }

        SendDataTo(index, packet);
    }

    public static void SendMapNpcsToMap(int mapNum)
    {
        var packet = "MAPNPCDATA" + modTypes.SEP_CHAR;

        for (var i = 1; i <= modTypes.MAX_MAP_NPCS; i++)
        {
            packet = packet +
                     modTypes.MapNpc[mapNum, i].Num + modTypes.SEP_CHAR +
                     modTypes.MapNpc[mapNum, i].X + modTypes.SEP_CHAR +
                     modTypes.MapNpc[mapNum, i].Y + modTypes.SEP_CHAR +
                     modTypes.MapNpc[mapNum, i].Dir + modTypes.SEP_CHAR;
        }

        SendDataToMap(mapNum, packet);
    }

    public static void SendItems(int index)
    {
        for (var i = 1; i <= modTypes.MAX_ITEMS; i++)
        {
            if (!string.IsNullOrWhiteSpace(modTypes.Item[i].Name))
            {
                SendUpdateItemTo(index, i);
            }
        }
    }

    public static void SendNpcs(int index)
    {
        for (var i = 1; i <= modTypes.MAX_NPCS; i++)
        {
            if (!string.IsNullOrWhiteSpace(modTypes.Npc[i].Name))
            {
                SendUpdateNpcTo(index, i);
            }
        }
    }

    public static void SendInventory(int index)
    {
        var packet = "PLAYERINV" + modTypes.SEP_CHAR;

        for (var i = 1; i <= modTypes.MAX_INV; i++)
        {
            packet = packet +
                     modTypes.GetPlayerInvItemNum(index, i) + modTypes.SEP_CHAR +
                     modTypes.GetPlayerInvItemValue(index, i) + modTypes.SEP_CHAR +
                     modTypes.GetPlayerInvItemDur(index, i) + modTypes.SEP_CHAR;
        }

        SendDataTo(index, packet);
    }

    public static void SendInventoryUpdate(int index, int invSlot)
    {
        var packet =
            "PLAYERINVUPDATE" +
            modTypes.SEP_CHAR + invSlot +
            modTypes.SEP_CHAR + modTypes.GetPlayerInvItemNum(index, invSlot) +
            modTypes.SEP_CHAR + modTypes.GetPlayerInvItemValue(index, invSlot) +
            modTypes.SEP_CHAR + modTypes.GetPlayerInvItemDur(index, invSlot) +
            modTypes.SEP_CHAR;

        SendDataTo(index, packet);
    }

    public static void SendWornEquipment(int index)
    {
        var packet =
            "PLAYERWORNEQ" +
            modTypes.SEP_CHAR + modTypes.GetPlayerArmorSlot(index) +
            modTypes.SEP_CHAR + modTypes.GetPlayerWeaponSlot(index) +
            modTypes.SEP_CHAR + modTypes.GetPlayerHelmetSlot(index) +
            modTypes.SEP_CHAR + modTypes.GetPlayerShieldSlot(index) +
            modTypes.SEP_CHAR;

        SendDataTo(index, packet);
    }

    public static void SendHP(int index)
    {
        var packet =
            "PLAYERHP" +
            modTypes.SEP_CHAR + modTypes.GetPlayerMaxHP(index) +
            modTypes.SEP_CHAR + modTypes.GetPlayerHP(index) +
            modTypes.SEP_CHAR;

        SendDataTo(index, packet);
    }

    public static void SendMP(int index)
    {
        var packet =
            "PLAYERMP" +
            modTypes.SEP_CHAR + modTypes.GetPlayerMaxMP(index) +
            modTypes.SEP_CHAR + modTypes.GetPlayerMP(index) +
            modTypes.SEP_CHAR;

        SendDataTo(index, packet);
    }

    public static void SendSP(int index)
    {
        var packet =
            "PLAYERSP" +
            modTypes.SEP_CHAR + modTypes.GetPlayerMaxSP(index) +
            modTypes.SEP_CHAR + modTypes.GetPlayerSP(index) +
            modTypes.SEP_CHAR;

        SendDataTo(index, packet);
    }

    public static void SendStats(int index)
    {
        var packet =
            "PLAYERSTATS" +
            modTypes.SEP_CHAR + modTypes.GetPlayerSTR(index) +
            modTypes.SEP_CHAR + modTypes.GetPlayerDEF(index) +
            modTypes.SEP_CHAR + modTypes.GetPlayerSPEED(index) +
            modTypes.SEP_CHAR + modTypes.GetPlayerMAGI(index) +
            modTypes.SEP_CHAR;

        SendDataTo(index, packet);
    }

    public static void SendWelcome(int index)
    {
        // Send them welcome
        PlayerMsg(index, $"Welcome to {modTypes.GAME_NAME}!  Programmed from scratch by yours truely Consty!  Version {modGeneral.CLIENT_MAJOR}.{modGeneral.CLIENT_MINOR}.{modGeneral.CLIENT_REVISION}", modText.BrightBlue);
        PlayerMsg(index, "Type /help for help on commands.  Use arrow keys to move, hold down shift to run, and use ctrl to attack.", modText.Cyan);

        // Send them MOTD
        if (File.Exists("Motd.txt"))
        {
            var motd = File.ReadAllText("Motd.txt");
            if (!string.IsNullOrWhiteSpace(motd))
            {
                PlayerMsg(index, "MOTD: " + motd.Trim(), modText.BrightCyan);
            }
        }

        // Send whos online
        SendWhosOnline(index);
    }

    public static void SendClasses(int index)
    {
        var packet = "CLASSESDATA" + modTypes.SEP_CHAR + modTypes.Max_Classes + modTypes.SEP_CHAR;

        for (var i = 0; i < modTypes.Max_Classes; i++)
        {
            packet = packet +
                     modTypes.GetClassName(i) + modTypes.SEP_CHAR +
                     modTypes.GetClassMaxHP(i) + modTypes.SEP_CHAR +
                     modTypes.GetClassMaxMP(i) + modTypes.SEP_CHAR +
                     modTypes.GetClassMaxSP(i) + modTypes.SEP_CHAR +
                     modTypes.Class[i].STR + modTypes.SEP_CHAR +
                     modTypes.Class[i].DEF + modTypes.SEP_CHAR +
                     modTypes.Class[i].SPEED + modTypes.SEP_CHAR +
                     modTypes.Class[i].MAGI + modTypes.SEP_CHAR;
        }

        SendDataTo(index, packet);
    }

    public static void SendNewCharClasses(int index)
    {
        var packet = "NEWCHARCLASSES" + modTypes.SEP_CHAR + modTypes.Max_Classes + modTypes.SEP_CHAR;

        for (var i = 0; i < modTypes.Max_Classes; i++)
        {
            packet = packet +
                     modTypes.GetClassName(i) + modTypes.SEP_CHAR +
                     modTypes.GetClassMaxHP(i) + modTypes.SEP_CHAR +
                     modTypes.GetClassMaxMP(i) + modTypes.SEP_CHAR +
                     modTypes.GetClassMaxSP(i) + modTypes.SEP_CHAR +
                     modTypes.Class[i].STR + modTypes.SEP_CHAR +
                     modTypes.Class[i].DEF + modTypes.SEP_CHAR +
                     modTypes.Class[i].SPEED + modTypes.SEP_CHAR +
                     modTypes.Class[i].MAGI + modTypes.SEP_CHAR;
        }

        SendDataTo(index, packet);
    }

    public static void SendLeftGame(int index)
    {
        var packet =
            "PLAYERDATA" +
            modTypes.SEP_CHAR + index +
            modTypes.SEP_CHAR + "" +
            modTypes.SEP_CHAR + 0 +
            modTypes.SEP_CHAR + 0 +
            modTypes.SEP_CHAR + 0 +
            modTypes.SEP_CHAR + 0 +
            modTypes.SEP_CHAR + 0 +
            modTypes.SEP_CHAR + 0 +
            modTypes.SEP_CHAR + 0 +
            modTypes.SEP_CHAR;

        SendDataToAllBut(index, packet);
    }

    public static void SendPlayerXY(int index)
    {
        var packet =
            "PLAYERXY" +
            modTypes.SEP_CHAR + modTypes.GetPlayerX(index) +
            modTypes.SEP_CHAR + modTypes.GetPlayerY(index) +
            modTypes.SEP_CHAR;

        SendDataTo(index, packet);
    }

    public static void SendUpdateItemToAll(int itemNum)
    {
        var packet =
            "UPDATEITEM" +
            modTypes.SEP_CHAR + itemNum +
            modTypes.SEP_CHAR + modTypes.Item[itemNum].Name.Trim() +
            modTypes.SEP_CHAR + modTypes.Item[itemNum].Pic +
            modTypes.SEP_CHAR + modTypes.Item[itemNum].Type +
            modTypes.SEP_CHAR + modTypes.Item[itemNum].Data1 +
            modTypes.SEP_CHAR + modTypes.Item[itemNum].Data2 +
            modTypes.SEP_CHAR + modTypes.Item[itemNum].Data3 +
            modTypes.SEP_CHAR;

        SendDataToAll(packet);
    }

    public static void SendUpdateItemTo(int index, int itemNum)
    {
        var packet =
            "UPDATEITEM" +
            modTypes.SEP_CHAR + itemNum +
            modTypes.SEP_CHAR + modTypes.Item[itemNum].Name.Trim() +
            modTypes.SEP_CHAR + modTypes.Item[itemNum].Pic +
            modTypes.SEP_CHAR + modTypes.Item[itemNum].Type +
            modTypes.SEP_CHAR + modTypes.Item[itemNum].Data1 +
            modTypes.SEP_CHAR + modTypes.Item[itemNum].Data2 +
            modTypes.SEP_CHAR + modTypes.Item[itemNum].Data3 +
            modTypes.SEP_CHAR;

        SendDataTo(index, packet);
    }

    public static void SendEditItemTo(int index, int itemNum)
    {
        var packet =
            "EDITITEM" +
            modTypes.SEP_CHAR + itemNum +
            modTypes.SEP_CHAR + modTypes.Item[itemNum].Name.Trim() +
            modTypes.SEP_CHAR + modTypes.Item[itemNum].Pic +
            modTypes.SEP_CHAR + modTypes.Item[itemNum].Type +
            modTypes.SEP_CHAR + modTypes.Item[itemNum].Data1 +
            modTypes.SEP_CHAR + modTypes.Item[itemNum].Data2 +
            modTypes.SEP_CHAR + modTypes.Item[itemNum].Data3 +
            modTypes.SEP_CHAR;

        SendDataTo(index, packet);
    }

    public static void SendUpdateNpcToAll(int npcNum)
    {
        var packet =
            "UPDATENPC" +
            modTypes.SEP_CHAR + npcNum +
            modTypes.SEP_CHAR + modTypes.Npc[npcNum].Name.Trim() +
            modTypes.SEP_CHAR + modTypes.Npc[npcNum].Sprite +
            modTypes.SEP_CHAR;

        SendDataToAll(packet);
    }

    public static void SendUpdateNpcTo(int index, int npcNum)
    {
        var packet =
            "UPDATENPC" +
            modTypes.SEP_CHAR + npcNum +
            modTypes.SEP_CHAR + modTypes.Npc[npcNum].Name.Trim() +
            modTypes.SEP_CHAR + modTypes.Npc[npcNum].Sprite +
            modTypes.SEP_CHAR;

        SendDataTo(index, packet);
    }

    public static void SendEditNpcTo(int index, int npcNum)
    {
        var packet =
            "EDITNPC" +
            modTypes.SEP_CHAR + npcNum +
            modTypes.SEP_CHAR + modTypes.Npc[npcNum].Name.Trim() +
            modTypes.SEP_CHAR + modTypes.Npc[npcNum].AttackSay.Trim() +
            modTypes.SEP_CHAR + modTypes.Npc[npcNum].Sprite +
            modTypes.SEP_CHAR + modTypes.Npc[npcNum].SpawnSecs +
            modTypes.SEP_CHAR + modTypes.Npc[npcNum].Behavior +
            modTypes.SEP_CHAR + modTypes.Npc[npcNum].Range +
            modTypes.SEP_CHAR + modTypes.Npc[npcNum].DropChance +
            modTypes.SEP_CHAR + modTypes.Npc[npcNum].DropItem +
            modTypes.SEP_CHAR + modTypes.Npc[npcNum].DropItemValue +
            modTypes.SEP_CHAR + modTypes.Npc[npcNum].STR +
            modTypes.SEP_CHAR + modTypes.Npc[npcNum].DEF +
            modTypes.SEP_CHAR + modTypes.Npc[npcNum].SPEED +
            modTypes.SEP_CHAR + modTypes.Npc[npcNum].MAGI +
            modTypes.SEP_CHAR;
        
        SendDataTo(index, packet);
    }

    public static void SendShops(int index)
    {
        for (var i = 1; i <= modTypes.MAX_SHOPS; i++)
        {
            if (!string.IsNullOrWhiteSpace(modTypes.Shop[i].Name))
            {
                SendUpdateShopTo(index, i);
            }
        }
    }

    public static void SendUpdateShopToAll(int shopNum)
    {
        var packet =
            "UPDATESHOP" +
            modTypes.SEP_CHAR + shopNum +
            modTypes.SEP_CHAR + modTypes.Shop[shopNum].Name +
            modTypes.SEP_CHAR;

        SendDataToAll(packet);
    }

    public static void SendUpdateShopTo(int index, int shopNum)
    {
        var packet =
            "UPDATESHOP" +
            modTypes.SEP_CHAR + shopNum +
            modTypes.SEP_CHAR + modTypes.Shop[shopNum].Name +
            modTypes.SEP_CHAR;

        SendDataTo(index, packet);
    }

    public static void SendEditShopTo(int index, int shopNum)
    {
        var packet =
            "EDITSHOP" +
            modTypes.SEP_CHAR + shopNum +
            modTypes.SEP_CHAR + modTypes.Shop[shopNum].Name.Trim() +
            modTypes.SEP_CHAR + modTypes.Shop[shopNum].JoinSay.Trim() +
            modTypes.SEP_CHAR + modTypes.Shop[shopNum].LeaveSay.Trim() +
            modTypes.SEP_CHAR + modTypes.Shop[shopNum].FixesItems;

        for (var i = 1; i <= modTypes.MAX_TRADES; i++)
        {
            packet = packet +
                     modTypes.SEP_CHAR + modTypes.Shop[shopNum].TradeItem[i].GiveItem +
                     modTypes.SEP_CHAR + modTypes.Shop[shopNum].TradeItem[i].GiveValue +
                     modTypes.SEP_CHAR + modTypes.Shop[shopNum].TradeItem[i].GetItem +
                     modTypes.SEP_CHAR + modTypes.Shop[shopNum].TradeItem[i].GetValue;
        }

        SendDataTo(index, packet + modTypes.SEP_CHAR);
    }

    public static void SendSpells(int index)
    {
        for (var i = 1; i <= modTypes.MAX_SPELLS; i++)
        {
            if (!string.IsNullOrWhiteSpace(modTypes.Spell[i].Name))
            {
                SendUpdateSpellTo(index, i);
            }
        }
    }

    public static void SendUpdateSpellToAll(int spellNum)
    {
        var packet = "UPDATESPELL" + modTypes.SEP_CHAR + spellNum + modTypes.SEP_CHAR + modTypes.Spell[spellNum].Name + modTypes.SEP_CHAR;

        SendDataToAll(packet);
    }

    public static void SendUpdateSpellTo(int index, int spellNum)
    {
        var packet = "UPDATESPELL" + modTypes.SEP_CHAR + spellNum + modTypes.SEP_CHAR + modTypes.Spell[spellNum].Name + modTypes.SEP_CHAR;

        SendDataTo(index, packet);
    }

    public static void SendEditSpellTo(int index, int spellNum)
    {
        var packet =
            "EDITSPELL" +
            modTypes.SEP_CHAR + spellNum +
            modTypes.SEP_CHAR + modTypes.Spell[spellNum].Name.Trim() +
            modTypes.SEP_CHAR + modTypes.Spell[spellNum].ClassReq +
            modTypes.SEP_CHAR + modTypes.Spell[spellNum].LevelReq +
            modTypes.SEP_CHAR + modTypes.Spell[spellNum].Type +
            modTypes.SEP_CHAR + modTypes.Spell[spellNum].Data1 +
            modTypes.SEP_CHAR + modTypes.Spell[spellNum].Data2 +
            modTypes.SEP_CHAR + modTypes.Spell[spellNum].Data3 +
            modTypes.SEP_CHAR;

        SendDataTo(index, packet);
    }

    public static void SendTrade(int index, int shopNum)
    {
        var packet =
            "TRADE" +
            modTypes.SEP_CHAR + shopNum +
            modTypes.SEP_CHAR + modTypes.Shop[shopNum].FixesItems;

        for (var i = 1; i <= modTypes.MAX_TRADES; i++)
        {
            packet = packet +
                     modTypes.SEP_CHAR + modTypes.Shop[shopNum].TradeItem[i].GiveItem +
                     modTypes.SEP_CHAR + modTypes.Shop[shopNum].TradeItem[i].GiveValue +
                     modTypes.SEP_CHAR + modTypes.Shop[shopNum].TradeItem[i].GetItem +
                     modTypes.SEP_CHAR + modTypes.Shop[shopNum].TradeItem[i].GetValue;

            // Item #
            var itemNum = modTypes.Shop[shopNum].TradeItem[i].GetItem;

            if (modTypes.Item[itemNum].Type == modTypes.ITEM_TYPE_SPELL)
            {
                // Spell class requirement
                var classNum = modTypes.Spell[modTypes.Item[itemNum].Data1].ClassReq;

                PlayerMsg(index, classNum == 0
                        ? $"{modTypes.Item[itemNum].Name.Trim()} can be used by all classes."
                        : $"{modTypes.Item[itemNum].Name.Trim()} can only be used by a {modTypes.GetClassName(classNum - 1)};",
                    modText.Yellow);
            }
        }

        SendDataTo(index, packet + modTypes.SEP_CHAR);
    }

    public static void SendPlayerSpells(int index)
    {
        var packet = "SPELLS" + modTypes.SEP_CHAR;

        for (var i = 1; i <= modTypes.MAX_PLAYER_SPELLS; i++)
        {
            packet = packet + modTypes.GetPlayerSpell(index, i) + modTypes.SEP_CHAR;
        }

        SendDataTo(index, packet);
    }

    public static void SendWeatherTo(int index)
    {
        var packet = "WEATHER" + modTypes.SEP_CHAR + modGeneral.GameWeather + modTypes.SEP_CHAR;
        SendDataTo(index, packet);
    }

    public static void SendWeatherToAll()
    {
        for (var i = 1; i <= modTypes.MAX_PLAYERS; i++)
        {
            if (IsPlaying(i))
            {
                SendWeatherTo(i);
            }
        }
    }

    public static void SendTimeTo(int index)
    {
        var packet = "TIME" + modTypes.SEP_CHAR + modGeneral.GameTime + modTypes.SEP_CHAR;
        SendDataTo(index, packet);
    }

    public static void SendTimeToAll()
    {
        for (var i = 1; i <= modTypes.MAX_PLAYERS; i++)
        {
            if (IsPlaying(i))
            {
                SendTimeTo(i);
            }
        }
    }
}