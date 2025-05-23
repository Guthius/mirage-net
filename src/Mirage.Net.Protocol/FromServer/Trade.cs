using Mirage.Shared.Constants;
using Mirage.Shared.Data;

namespace Mirage.Net.Protocol.FromServer;

public sealed record Trade(int ShopId, bool FixesItems, ShopTradeInfo[] Trades) : IPacket<Trade>
{
    public static string PacketId => "trade";

    public static Trade ReadFrom(PacketReader reader)
    {
        var shopId = reader.ReadInt32();
        var fixesItems = reader.ReadBoolean();

        var trades = new ShopTradeInfo[Limits.MaxShopTrades + 1];
        for (var i = 1; i <= Limits.MaxShopTrades; i++)
        {
            trades[i] = new ShopTradeInfo
            {
                GiveItemId = reader.ReadInt32(),
                GiveItemQuantity = reader.ReadInt32(),
                GetItemId = reader.ReadInt32(),
                GetItemQuantity = reader.ReadInt32()
            };
        }

        return new Trade(shopId, fixesItems, trades);
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(ShopId);
        writer.WriteBoolean(FixesItems);

        foreach (var trade in Trades)
        {
            writer.WriteInt32(trade.GiveItemId);
            writer.WriteInt32(trade.GiveItemQuantity);
            writer.WriteInt32(trade.GetItemId);
            writer.WriteInt32(trade.GetItemQuantity);
        }
    }
}