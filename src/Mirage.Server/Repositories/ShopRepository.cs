using System.Diagnostics;
using Mirage.Shared.Constants;
using Mirage.Shared.Data;
using MongoDB.Driver;
using Serilog;

namespace Mirage.Server.Repositories;

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