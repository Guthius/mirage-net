using Mirage.Server.Repositories;

namespace Mirage.Server.Maps;

public static class MapManager
{
    private static readonly Dictionary<string, Map> Maps = new(StringComparer.OrdinalIgnoreCase);

    public static void Initialize()
    {
        foreach (var (fileName, mapInfo) in NewMapRepository.All())
        {
            Maps[fileName] = new Map(fileName, mapInfo);
        }
    }

    public static void Update(float dt)
    {
        foreach (var map in Maps.Values)
        {
            map.Update(dt);
        }
    }

    public static Map? GetByName(string mapName)
    {
        return Maps.GetValueOrDefault(mapName);
    }
}