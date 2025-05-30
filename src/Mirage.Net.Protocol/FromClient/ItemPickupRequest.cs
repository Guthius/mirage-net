namespace Mirage.Net.Protocol.FromClient;

public sealed record ItemPickupRequest : IPacket<ItemPickupRequest>
{
    public static string PacketId => nameof(ItemPickupRequest);

    public static ItemPickupRequest ReadFrom(PacketReader reader)
    {
        return EmptyPacket<ItemPickupRequest>.Value;
    }

    public void WriteTo(PacketWriter writer)
    {
    }
}