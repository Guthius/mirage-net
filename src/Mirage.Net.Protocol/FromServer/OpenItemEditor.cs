namespace Mirage.Net.Protocol.FromServer;

public sealed record OpenItemEditor : IPacket<OpenItemEditor>
{
    public static string PacketId => "itemeditor";

    public static OpenItemEditor ReadFrom(PacketReader reader)
    {
        return EmptyPacket<OpenItemEditor>.Value;
    }

    public void WriteTo(PacketWriter writer)
    {
    }
}