namespace Mirage.Net.Protocol.FromClient;

public sealed record FixItemRequest(int InventorySlot) : IPacket<FixItemRequest>
{
    public static string PacketId => "fixitem";

    public static FixItemRequest ReadFrom(PacketReader reader)
    {
        return new FixItemRequest(InventorySlot: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(InventorySlot);
    }
}