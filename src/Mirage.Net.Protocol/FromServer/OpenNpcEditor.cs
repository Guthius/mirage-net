namespace Mirage.Net.Protocol.FromServer;

public sealed record OpenNpcEditor : IPacket<OpenNpcEditor>
{
    public static string PacketId => "npceditor";

    public static OpenNpcEditor ReadFrom(PacketReader reader)
    {
        return EmptyPacket<OpenNpcEditor>.Value;
    }

    public void WriteTo(PacketWriter writer)
    {
    }
}