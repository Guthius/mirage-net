namespace Mirage.Net.Protocol.FromServer;

public sealed record UpdateSpell(int SpellId, string Name) : IPacket<UpdateSpell>
{
    public static string PacketId => "updatespell";

    public static UpdateSpell ReadFrom(PacketReader reader)
    {
        return new UpdateSpell(
            SpellId: reader.ReadInt32(),
            Name: reader.ReadString());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(SpellId);
        writer.WriteString(Name);
    }
}