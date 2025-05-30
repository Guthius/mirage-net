using Mirage.Shared.Data;

namespace Mirage.Server.Players;

public sealed record PlayerInventorySlot
{
    public required ItemInfo Item { get; set; }
    public int Quantity { get; set; }
    public int Durability { get; set; }
}