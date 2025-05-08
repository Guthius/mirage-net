using System.Diagnostics;
using Mirage.Game.Constants;
using Mirage.Game.Data;
using Mirage.Server.Modules;
using MongoDB.Driver;
using Serilog;

namespace Mirage.Server.Game.Managers;

public static class MapManager
{
    private static IMongoCollection<MapInfo> GetCollection()
    {
        return DatabaseManager.GetCollection<MapInfo>("maps");
    }

    public static MapInfo? Get(int mapId)
    {
        if (mapId <= 0)
        {
            return null;
        }

        return modTypes.Maps[mapId];
    }

    public static void Update(int mapId, MapInfo mapInfo)
    {
        var currentMapInfo = Get(mapId);
        if (currentMapInfo is null)
        {
            return;
        }

        modTypes.Maps[mapId] = mapInfo with
        {
            Revision = currentMapInfo.Revision + 1
        };

        Save(mapId);
    }

    private static void Save(int mapId)
    {
        GetCollection().ReplaceOne(x => x.Id == mapId, modTypes.Maps[mapId], new ReplaceOptions
        {
            IsUpsert = true
        });
    }

    public static void Load()
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var mapInfos = GetCollection()
                .Find(Builders<MapInfo>.Filter.Empty)
                .ToList();

            for (var mapId = 1; mapId <= Limits.MaxMaps; mapId++)
            {
                modTypes.Maps[mapId] = mapInfos.FirstOrDefault(x => x.Id == mapId) ?? CreateMap(mapId);
            }
        }
        finally
        {
            stopwatch.Stop();

            Log.Information("Loaded {Count} maps in {ElapsedMs}ms", modTypes.Maps.Length, stopwatch.ElapsedMilliseconds);
        }

        return;

        static MapInfo CreateMap(int mapId)
        {
            return new MapInfo
            {
                Id = mapId
            };
        }
    }
}