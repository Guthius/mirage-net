using Mirage.Server.Repositories.Maps;

namespace Mirage.Server.Maps;

public sealed class MapService : IMapService
{
    private readonly Dictionary<string, Map> _maps = new(StringComparer.OrdinalIgnoreCase);

    public MapService(IMapRepository mapRepository, IServiceProvider services)
    {
        var mapInfos = mapRepository.Load();

        foreach (var (fileName, mapInfo) in mapInfos)
        {
            _maps[fileName] = new Map(fileName, mapInfo, services);
        }
    }

    public void Update(float dt)
    {
        foreach (var map in _maps.Values)
        {
            map.Update(dt);
        }
    }

    public Map? GetByName(string mapName)
    {
        return _maps.GetValueOrDefault(mapName);
    }
}