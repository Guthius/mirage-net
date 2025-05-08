namespace Mirage.Net.Protocol.FromClient;

public sealed record OpenManEditorRequest : IPacket<OpenManEditorRequest>
{
    public static string PacketId => "requesteditmap";
    
    public static OpenManEditorRequest ReadFrom(PacketReader reader)
    {
        return EmptyPacket<OpenManEditorRequest>.Value;
    }

    public void WriteTo(PacketWriter writer)
    {
    }
}