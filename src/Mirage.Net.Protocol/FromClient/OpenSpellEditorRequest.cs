namespace Mirage.Net.Protocol.FromClient;

public sealed record OpenSpellEditorRequest : IPacket<OpenSpellEditorRequest>
{
    public static string PacketId => "requesteditspell";

    public static OpenSpellEditorRequest ReadFrom(PacketReader reader)
    {
        return EmptyPacket<OpenSpellEditorRequest>.Value;
    }

    public void WriteTo(PacketWriter writer)
    {
    }
}