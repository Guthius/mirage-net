using Microsoft.Xna.Framework.Graphics;
using Mirage.Game.Data;

namespace Mirage.Client.Game;

public static class MapRenderer
{
    public static void DrawMap(SpriteBatch spriteBatch, NewMapInfo mapInfo)
    {
        foreach (var layer in mapInfo.Layers)
        {
            DrawMapLayer(spriteBatch, mapInfo, layer.Tiles);
        }
    }

    private static void DrawMapLayer(SpriteBatch spriteBatch, NewMapInfo mapInfo, int[] tiles)
    {
        for (var y = 0; y < mapInfo.Height; y++)
        {
            for (var x = 0; x < mapInfo.Width; x++)
            {
                var index = y * mapInfo.Width + x;
                var tile = tiles[index];
                if (tile == 0)
                {
                    continue;
                }
            }
        }
    }
}