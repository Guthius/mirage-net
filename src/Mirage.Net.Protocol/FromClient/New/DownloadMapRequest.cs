namespace Mirage.Net.Protocol.FromClient.New;

public sealed record DownloadMapRequest(string MapName) : IPacket<DownloadMapRequest>
{
    public static string PacketId => nameof(DownloadMapRequest);

    public static DownloadMapRequest ReadFrom(PacketReader reader)
    {
        return new DownloadMapRequest(MapName: reader.ReadString());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteString(MapName);
    }
}