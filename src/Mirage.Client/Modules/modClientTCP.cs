using System.Reflection;
using System.Text;
using Mirage.Compat;

namespace Mirage.Modules;

public static class modClientTCP
{
    private static readonly SynchronizationContext SynchronizationContext = SynchronizationContext.Current!;
    private static readonly byte[] PlayerBuffer = new byte[0xFFFF];
    private static int _playerBufferLen;

    public static bool InGame;

    public static void TcpInit()
    {
        _playerBufferLen = 0;

        My.Forms.frmMirage.Socket.RemoteHost = "127.0.0.1";
        My.Forms.frmMirage.Socket.RemotePort = modTypes.GAME_PORT;
    }

    public static void TcpDestroy()
    {
        My.Forms.frmMirage.Socket.Close();

        My.Forms.frmChars.Hide();
        My.Forms.frmCredits.Hide();
        My.Forms.frmDeleteAccount.Hide();
        My.Forms.frmLogin.Hide();
        My.Forms.frmNewAccount.Hide();
        My.Forms.frmNewChar.Hide();
    }

    public static void IncomingData(byte[] bytes)
    {
        Buffer.BlockCopy(bytes, 0, PlayerBuffer, _playerBufferLen, bytes.Length);

        _playerBufferLen += bytes.Length;

        var buffer = PlayerBuffer.AsSpan(0, _playerBufferLen);
        while (buffer.Length > 0)
        {
            var end = buffer.IndexOf((byte) 237);
            if (end == -1)
            {
                break;
            }

            var packet = Encoding.UTF8.GetString(buffer[..end]);

            SynchronizationContext.Post(_ => modHandleData.HandleData(packet), null);

            buffer = buffer[(end + 1)..];
        }

        buffer.CopyTo(PlayerBuffer.AsSpan());

        _playerBufferLen = buffer.Length;
    }

    public static bool ConnectToServer()
    {
        // Check to see if we are already connected, if so just exit
        if (IsConnected())
        {
            return true;
        }

        var wait = modGameLogic.GetTickCount();
        My.Forms.frmMirage.Socket.Close();
        My.Forms.frmMirage.Socket.Connect();

        // Wait until connected or 3 seconds have passed and report the server being down
        while (!IsConnected() && modGameLogic.GetTickCount() <= wait + 3000)
        {
            Application.DoEvents();
        }

        return IsConnected();
    }

    public static bool IsConnected()
    {
        return My.Forms.frmMirage.Socket.State == WinsockState.Connected;
    }

    public static bool IsPlaying(int index)
    {
        return modTypes.GetPlayerName(index) != "";
    }

    public static void SendData(ReadOnlySpan<char> data)
    {
        if (!IsConnected())
        {
            return;
        }

        var len = Encoding.UTF8.GetByteCount(data);
        var packet = new byte[len + 1];
        Encoding.UTF8.GetBytes(data, packet);
        packet[len] = 237;

        My.Forms.frmMirage.Socket.SendData(packet);

        Application.DoEvents();
    }

    public static void SendNewAccount(string name, string password)
    {
        var packet =
            "newaccount" +
            modTypes.SEP_CHAR + name.Trim() +
            modTypes.SEP_CHAR + password.Trim() +
            modTypes.SEP_CHAR;

        SendData(packet);
    }

    public static void SendDelAccount(string name, string password)
    {
        var packet =
            "delaccount" +
            modTypes.SEP_CHAR + name.Trim() +
            modTypes.SEP_CHAR + password.Trim() +
            modTypes.SEP_CHAR;

        SendData(packet);
    }

    public static void SendLogin(string name, string password)
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(1, 0, 0);

        var packet =
            "login" +
            modTypes.SEP_CHAR + name +
            modTypes.SEP_CHAR + password +
            modTypes.SEP_CHAR + version.Major +
            modTypes.SEP_CHAR + version.Minor +
            modTypes.SEP_CHAR + version.Revision +
            modTypes.SEP_CHAR;

