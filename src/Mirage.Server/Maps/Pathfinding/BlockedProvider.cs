using AStarNavigator;
using AStarNavigator.Providers;

namespace Mirage.Server.Maps.Pathfinding;

public sealed class BlockedProvider(Map map) : IBlockedProvider
{
    public bool IsBlocked(Tile coord)
    {
        return !map.IsPassable((int) coord.X, (int) coord.Y);
    }
}