namespace Mirage.Net.Protocol.FromClient;

public sealed record OpenItemEditorRequest : IPacket<OpenItemEditorRequest>
{
    public static string PacketId => "requestedititem";

    public static OpenItemEditorRequest ReadFrom(PacketReader reader)
    {
        return EmptyPacket<OpenItemEditorRequest>.Value;
    }

    public void WriteTo(PacketWriter writer)
    {
    }
}