        SendData(packet);
    }

    public static void SendAddChar(string name, int sex, int classNum, int slot)
    {
        var packet =
            "addchar" +
            modTypes.SEP_CHAR + name.Trim() +
            modTypes.SEP_CHAR + sex +
            modTypes.SEP_CHAR + classNum +
            modTypes.SEP_CHAR + slot +
            modTypes.SEP_CHAR;

        SendData(packet);
    }


    public static void SendDelChar(int slot)
    {
        var packet =
            "delchar" +
            modTypes.SEP_CHAR + slot +
            modTypes.SEP_CHAR;

        SendData(packet);
    }

    public static void SendGetClasses()
    {
        var packet = "getclasses" + modTypes.SEP_CHAR;

        SendData(packet);
    }

    public static void SendUseChar(int slot)
    {
        var packet =
            "usechar" +
            modTypes.SEP_CHAR + slot +
            modTypes.SEP_CHAR;

        SendData(packet);
    }

    public static void SayMsg(string text)
    {
        var packet =
            "saymsg" +
            modTypes.SEP_CHAR + text.Trim() +
            modTypes.SEP_CHAR;

        SendData(packet);
    }

    public static void GlobalMsg(string text)
    {
        var packet =
            "globalmsg" +
            modTypes.SEP_CHAR + text.Trim() +
            modTypes.SEP_CHAR;

        SendData(packet);
    }

    public static void BroadcastMsg(string text)
    {
        var packet =
            "broadcastmsg" +
            modTypes.SEP_CHAR + text.Trim() +
            modTypes.SEP_CHAR;

        SendData(packet);
    }

    public static void EmoteMsg(string text)
    {
        var packet =
            "emotemsg" +
            modTypes.SEP_CHAR + text.Trim() +
            modTypes.SEP_CHAR;

        SendData(packet);
    }

    public static void MapMsg(string text)
    {
        var packet =
            "mapmsg" +
            modTypes.SEP_CHAR + text.Trim() +
            modTypes.SEP_CHAR;

        SendData(packet);
    }

    public static void PlayerMsg(string text, string msgTo)
    {
        var packet =
            "playermsg" +
            modTypes.SEP_CHAR + msgTo.Trim() +
            modTypes.SEP_CHAR + text.Trim() +
            modTypes.SEP_CHAR;

        SendData(packet);
    }

    public static void AdminMsg(string text)
    {
        var packet =
            "adminmsg" +
            modTypes.SEP_CHAR + text.Trim() +
            modTypes.SEP_CHAR;

        SendData(packet);
    }

    public static void SendPlayerMove()
    {
        var packet =
            "playermove" +
            modTypes.SEP_CHAR + modTypes.GetPlayerDir(modGameLogic.MyIndex) +
            modTypes.SEP_CHAR + modTypes.Player[modGameLogic.MyIndex].Moving +
            modTypes.SEP_CHAR;

        SendData(packet);
    }

    public static void SendPlayerDir()
    {
        var packet =
            "playerdir" +
            modTypes.SEP_CHAR + modTypes.GetPlayerDir(modGameLogic.MyIndex) +
            modTypes.SEP_CHAR;

        SendData(packet);
    }

    public static void SendPlayerRequestNewMap()
    {
        var packet =
            "requestnewmap" +
            modTypes.SEP_CHAR + modTypes.GetPlayerDir(modGameLogic.MyIndex) +
            modTypes.SEP_CHAR;

        SendData(packet);
    }

    public static void SendMap()
    {
        var packet =
            "MAPDATA" +
            modTypes.SEP_CHAR + modTypes.GetPlayerMap(modGameLogic.MyIndex) +
            modTypes.SEP_CHAR + modTypes.Map.Name.Trim() +
            modTypes.SEP_CHAR + modTypes.Map.Revision +
            modTypes.SEP_CHAR + modTypes.Map.Moral +
            modTypes.SEP_CHAR + modTypes.Map.Up +
            modTypes.SEP_CHAR + modTypes.Map.Down +
            modTypes.SEP_CHAR + modTypes.Map.Left +
            modTypes.SEP_CHAR + modTypes.Map.Right +
            modTypes.SEP_CHAR + modTypes.Map.Music +
            modTypes.SEP_CHAR + modTypes.Map.BootMap +
            modTypes.SEP_CHAR + modTypes.Map.BootX +
            modTypes.SEP_CHAR + modTypes.Map.BootY +
            modTypes.SEP_CHAR + modTypes.Map.Shop;

        for (var y = 0; y <= modTypes.MAX_MAPY; y++)
        {
            for (var x = 0; x <= modTypes.MAX_MAPX; x++)
            {
                var tile = modTypes.Map.Tile[x, y];

                packet += "" +
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

        for (var x = 1; x <= modTypes.MAX_MAP_NPCS; x++)
        {
            packet += "" + modTypes.SEP_CHAR + modTypes.Map.Npc[x];
        }

        SendData(packet + modTypes.SEP_CHAR);
    }

    public static void WarpMeTo(string name)
    {
        var packet =
            "WARPMETO" +
            modTypes.SEP_CHAR + name.Trim() +
            modTypes.SEP_CHAR;

        SendData(packet);
    }

    public static void WarpToMe(string name)
    {
        var packet =
            "WARPTOME" +
            modTypes.SEP_CHAR + name.Trim() +
            modTypes.SEP_CHAR;

        SendData(packet);
    }

    public static void WarpTo(int mapNum)
    {
        var packet =
            "WARPTO" +
            modTypes.SEP_CHAR + mapNum +
            modTypes.SEP_CHAR;

        SendData(packet);
    }

    public static void SendSetAccess(string name, int access)
    {
        var packet =
            "SETACCESS" +
            modTypes.SEP_CHAR + name.Trim() +
            modTypes.SEP_CHAR + access +
            modTypes.SEP_CHAR;

        SendData(packet);
    }

    public static void SendSetSprite(int spriteNum)
    {
        var packet =
            "SETSPRITE" +
            modTypes.SEP_CHAR + spriteNum +
            modTypes.SEP_CHAR;

        SendData(packet);
    }

    public static void SendKick(string name)
    {
        var packet =
            "KICKPLAYER" +
            modTypes.SEP_CHAR + name.Trim() +
            modTypes.SEP_CHAR;

        SendData(packet);
    }

    public static void SendBan(string name)
    {
        var packet =
            "BANPLAYER" +
            modTypes.SEP_CHAR + name.Trim() +
            modTypes.SEP_CHAR;

        SendData(packet);
    }

    public static void SendBanList()
    {
        var packet = "BANLIST" + modTypes.SEP_CHAR;

        SendData(packet);
    }

    public static void SendRequestEditItem()
    {
        var packet = "REQUESTEDITITEM" + modTypes.SEP_CHAR;

        SendData(packet);
    }

    public static void SendSaveItem(int itemNum)
    {
        var packet =
            "SAVEITEM" +
            modTypes.SEP_CHAR + itemNum +
            modTypes.SEP_CHAR + modTypes.Item[itemNum].Name.Trim() +
            modTypes.SEP_CHAR + modTypes.Item[itemNum].Pic +
            modTypes.SEP_CHAR + modTypes.Item[itemNum].Type +
            modTypes.SEP_CHAR + modTypes.Item[itemNum].Data1 +
            modTypes.SEP_CHAR + modTypes.Item[itemNum].Data2 +
            modTypes.SEP_CHAR + modTypes.Item[itemNum].Data3 +
            modTypes.SEP_CHAR;

        SendData(packet);
    }

    public static void SendRequestEditNpc()
    {
        var packet = "REQUESTEDITNPC" + modTypes.SEP_CHAR;

        SendData(packet);
    }

    public static void SendSaveNpc(int npcNum)
    {
        var packet =
            "SAVENPC" +
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

        SendData(packet);
    }

    public static void SendMapRespawn()
    {
        var packet = "MAPRESPAWN" + modTypes.SEP_CHAR;

        SendData(packet);
    }

    public static void SendUseItem(int invNum)
    {
        var packet =
            "USEITEM" +
            modTypes.SEP_CHAR + invNum +
            modTypes.SEP_CHAR;

        SendData(packet);
    }

    public static void SendDropItem(int invNum, int amount)
    {
        var packet =
            "DROPITEM" +
            modTypes.SEP_CHAR + invNum +
            modTypes.SEP_CHAR + amount +
            modTypes.SEP_CHAR;

        SendData(packet);
    }

    public static void SendWhosOnline()
    {
        var packet = "WHOSONLINE" + modTypes.SEP_CHAR;

        SendData(packet);
    }

    public static void SendMOTDChange(string motd)
    {
        var packet =
            "SETMOTD" +
            modTypes.SEP_CHAR + motd.Trim() +
            modTypes.SEP_CHAR;

        SendData(packet);
    }

    public static void SendRequestEditShop()
    {
        var packet = "REQUESTEDITSHOP" + modTypes.SEP_CHAR;

        SendData(packet);
    }

    public static void SendSaveShop(int shopNum)
    {
        var packet =
            "SAVESHOP" +
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

        SendData(packet + modTypes.SEP_CHAR);
    }

    public static void SendRequestEditSpell()
    {
        var packet = "REQUESTEDITSPELL" + modTypes.SEP_CHAR;

        SendData(packet);
    }

    public static void SendSaveSpell(int spellNum)
    {
        var packet =
            "SAVESPELL" +
            modTypes.SEP_CHAR + spellNum +
            modTypes.SEP_CHAR + modTypes.Spell[spellNum].Name.Trim() +
            modTypes.SEP_CHAR + modTypes.Spell[spellNum].ClassReq +
            modTypes.SEP_CHAR + modTypes.Spell[spellNum].LevelReq +
            modTypes.SEP_CHAR + modTypes.Spell[spellNum].Type +
            modTypes.SEP_CHAR + modTypes.Spell[spellNum].Data1 +
            modTypes.SEP_CHAR + modTypes.Spell[spellNum].Data2 +
            modTypes.SEP_CHAR + modTypes.Spell[spellNum].Data3 +
            modTypes.SEP_CHAR;

        SendData(packet);
    }

    public static void SendRequestEditMap()
    {
        var packet = "REQUESTEDITMAP" + modTypes.SEP_CHAR;

        SendData(packet);
    }

    public static void SendPartyRequest(string name)
    {
        var packet =
            "PARTY" +
            modTypes.SEP_CHAR + name +
            modTypes.SEP_CHAR;

        SendData(packet);
    }

    public static void SendJoinParty()
    {
        var packet = "JOINPARTY" + modTypes.SEP_CHAR;

        SendData(packet);
    }

    public static void SendLeaveParty()
    {
        var packet = "LEAVEPARTY" + modTypes.SEP_CHAR;

        SendData(packet);
    }

    public static void SendBanDestroy()
    {
        var packet = "BANDESTROY" + modTypes.SEP_CHAR;

        SendData(packet);
    }

    public static void SendRequestLocation()
    {
        var packet = "REQUESTLOCATION" + modTypes.SEP_CHAR;

        SendData(packet);
    }
}