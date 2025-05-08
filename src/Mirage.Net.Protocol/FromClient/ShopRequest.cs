namespace Mirage.Net.Protocol.FromClient;

public sealed record ShopRequest : IPacket<ShopRequest>
{
    public static string PacketId => "trade";
    
    public static ShopRequest ReadFrom(PacketReader reader)
    {
        return EmptyPacket<ShopRequest>.Value;
    }

    public void WriteTo(PacketWriter writer)
    {
    }
}