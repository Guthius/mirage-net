using Mirage.Shared.Data;
using MongoDB.Driver;
using Serilog;

namespace Mirage.Server.Repositories;

public static class AccountRepository
{
    private static IMongoCollection<AccountInfo> GetCollection()
    {
        return Database.GetCollection<AccountInfo>("accounts");
    }

    public static bool Exists(string accountName)
    {
        var count = GetCollection().CountDocuments(x => x.Name == accountName);

        return count > 0;
    }

    public static AccountInfo Create(string accountName, string password)
    {
        var account = new AccountInfo
        {
            Name = accountName,
            Password = BCrypt.Net.BCrypt.HashPassword(password)
        };

        GetCollection().InsertOne(account);

        return account;
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

        CharacterRepository.DeleteForAccount(accountId);

        Log.Information("Account '{AccountId}' has been deleted", accountId);
    }
}