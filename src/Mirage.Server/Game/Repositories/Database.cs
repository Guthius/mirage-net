using MongoDB.Driver;

namespace Mirage.Server.Game.Repositories;

public static class Database
{
    private const string ConnectionString = "mongodb://localhost/mirage";

    private static IMongoDatabase GetDatabase()
    {
        var mongoUrl = MongoUrl.Create(ConnectionString);
        var mongoClient = new MongoClient(mongoUrl);

        return mongoClient.GetDatabase(mongoUrl.DatabaseName);
    }
    
    public static IMongoCollection<T> GetCollection<T>(string collectionName)
    {
        return GetDatabase().GetCollection<T>(collectionName);
    }
}