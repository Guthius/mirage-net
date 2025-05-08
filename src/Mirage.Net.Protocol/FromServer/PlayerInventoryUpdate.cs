namespace Mirage.Net.Protocol.FromServer;

public sealed record PlayerInventoryUpdate(int InventorySlot, int ItemId, int Quantity, int Durability) : IPacket<PlayerInventoryUpdate>
{
    public static string PacketId => "playerinvupdate";

    public static PlayerInventoryUpdate ReadFrom(PacketReader reader)
    {
        return new PlayerInventoryUpdate(
            InventorySlot: reader.ReadInt32(),
            ItemId: reader.ReadInt32(),
            Quantity: reader.ReadInt32(),
            Durability: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(InventorySlot);
        writer.WriteInt32(ItemId);
        writer.WriteInt32(Quantity);
        writer.WriteInt32(Durability);
    }
}