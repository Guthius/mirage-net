namespace Mirage.Net.Protocol.FromClient;

public sealed record WhosOnlineRequest : IPacket<WhosOnlineRequest>
{
    public static string PacketId => "whosonline";

    public static WhosOnlineRequest ReadFrom(PacketReader reader)
    {
        return EmptyPacket<WhosOnlineRequest>.Value;
    }

    public void WriteTo(PacketWriter writer)
    {
    }
}