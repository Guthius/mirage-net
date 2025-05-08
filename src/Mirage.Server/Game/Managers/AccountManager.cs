using Mirage.Game.Data;
using MongoDB.Driver;
using Serilog;

namespace Mirage.Server.Game.Managers;

public static class AccountManager
{
    private static IMongoCollection<AccountInfo> GetCollection()
    {
        return DatabaseManager.GetCollection<AccountInfo>("accounts");
    }

    public static bool Exists(string accountName)
    {
        var count = GetCollection().CountDocuments(x => x.Name == accountName);

        return count > 0;
    }

    public static void Create(string accountName, string password)
    {
        GetCollection().InsertOne(new AccountInfo
        {
            Name = accountName,
            Password = BCrypt.Net.BCrypt.HashPassword(password)
        });
    }

    public static AccountInfo? Authenticate(string accountName, string password)
    {
        var accountInfo = GetCollection().Find(x => x.Name == accountName).FirstOrDefault();

        if (accountInfo is null || !BCrypt.Net.BCrypt.Verify(password, accountInfo.Password))
        {
            return null;
        }

        return accountInfo;
    }

    public static void Delete(string accountId)
    {
        var result = GetCollection().DeleteOne(x => x.Id == accountId);

        if (result.DeletedCount == 0)
        {
            return;
        }

        CharacterManager.DeleteForAccount(accountId);

        Log.Information("Account '{AccountId}' has been deleted", accountId);
    }
}