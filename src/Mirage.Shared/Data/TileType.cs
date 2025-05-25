namespace Mirage.Shared.Data;

[Flags]
public enum TileType
{
    Walkable = 0,
    Blocked = 1,
    Warp = 2,
    Item = 4,
    NpcAvoid = 8,
    Key = 16,
    KeyOpen = 32
}