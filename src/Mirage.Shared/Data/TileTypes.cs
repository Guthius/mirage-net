namespace Mirage.Shared.Data;

[Flags]
public enum TileTypes
{
    None = 0,
    Blocked = 1,
    Warp = 2,
    Item = 4,
    NpcAvoid = 8,
    Key = 16,
    KeyOpen = 32
}