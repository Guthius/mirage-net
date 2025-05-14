namespace Mirage.Net.Protocol.FromClient;

public sealed record OpenMapEditorRequest : IPacket<OpenMapEditorRequest>
{
    public static string PacketId => "requesteditmap";
    
    public static OpenMapEditorRequest ReadFrom(PacketReader reader)
    {
        return EmptyPacket<OpenMapEditorRequest>.Value;
    }

    public void WriteTo(PacketWriter writer)
    {
    }
}