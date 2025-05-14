using Mirage.Game.Constants;
using Mirage.Game.Data;
using Mirage.Server.Repositories;

namespace Mirage.Server.Game;

public static class MapManager
{
    private static readonly Dictionary<string, Map> Maps = new(StringComparer.OrdinalIgnoreCase);

    public static void Initialize()
    {
        foreach (var mapInfo in NewMapRepository.GetAll())
        {
            Maps[mapInfo.Name] = new Map(mapInfo);
        }

        CreateStartMapIfNotExist();
    }

    public static void Update()
    {
        foreach (var map in Maps.Values)
        {
            map.Update();
        }
    }
    
    private static void CreateStartMapIfNotExist()
    {
        if (Maps.ContainsKey(Options.StartMapName))
        {
            return;
        }

        Maps[Options.StartMapName] = new Map(new NewMapInfo
        {
            Name = Options.StartMapName,
            Revision = 1,
            Width = 50,
            Height = 50,
            Layers =
            [
                new MapLayerInfo
                {
                    Name = "Ground",
                    Tiles = new int[50 * 50]
                }
            ],
            Tiles = new NewTileInfo[50 * 50]
        });
    }

    public static Map? GetMap(string mapName)
    {
        return Maps.GetValueOrDefault(mapName);
    }
}