using System.Diagnostics;
using Mirage.Game.Data;
using MongoDB.Driver;
using Serilog;

namespace Mirage.Server.Repositories;

public static class NewMapRepository
{
    private static readonly List<NewMapInfo> Maps = [];
    private static readonly Dictionary<string, NewMapInfo> MapsByName = new(StringComparer.OrdinalIgnoreCase);
    
    private static IMongoCollection<NewMapInfo> GetCollection()
    {
        return Database.GetCollection<NewMapInfo>("new_maps");
    }

    public static NewMapInfo? Get(string mapName)
    {
        return MapsByName.GetValueOrDefault(mapName);
    }

    public static IReadOnlyList<NewMapInfo> GetAll()
    {
        return Maps;
    }
    
    public static void Load()
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            Maps.Clear();
            Maps.AddRange(GetCollection()
                .Find(Builders<NewMapInfo>.Filter.Empty)
                .ToList());

            MapsByName.Clear();
            
            foreach (var map in Maps)
            {
                MapsByName[map.Name] = map;
            }
        }
        finally
        {
            stopwatch.Stop();

            Log.Information("Loaded {Count} maps in {ElapsedMs}ms", MapsByName.Count, stopwatch.ElapsedMilliseconds);
        }
    }
}