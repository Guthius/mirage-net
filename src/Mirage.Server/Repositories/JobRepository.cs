using System.Diagnostics;
using Mirage.Shared.Data;
using MongoDB.Driver;
using Serilog;

namespace Mirage.Server.Repositories;

public static class JobRepository
{
    private static List<JobInfo> Jobs { get; set; } = [];
    
    private static IMongoCollection<JobInfo> GetCollection()
    {
        return Database.GetCollection<JobInfo>("classes");
    }

    public static JobInfo? Get(string classId)
    {
        return Jobs.Find(classInfo => classInfo.Id == classId);
    }

    public static List<JobInfo> GetAll()
    {
        return Jobs;
    }

    public static string GetName(string classId)
    {
        return Jobs.Find(classInfo => classInfo.Id == classId)?.Name ?? string.Empty;
    }

    public static void Load()
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            CreateDefaultClasses();

            var classInfos = GetCollection()
                .Find(Builders<JobInfo>.Filter.Empty)
                .ToList();

            Jobs = classInfos;
        }
        finally
        {
            stopwatch.Stop();

            Log.Information("Loaded {Count} classes in {ElapsedMs}ms", Jobs.Count, stopwatch.ElapsedMilliseconds);
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

    private static IEnumerable<JobInfo> GetDefaultClasses()
    {
        yield return new JobInfo
        {
            Name = "Knight",
            Sprite = 0,
            Strength = 8,
            Defense = 7,
            Speed = 5,
            Intelligence = 0
        };

        yield return new JobInfo
        {
            Name = "Black Mage",
            Sprite = 1,
            Strength = 2,
            Defense = 2,
            Speed = 3,
            Intelligence = 13
        };
        
        yield return new JobInfo
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