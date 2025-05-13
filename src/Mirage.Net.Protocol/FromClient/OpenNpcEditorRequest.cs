namespace Mirage.Net.Protocol.FromClient;

public sealed record OpenNpcEditorRequest : IPacket<OpenNpcEditorRequest>
{
    public static string PacketId => "requesteditnpc";
    
    public static OpenNpcEditorRequest ReadFrom(PacketReader reader)
    {
        return EmptyPacket<OpenNpcEditorRequest>.Value;
    }

    public void WriteTo(PacketWriter writer)
    {
    }
}