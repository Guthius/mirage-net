namespace Mirage.Net.Protocol.FromServer;

public sealed record UpdateInventorySlotQuantityCommand(int SlotIndex, int Quantity) : IPacket<UpdateInventorySlotQuantityCommand>
{
    public static string PacketId => nameof(UpdateInventorySlotQuantityCommand);

    public static UpdateInventorySlotQuantityCommand ReadFrom(PacketReader reader)
    {
        return new UpdateInventorySlotQuantityCommand(
            SlotIndex: reader.ReadInt32(),
            Quantity: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(SlotIndex);
        writer.WriteInt32(Quantity);
    }
}