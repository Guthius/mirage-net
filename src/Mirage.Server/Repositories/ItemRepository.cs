using System.Diagnostics;
using System.Runtime.CompilerServices;
using Mirage.Shared.Constants;
using Mirage.Shared.Data;
using MongoDB.Driver;
using Serilog;

namespace Mirage.Server.Repositories;

public static class ItemRepository
{
    private static readonly ItemInfo[] Items = new ItemInfo[Limits.MaxItems + 1];

    private static IMongoCollection<ItemInfo> GetCollection()
    {
        return Database.GetCollection<ItemInfo>("items");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ItemInfo? Get(int itemId)
    {
        if (itemId is <= 0 or > Limits.MaxItems)
        {
            return null;
        }

        return Items[itemId];
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
                Items[itemId] = itemInfos.FirstOrDefault(x => x.Id == itemId) ?? CreateItem(itemId);
            }
        }
        finally
        {
            stopwatch.Stop();

            Log.Information("Loaded {Count} items in {ElapsedMs}ms", Items.Length, stopwatch.ElapsedMilliseconds);
        }

        static ItemInfo CreateItem(int itemId)
        {
            return new ItemInfo
            {
                Id = itemId
            };
        }
    }
}