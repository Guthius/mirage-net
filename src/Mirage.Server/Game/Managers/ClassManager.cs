using System.Diagnostics;
using Mirage.Game.Data;
using Mirage.Server.Modules;
using MongoDB.Driver;
using Serilog;

namespace Mirage.Server.Game.Managers;

public static class ClassManager
{
    private static IMongoCollection<ClassInfo> GetCollection()
    {
        return DatabaseManager.GetCollection<ClassInfo>("classes");
    }

    public static string GetName(int classId)
    {
        if (classId < 0 || classId >= modTypes.Classes.Count)
        {
            return string.Empty;
        }

        return modTypes.Classes[classId].Name;
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

            modTypes.Classes = classInfos;
        }
        finally
        {
            stopwatch.Stop();

            Log.Information("Loaded {Count} classes in {ElapsedMs}ms", modTypes.Classes.Count, stopwatch.ElapsedMilliseconds);
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