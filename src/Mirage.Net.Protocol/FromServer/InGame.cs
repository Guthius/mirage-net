namespace Mirage.Net.Protocol.FromServer;

public sealed record InGame : IPacket<InGame>
{
    public static string PacketId => "ingame";

    public static InGame ReadFrom(PacketReader reader)
    {
        return EmptyPacket<InGame>.Value;
    }

    public void WriteTo(PacketWriter writer)
    {
    }
}