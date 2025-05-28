using Microsoft.Extensions.Logging;
using Mirage.Server.Repositories.Characters;
using MongoDB.Driver;

namespace Mirage.Server.Repositories.Accounts;

public sealed class AccountRepository(ILogger<AccountRepository> logger, ICharacterRepository characterRepository) : IAccountRepository
{
    private static IMongoCollection<AccountInfo> GetCollection()
    {
        return Database.GetCollection<AccountInfo>("accounts");
    }

    public bool Exists(string accountName)
    {
        var count = GetCollection().CountDocuments(x => x.Name == accountName);

        return count > 0;
    }

    public AccountInfo Create(string accountName, string password)
    {
        var account = new AccountInfo
        {
            Name = accountName,
            Password = BCrypt.Net.BCrypt.HashPassword(password)
        };

        GetCollection().InsertOne(account);

        logger.LogInformation("Account {AccountName} has been created", accountName);

        return account;
    }

    public AccountInfo? Authenticate(string accountName, string password)
    {
        var accountInfo = GetCollection().Find(x => x.Name == accountName).FirstOrDefault();

        if (accountInfo is null || !BCrypt.Net.BCrypt.Verify(password, accountInfo.Password))
        {
            return null;
        }

        return accountInfo;
    }

    public void Delete(string accountId)
    {
        var result = GetCollection().DeleteOne(x => x.Id == accountId);

        if (result.DeletedCount == 0)
        {
            return;
        }

        characterRepository.DeleteForAccount(accountId);

        logger.LogInformation("Account {AccountId} has been deleted", accountId);
    }
}