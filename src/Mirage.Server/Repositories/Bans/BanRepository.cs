using MongoDB.Driver;

namespace Mirage.Server.Repositories.Bans;

public sealed class BanRepository : IBanRepository
{
    private static IMongoCollection<BanInfo> GetCollection()
    {
        return Database.GetCollection<BanInfo>("banlist");
    }

    public List<BanInfo> GetAll()
    {
        return GetCollection().Find(Builders<BanInfo>.Filter.Empty).ToList();
    }

    public void AddBan(string ip, string bannedBy)
    {
        GetCollection().ReplaceOne(x => x.Ip == ip,
            new BanInfo
            {
                Ip = ip,
                BannedBy = bannedBy
            },
            new ReplaceOptions
            {
                IsUpsert = true
            });
    }

    public bool IsBanned(string ip)
    {
        return GetCollection().Find(x => x.Ip == ip).Any();
    }

    public void ClearAll()
    {
        GetCollection().DeleteMany(x => true);
    }
}