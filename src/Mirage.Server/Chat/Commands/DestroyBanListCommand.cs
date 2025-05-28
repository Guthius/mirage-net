using Mirage.Server.Players;
using Mirage.Server.Repositories.Bans;
using Mirage.Shared.Constants;
using Mirage.Shared.Data;

namespace Mirage.Server.Chat.Commands;

public sealed class DestroyBanListCommand(IBanRepository banRepository) : Command(ChatCommandNames.DestroyBanList, AccessLevel.Administrator)
{
    public override void Execute(Player player, ReadOnlySpan<char> args)
    {
        banRepository.ClearAll();

        player.Tell("Ban list destroyed.", ColorCode.White);
    }
}