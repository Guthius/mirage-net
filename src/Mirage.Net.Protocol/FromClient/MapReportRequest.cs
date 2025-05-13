namespace Mirage.Net.Protocol.FromClient;

public sealed record MapReportRequest : IPacket<MapReportRequest>
{
    public static string PacketId => "mapreport";
    
    public static MapReportRequest ReadFrom(PacketReader reader)
    {
        return EmptyPacket<MapReportRequest>.Value;
    }

    public void WriteTo(PacketWriter writer)
    {
    }
}