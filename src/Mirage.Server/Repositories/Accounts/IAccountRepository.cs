namespace Mirage.Server.Repositories.Accounts;

public interface IAccountRepository
{
    bool Exists(string accountName);
    AccountInfo Create(string accountName, string password);
    AccountInfo? Authenticate(string accountName, string password);
    void Delete(string accountId);
}