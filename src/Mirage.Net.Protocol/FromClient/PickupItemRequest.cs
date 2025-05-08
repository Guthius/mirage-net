namespace Mirage.Net.Protocol.FromClient;

public sealed record PickupItemRequest : IPacket<PickupItemRequest>
{
    public static string PacketId => "mapgetitem";

    public static PickupItemRequest ReadFrom(PacketReader reader)
    {
        return EmptyPacket<PickupItemRequest>.Value;
    }

    public void WriteTo(PacketWriter writer)
    {
    }
}