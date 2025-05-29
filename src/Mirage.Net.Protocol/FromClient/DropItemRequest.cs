namespace Mirage.Net.Protocol.FromClient;

public sealed record DropItemRequest(int SlotIndex, int Quantity) : IPacket<DropItemRequest>
{
    public static string PacketId => nameof(DropItemRequest);

    public static DropItemRequest ReadFrom(PacketReader reader)
    {
        return new DropItemRequest(
            SlotIndex: reader.ReadInt32(),
            Quantity: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(SlotIndex);
        writer.WriteInt32(Quantity);
    }
}