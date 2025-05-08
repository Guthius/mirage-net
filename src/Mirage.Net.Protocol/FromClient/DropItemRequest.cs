namespace Mirage.Net.Protocol.FromClient;

public sealed record DropItemRequest(int InventorySlot, int Quantity) : IPacket<DropItemRequest>
{
    public static string PacketId => "mapdropitem";

    public static DropItemRequest ReadFrom(PacketReader reader)
    {
        return new DropItemRequest(
            InventorySlot: reader.ReadInt32(),
            Quantity: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(InventorySlot);
        writer.WriteInt32(Quantity);
    }
}