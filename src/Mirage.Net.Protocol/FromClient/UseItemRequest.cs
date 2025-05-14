namespace Mirage.Net.Protocol.FromClient;

public sealed record UseItemRequest(int InventorySlot) : IPacket<UseItemRequest>
{
    public static string PacketId => "useitem";

    public static UseItemRequest ReadFrom(PacketReader reader)
    {
        return new UseItemRequest(InventorySlot: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(InventorySlot);
    }
}