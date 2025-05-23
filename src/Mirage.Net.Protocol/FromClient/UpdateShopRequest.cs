using Mirage.Shared.Constants;
using Mirage.Shared.Data;

namespace Mirage.Net.Protocol.FromClient;

public sealed record UpdateShopRequest(ShopInfo ShopInfo) : IPacket<UpdateShopRequest>
{
    public static string PacketId => "saveshop";

    public static UpdateShopRequest ReadFrom(PacketReader reader)
    {
        var shop = new ShopInfo
        {
            Id = reader.ReadInt32(),
            Name = reader.ReadString(),
            JoinSay = reader.ReadString(),
            LeaveSay = reader.ReadString(),
            FixesItems = reader.ReadBoolean()
        };

        for (var i = 1; i <= Limits.MaxShopTrades; i++)
        {
            shop.Trades[i].GiveItemId = reader.ReadInt32();
            shop.Trades[i].GiveItemQuantity = reader.ReadInt32();
            shop.Trades[i].GetItemId = reader.ReadInt32();
            shop.Trades[i].GetItemQuantity = reader.ReadInt32();
        }

        return new UpdateShopRequest(shop);
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(ShopInfo.Id);
        writer.WriteString(ShopInfo.Name);
        writer.WriteString(ShopInfo.JoinSay);
        writer.WriteString(ShopInfo.LeaveSay);
        writer.WriteBoolean(ShopInfo.FixesItems);

        foreach (var shopTradeInfo in ShopInfo.Trades)
        {
            writer.WriteInt32(shopTradeInfo.GiveItemId);
            writer.WriteInt32(shopTradeInfo.GiveItemQuantity);
            writer.WriteInt32(shopTradeInfo.GetItemId);
            writer.WriteInt32(shopTradeInfo.GetItemQuantity);
        }
    }
}