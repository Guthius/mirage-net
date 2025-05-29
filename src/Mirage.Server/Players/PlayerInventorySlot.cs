namespace Mirage.Server.Players;

public sealed class PlayerInventorySlot
{
    public string ItemId { get; set; } = string.Empty;
    public int Quantity { get; set; }
}