using System.Diagnostics;
using System.Runtime.CompilerServices;
using Mirage.Shared.Constants;
using Mirage.Shared.Data;
using MongoDB.Driver;
using Serilog;

namespace Mirage.Server.Repositories;

public static class MapRepository
{
    private static readonly MapInfo[] Maps = new MapInfo[Limits.MaxMaps + 1];

    private static IMongoCollection<MapInfo> GetCollection()
    {
        return Database.GetCollection<MapInfo>("maps");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MapInfo? Get(int mapId)
    {
        if (mapId <= 0)
        {
            return null;
        }

        return Maps[mapId];
    }

    public static void Update(int mapId, MapInfo mapInfo)
    {
        var currentMapInfo = Get(mapId);
        if (currentMapInfo is null)
        {
            return;
        }

        Maps[mapId] = mapInfo with
        {
            Revision = currentMapInfo.Revision + 1
        };

        Save(mapId);
    }

    private static void Save(int mapId)
    {
        GetCollection().ReplaceOne(x => x.Id == mapId, Maps[mapId], new ReplaceOptions
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
                Maps[mapId] = mapInfos.FirstOrDefault(x => x.Id == mapId) ?? CreateMap(mapId);
            }
        }
        finally
        {
            stopwatch.Stop();

            Log.Information("Loaded {Count} maps in {ElapsedMs}ms", Maps.Length, stopwatch.ElapsedMilliseconds);
        }

        static MapInfo CreateMap(int mapId)
        {
            return new MapInfo
            {
                Id = mapId
            };
        }
    }

    public static List<string> GetFreeMapRanges()
    {
        var start = 1;
        var end = 1;

        var ranges = new List<string>();

        for (var id = 1; id <= Limits.MaxMaps; id++)
        {
            if (string.IsNullOrWhiteSpace(Maps[id].Name))
            {
                end++;
            }
            else
            {
                if (end - start > 0)
                {
                    ranges.Add($"{start}-{end - 1}");
                }

                start = id + 1;
                end = id + 1;
            }
        }

        ranges.Add($"{start}-{end - 1}");

        return ranges;
    }
}