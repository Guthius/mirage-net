namespace Mirage.Net.Protocol.FromServer;

public sealed record OpenShopEditor : IPacket<OpenShopEditor>
{
    public static string PacketId => "shopeditor";

    public static OpenShopEditor ReadFrom(PacketReader reader)
    {
        return EmptyPacket<OpenShopEditor>.Value;
    }

    public void WriteTo(PacketWriter writer)
    {
    }
}