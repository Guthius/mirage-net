namespace Mirage.Net.Protocol.FromClient;

public sealed record CastRequest(int SpellSlot) : IPacket<CastRequest>
{
    public static string PacketId => "cast";

    public static CastRequest ReadFrom(PacketReader reader)
    {
        return new CastRequest(SpellSlot: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(SpellSlot);
    }
}