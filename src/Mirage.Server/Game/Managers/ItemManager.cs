using System.Diagnostics;
using Mirage.Game.Constants;
using Mirage.Game.Data;
using Mirage.Server.Modules;
using MongoDB.Driver;
using Serilog;

namespace Mirage.Server.Game.Managers;

public static class ItemManager
{
    private static IMongoCollection<ItemInfo> GetCollection()
    {
        return DatabaseManager.GetCollection<ItemInfo>("items");
    }

    public static ItemInfo? Get(int itemId)
    {
        if (itemId is <= 0 or > Limits.MaxItems)
        {
            return null;
        }

        return modTypes.Items[itemId];
    }

    public static void Update(int itemId, ItemInfo itemInfo)
    {
        if (itemId is <= 0 or > Limits.MaxItems)
        {
            return;
        }

        modTypes.Items[itemId] = itemInfo;

        Save(itemId);
    }

    private static void Save(int itemId)
    {
        GetCollection().ReplaceOne(x => x.Id == itemId, modTypes.Items[itemId], new ReplaceOptions
        {
            IsUpsert = true
        });
    }

    public static void Load()
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var itemInfos = GetCollection()
                .Find(Builders<ItemInfo>.Filter.Empty)
                .ToList();

            for (var itemId = 1; itemId <= Limits.MaxItems; itemId++)
            {
                modTypes.Items[itemId] = itemInfos.FirstOrDefault(x => x.Id == itemId) ?? CreateItem(itemId);
            }
        }
        finally
        {
            stopwatch.Stop();

            Log.Information("Loaded {Count} items in {ElapsedMs}ms", modTypes.Items.Length, stopwatch.ElapsedMilliseconds);
        }

        return;

        static ItemInfo CreateItem(int itemId)
        {
            return new ItemInfo
            {
                Id = itemId
            };
        }
    }
}