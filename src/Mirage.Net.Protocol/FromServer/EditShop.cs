using Mirage.Game.Constants;
using Mirage.Game.Data;

namespace Mirage.Net.Protocol.FromServer;

public sealed record EditShop(ShopInfo ShopInfo) : IPacket<EditShop>
{
    public static string PacketId => "editshop";

    public static EditShop ReadFrom(PacketReader reader)
    {
        var shopInfo = new ShopInfo
        {
            Id = reader.ReadInt32(),
            Name = reader.ReadString(),
            JoinSay = reader.ReadString(),
            LeaveSay = reader.ReadString(),
            FixesItems = reader.ReadBoolean(),
            Trades = new ShopTradeInfo[Limits.MaxShopTrades + 1]
        };

        for (var i = 1; i <= Limits.MaxShopTrades; i++)
        {
            shopInfo.Trades[i] = new ShopTradeInfo
            {
                GiveItemId = reader.ReadInt32(),
                GiveItemQuantity = reader.ReadInt32(),
                GetItemId = reader.ReadInt32(),
                GetItemQuantity = reader.ReadInt32()
            };
        }

        return new EditShop(shopInfo);
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(ShopInfo.Id);
        writer.WriteString(ShopInfo.Name);
        writer.WriteString(ShopInfo.JoinSay);
        writer.WriteString(ShopInfo.LeaveSay);
        writer.WriteBoolean(ShopInfo.FixesItems);

        for (var i = 1; i <= Limits.MaxShopTrades; i++)
        {
            writer.WriteInt32(ShopInfo.Trades[i].GiveItemId);
            writer.WriteInt32(ShopInfo.Trades[i].GiveItemQuantity);
            writer.WriteInt32(ShopInfo.Trades[i].GetItemId);
            writer.WriteInt32(ShopInfo.Trades[i].GetItemQuantity);
        }
    }
}