using System.Text;
using Mirage.Compat;
using Mirage.Game.Constants;

namespace Mirage.Client.Modules;

public static class modClientTCP
{
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

    public static void SendMap()
    {
        var packet =
            "MAPDATA" +
            modTypes.SEP_CHAR + modTypes.Player[modGameLogic.MyIndex].Map +
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

        for (var x = 1; x <= Limits.MaxMapNpcs; x++)
        {
            packet += "" + modTypes.SEP_CHAR + modTypes.Map.Npc[x];
        }

        SendData(packet + modTypes.SEP_CHAR);
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

    public static void SendSaveShop(int shopNum)
    {
        var packet =
            "SAVESHOP" +
            modTypes.SEP_CHAR + shopNum +
            modTypes.SEP_CHAR + modTypes.Shop[shopNum].Name.Trim() +
            modTypes.SEP_CHAR + modTypes.Shop[shopNum].JoinSay.Trim() +
            modTypes.SEP_CHAR + modTypes.Shop[shopNum].LeaveSay.Trim() +
            modTypes.SEP_CHAR + modTypes.Shop[shopNum].FixesItems;

        for (var i = 1; i <= Limits.MaxShopTrades; i++)
        {
            packet = packet +
                     modTypes.SEP_CHAR + modTypes.Shop[shopNum].TradeItem[i].GiveItem +
                     modTypes.SEP_CHAR + modTypes.Shop[shopNum].TradeItem[i].GiveValue +
                     modTypes.SEP_CHAR + modTypes.Shop[shopNum].TradeItem[i].GetItem +
                     modTypes.SEP_CHAR + modTypes.Shop[shopNum].TradeItem[i].GetValue;
        }

        SendData(packet + modTypes.SEP_CHAR);
    }
}