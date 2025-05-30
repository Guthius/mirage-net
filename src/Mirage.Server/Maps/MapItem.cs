using Mirage.Shared.Data;

namespace Mirage.Server.Maps;

public sealed record MapItem(int Id, ItemInfo Info, int Durability, int Quantity, int X, int Y, bool Expires)
{
    public float LifeTime { get; set; }
}