namespace Mirage.Net.Protocol.FromClient;

public sealed record GetStatsRequest : IPacket<GetStatsRequest>
{
    public static string PacketId => "getstats";
    
    public static GetStatsRequest ReadFrom(PacketReader reader)
    {
        return EmptyPacket<GetStatsRequest>.Value;
    }

    public void WriteTo(PacketWriter writer)
    {
    }
}