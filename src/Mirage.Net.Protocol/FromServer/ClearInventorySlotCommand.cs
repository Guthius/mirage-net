namespace Mirage.Net.Protocol.FromServer;

public sealed record ClearInventorySlotCommand(int Slot) : IPacket<ClearInventorySlotCommand>
{
    public static string PacketId => nameof(ClearInventorySlotCommand);

    public static ClearInventorySlotCommand ReadFrom(PacketReader reader)
    {
        return new ClearInventorySlotCommand(Slot: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(Slot);
    }
}