namespace Mirage.Net.Protocol.FromServer;

public sealed record MapDone : IPacket<MapDone>
{
    public static string PacketId => "mapdone";

    private static readonly MapDone Instance = new();

    public static MapDone ReadFrom(PacketReader reader)
    {
        return Instance;
    }

    public void WriteTo(PacketWriter writer)
    {
    }
}