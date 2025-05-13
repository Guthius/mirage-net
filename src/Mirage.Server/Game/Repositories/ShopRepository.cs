using System.Diagnostics;
using Mirage.Game.Constants;
using Mirage.Game.Data;
using MongoDB.Driver;
using Serilog;

namespace Mirage.Server.Game.Repositories;

public static class ShopRepository
{
    private static readonly ShopInfo[] Shops = new ShopInfo[Limits.MaxShops + 1];

    private static IMongoCollection<ShopInfo> GetCollection()
    {
        return Database.GetCollection<ShopInfo>("shops");
    }

    public static ShopInfo? Get(int shopId)
    {
        if (shopId is <= 0 or > Limits.MaxShops)
        {
            return null;
        }

        return Shops[shopId];
    }

    public static void Update(int shopId, ShopInfo shopInfo)
    {
        if (shopId is <= 0 or > Limits.MaxShops)
        {
            return;
        }

        Shops[shopId] = shopInfo;

        Save(shopId);
    }

    private static void Save(int shopId)
    {
        GetCollection().ReplaceOne(x => x.Id == shopId, Shops[shopId], new ReplaceOptions
        {
            IsUpsert = true
        });
    }

    public static void Load()
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var shopInfos = GetCollection()
                .Find(Builders<ShopInfo>.Filter.Empty)
                .ToList();

            for (var shopId = 1; shopId <= Limits.MaxShops; shopId++)
            {
                Shops[shopId] = shopInfos.FirstOrDefault(x => x.Id == shopId) ?? CreateSpell(shopId);
            }
        }
        finally
        {
            stopwatch.Stop();

            Log.Information("Loaded {Count} shops in {ElapsedMs}ms", Shops.Length, stopwatch.ElapsedMilliseconds);
        }

        static ShopInfo CreateSpell(int shopId)
        {
            return new ShopInfo
            {
                Id = shopId
            };
        }
    }
}