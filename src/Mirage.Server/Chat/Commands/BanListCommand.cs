using Mirage.Server.Players;
using Mirage.Server.Repositories.Bans;
using Mirage.Shared.Constants;
using Mirage.Shared.Data;

namespace Mirage.Server.Chat.Commands;

public sealed class BanListCommand(IBanRepository banRepository) : Command(ChatCommandNames.BanList, AccessLevel.Mapper)
{
    public override void Execute(Player player, ReadOnlySpan<char> args)
    {
        var banInfos = banRepository.GetAll();
        if (banInfos.Count == 0)
        {
            return;
        }

        var lineNumber = 1;

        foreach (var banInfo in banInfos)
        {
            player.Tell($"{lineNumber}: Banned IP {banInfo.Ip} by {banInfo.BannedBy}", ColorCode.White);

            lineNumber++;
        }
    }
}