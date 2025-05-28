namespace Mirage.Server.Repositories.Bans;

public interface IBanRepository
{
    List<BanInfo> GetAll();
    void AddBan(string ip, string bannedBy);
    bool IsBanned(string ip);
    void ClearAll();
}