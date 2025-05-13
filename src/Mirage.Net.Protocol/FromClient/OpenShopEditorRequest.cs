namespace Mirage.Net.Protocol.FromClient;

public sealed record OpenShopEditorRequest : IPacket<OpenShopEditorRequest>
{
    public static string PacketId => "requesteditshop";
    
    public static OpenShopEditorRequest ReadFrom(PacketReader reader)
    {
        return EmptyPacket<OpenShopEditorRequest>.Value;
    }

    public void WriteTo(PacketWriter writer)
    {
    }
}