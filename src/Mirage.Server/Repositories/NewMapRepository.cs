using System.Diagnostics;
using Mirage.Server.Assets;
using Mirage.Shared.Data;
using Serilog;
using TiledSharp;

namespace Mirage.Server.Repositories;

public static class NewMapRepository
{
    private static class Properties
    {
        public const string Name = "name";
        public const string PvpEnabled = "pvp_enabled";
    }

    private static readonly Dictionary<string, NewMapInfo> Maps = new(StringComparer.OrdinalIgnoreCase);

    public static IEnumerable<KeyValuePair<string, NewMapInfo>> All()
    {
        return Maps;
    }

    public static void Load()
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            foreach (var path in Directory.GetFiles("Content", "*.tmx"))
            {
                var mapInfo = LoadMapInfo(path);
                if (mapInfo is null)
                {
                    continue;
                }

                BuildCacheEntry(mapInfo, path);
            }
        }
        finally
        {
            stopwatch.Stop();

            Log.Information("Loaded {Count} maps in {ElapsedMs}ms", Maps.Count, stopwatch.ElapsedMilliseconds);
        }
    }
    
    private static NewMapInfo? LoadMapInfo(string path)
    {
        try
        {
            var map = new TmxMap(path);
            var mapInfo = new NewMapInfo
            {
                Id = AssetManager.ComputeHash(path),
                Name = map.Properties.GetValueOrDefault(Properties.Name, string.Empty),
                PvpEnabled = map.Properties.GetValueOrDefault(Properties.PvpEnabled) == "true",
                TileWidth = map.TileWidth,
                TileHeight = map.TileHeight,
                Width = map.Width,
                Height = map.Height,
                Tilesets = LoadTilesets(map).ToList(),
                Layers = LoadLayers(map).ToList(),
                Tiles = LoadTiles(map)
            };

            return Maps[Path.GetFileName(path)] = mapInfo;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to load map {Path}", path);

            return null;
        }
    }

    private static void BuildCacheEntry(NewMapInfo map, string path)
    {
        path += ".cache";
        
        using var stream = File.OpenWrite(path);

        map.WriteTo(stream);

        AssetManager.Register(map.Id, path);
    }

    private static IEnumerable<TilesetInfo> LoadTilesets(TmxMap tmxMap)
    {
        foreach (var tileset in tmxMap.Tilesets)
        {
            var asset = AssetManager.Register(tileset.Image.Source);

            if (asset is not null)
            {
                yield return new TilesetInfo
                {
                    Id = asset.Id,
                    FirstGid = tileset.FirstGid,
                    TileWidth = tileset.TileWidth,
                    TileHeight = tileset.TileHeight
                };
            }
        }
    }

    private static IEnumerable<MapLayerInfo> LoadLayers(TmxMap tmxMap)
    {
        foreach (var layer in tmxMap.Layers)
        {
            if (layer.Name.Equals("Meta", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var drawOverActors = false;
            if (layer.Properties.TryGetValue("draw_over_actors", out var value))
            {
                drawOverActors = value == "true";
            }

            yield return new MapLayerInfo
            {
                DrawOverActors = drawOverActors,
                Tiles = layer.Tiles.Select(x => x.Gid).ToArray()
            };
        }
    }

    private static TileType GetType(string type)
    {
        return type.ToLowerInvariant() switch
        {
            "wall" => TileType.Blocked,
            _ => TileType.Walkable
        };
    }

    private static NewTileInfo[] LoadTiles(TmxMap tmxMap)
    {
        var tiles = new NewTileInfo[tmxMap.Width * tmxMap.Height];

        var tileset = tmxMap.Tilesets.FirstOrDefault(x => x.Name.Equals("Meta", StringComparison.OrdinalIgnoreCase));
        if (tileset is null)
        {
            return tiles;
        }

        foreach (var layer in tmxMap.Layers)
        {
            if (!layer.Name.Equals("Meta", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            foreach (var layerTile in layer.Tiles)
            {
                var index = layerTile.Y * tmxMap.Width + layerTile.X;
                if (index >= tiles.Length)
                {
                    continue;
                }

                var tilesetTile = tileset.Tiles.GetValueOrDefault(layerTile.Gid - tileset.FirstGid);
                if (tilesetTile is null || !tilesetTile.Properties.TryGetValue("type", out var type))
                {
                    continue;
                }

                tiles[index].Type = GetType(type);
            }
        }

        return tiles;
    }
}