namespace Mirage.Net.Protocol.FromServer;

public sealed record CheckForMap(int MapId, int Revision) : IPacket<CheckForMap>
{
    public static string PacketId => "checkformap";

    public static CheckForMap ReadFrom(PacketReader reader)
    {
        return new CheckForMap(MapId: reader.ReadInt32(), Revision: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(MapId);
        writer.WriteInt32(Revision);
    }
}