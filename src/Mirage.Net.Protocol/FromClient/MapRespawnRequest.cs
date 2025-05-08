namespace Mirage.Net.Protocol.FromClient;

public sealed record MapRespawnRequest : IPacket<MapRespawnRequest>
{
    public static string PacketId => "maprespawn";

    public static MapRespawnRequest ReadFrom(PacketReader reader)
    {
        return EmptyPacket<MapRespawnRequest>.Value;
    }

    public void WriteTo(PacketWriter writer)
    {
    }
}