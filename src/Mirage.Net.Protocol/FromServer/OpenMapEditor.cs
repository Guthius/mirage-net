namespace Mirage.Net.Protocol.FromServer;

public sealed record OpenMapEditor : IPacket<OpenMapEditor>
{
    public static string PacketId => "editmap";

    public static OpenMapEditor ReadFrom(PacketReader reader)
    {
        return EmptyPacket<OpenMapEditor>.Value;
    }

    public void WriteTo(PacketWriter writer)
    {
    }
}