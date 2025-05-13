using System.Diagnostics;
using Mirage.Game.Data;
using MongoDB.Driver;
using Serilog;

namespace Mirage.Server.Game.Repositories;

public static class ClassRepository
{
    private static List<ClassInfo> Classes { get; set; } = [];
    
    private static IMongoCollection<ClassInfo> GetCollection()
    {
        return Database.GetCollection<ClassInfo>("classes");
    }

    public static ClassInfo? Get(int classId)
    {
        if (classId < 0 || classId >= Classes.Count)
        {
            return null;
        }
        
        return Classes[classId];
    }

    public static IReadOnlyList<ClassInfo> GetAll()
    {
        return Classes;
    }

    public static string GetName(int classId)
    {
        if (classId < 0 || classId >= Classes.Count)
        {
            return string.Empty;
        }

        return Classes[classId].Name;
    }

    public static void Load()
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            CreateDefaultClasses();

            var classInfos = GetCollection()
                .Find(Builders<ClassInfo>.Filter.Empty)
                .ToList();

            Classes = classInfos;
        }
        finally
        {
            stopwatch.Stop();

            Log.Information("Loaded {Count} classes in {ElapsedMs}ms", Classes.Count, stopwatch.ElapsedMilliseconds);
        }
    }

    private static void CreateDefaultClasses()
    {
        var count = GetCollection().CountDocuments(x => true);
        if (count > 0)
        {
            return;
        }
        
        GetCollection().InsertMany(GetDefaultClasses());
    }

    private static IEnumerable<ClassInfo> GetDefaultClasses()
    {
        yield return new ClassInfo
        {
            Name = "Knight",
            Sprite = 0,
            Strength = 8,
            Defense = 7,
            Speed = 5,
            Intelligence = 0
        };

        yield return new ClassInfo
        {
            Name = "Black Mage",
            Sprite = 1,
            Strength = 2,
            Defense = 2,
            Speed = 3,
            Intelligence = 13
        };
        
        yield return new ClassInfo
        {
            Name = "Monk",
            Sprite = 30,
            Strength = 8,
            Defense = 5,
            Speed = 7,
            Intelligence = 8
        };
    }
}