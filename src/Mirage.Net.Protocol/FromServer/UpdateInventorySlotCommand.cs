using Mirage.Shared.Data;

namespace Mirage.Net.Protocol.FromServer;

public sealed record UpdateInventorySlotCommand(int SlotIndex, ItemType Type, int Sprite, string ItemName, int Quantity) : IPacket<UpdateInventorySlotCommand>
{
    public static string PacketId => nameof(UpdateInventorySlotCommand);

    public static UpdateInventorySlotCommand ReadFrom(PacketReader reader)
    {
        return new UpdateInventorySlotCommand(
            SlotIndex: reader.ReadInt32(),
            Sprite: reader.ReadInt32(),
            Type: reader.ReadEnum<ItemType>(),
            ItemName: reader.ReadString(),
            Quantity: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(SlotIndex);
        writer.WriteInt32(Sprite);
        writer.WriteEnum(Type);
        writer.WriteString(ItemName);
        writer.WriteInt32(Quantity);
    }
}