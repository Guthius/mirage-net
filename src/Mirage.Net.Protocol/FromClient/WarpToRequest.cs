namespace Mirage.Net.Protocol.FromClient;

public sealed record WarpToRequest(int MapId) : IPacket<WarpToRequest>
{
    public static string PacketId => "warpto";

    public static WarpToRequest ReadFrom(PacketReader reader)
    {
        return new WarpToRequest(MapId: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(MapId);
    }
}