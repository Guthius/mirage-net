using Mirage.Game.Data;
using MongoDB.Driver;

namespace Mirage.Server.Repositories;

public static class BanRepository
{
    private static IMongoCollection<IpBanInfo> GetCollection()
    {
        return Database.GetCollection<IpBanInfo>("ip_bans");
    }

    public static IReadOnlyList<IpBanInfo> GetAll()
    {
        return GetCollection().Find(Builders<IpBanInfo>.Filter.Empty).ToList();
    }
    
    public static void AddBan(string ip, string bannedBy)
    {
        GetCollection().ReplaceOne(x => x.Ip == ip,
            new IpBanInfo
            {
                Ip = ip,
                BannedBy = bannedBy
            },
            new ReplaceOptions
            {
                IsUpsert = true
            });
    }

    public static bool IsBanned(string ip)
    {
        var filter = Builders<IpBanInfo>.Filter.Where(x => x.Ip == ip);

        return GetCollection().Find(filter).Any();
    }

    public static void Clear()
    {
        GetCollection().DeleteMany(x => true);
    }
}