using AStarNavigator;
using AStarNavigator.Providers;

namespace Mirage.Server.Maps.Pathfinding;

public sealed class NeighborProvider(int width, int height) : INeighborProvider
{
    private static readonly double[,] Neighbors = new double[,]
    {
        {0, -1}, {1, 0}, {0, 1}, {-1, 0}
    };

    public IEnumerable<Tile> GetNeighbors(Tile tile)
    {
        for (var i = 0; i < Neighbors.GetLength(0); i++)
        {
            var x = tile.X + Neighbors[i, 0];
            var y = tile.Y + Neighbors[i, 1];

            if (x < 0 || x > width || y < 0 || y > height)
            {
                continue;
            }

            yield return new Tile(x, y);
        }
    }
}