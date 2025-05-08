namespace Mirage.Net.Protocol.FromClient;

public sealed record BanDestroyRequest : IPacket<BanDestroyRequest>
{
    public static string PacketId => "bandestroy";

    public static BanDestroyRequest ReadFrom(PacketReader reader)
    {
        return EmptyPacket<BanDestroyRequest>.Value;
    }

    public void WriteTo(PacketWriter writer)
    {
    }
}