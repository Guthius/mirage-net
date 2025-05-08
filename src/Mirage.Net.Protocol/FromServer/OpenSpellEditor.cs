namespace Mirage.Net.Protocol.FromServer;

public sealed record OpenSpellEditor : IPacket<OpenSpellEditor>
{
    public static string PacketId => "spelleditor";

    public static OpenSpellEditor ReadFrom(PacketReader reader)
    {
        return EmptyPacket<OpenSpellEditor>.Value;
    }

    public void WriteTo(PacketWriter writer)
    {
    }
}