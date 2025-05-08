namespace Mirage.Net.Protocol.FromClient;

public sealed record EditSpellRequest(int SpellId) : IPacket<EditSpellRequest>
{
    public static string PacketId => "editspell";

    public static EditSpellRequest ReadFrom(PacketReader reader)
    {
        return new EditSpellRequest(SpellId: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(SpellId);
    }
}