namespace Mirage.Net.Protocol.FromClient;

public sealed record EditShopRequest(int ShopId) : IPacket<EditShopRequest>
{
    public static string PacketId => "editshop";

    public static EditShopRequest ReadFrom(PacketReader reader)
    {
        return new EditShopRequest(ShopId: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(ShopId);
    }
}