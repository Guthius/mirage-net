namespace Mirage.Net.Protocol.FromServer;

public sealed record UpdateShop(int ShopId, string Name) : IPacket<UpdateShop>
{
    public static string PacketId => "updateshop";

    public static UpdateShop ReadFrom(PacketReader reader)
    {
        return new UpdateShop(ShopId: reader.ReadInt32(), Name: reader.ReadString());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(ShopId);
        writer.WriteString(Name);
    }
}