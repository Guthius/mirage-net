using Mirage.Shared.Constants;
using Mirage.Shared.Data;

namespace Mirage.Net.Protocol.FromServer;

public sealed record PlayerInventory(InventorySlotInfo[] Slots) : IPacket<PlayerInventory>
{
    public static string PacketId => "playerinv";

    public static PlayerInventory ReadFrom(PacketReader reader)
    {
        var slots = new InventorySlotInfo[Limits.MaxInventory];

        for (var i = 0; i < Limits.MaxInventory; i++)
        {
            slots[i] = new InventorySlotInfo
            {
                ItemId = reader.ReadInt32(),
                Quantity = reader.ReadInt32(),
                Durability = reader.ReadInt32()
            };
        }

        return new PlayerInventory(slots);
    }

    public void WriteTo(PacketWriter writer)
    {
        foreach (var slot in Slots)
        {
            writer.WriteInt32(slot.ItemId);
            writer.WriteInt32(slot.Quantity);
            writer.WriteInt32(slot.Durability);
        }
    }
}