namespace Mirage.Net.Protocol.FromClient;

public sealed record BanListRequest : IPacket<BanListRequest>
{
    public static string PacketId => "banlist";
    
    public static BanListRequest ReadFrom(PacketReader reader)
    {
        return EmptyPacket<BanListRequest>.Value;
    }

    public void WriteTo(PacketWriter writer)
    {
    }
}