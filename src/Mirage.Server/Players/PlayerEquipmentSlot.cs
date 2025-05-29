using Mirage.Shared.Data;

namespace Mirage.Server.Players;

public sealed class PlayerEquipmentSlot
{
    public required ItemInfo Item { get; set; }
    public int Durability { get; set; }
}