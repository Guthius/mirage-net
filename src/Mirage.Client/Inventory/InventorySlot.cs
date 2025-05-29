using Mirage.Shared.Data;

namespace Mirage.Client.Inventory;

public sealed record InventorySlot(ItemType Type, int Sprite, string ItemName)
{
    public int Quantity { get; set; }
}