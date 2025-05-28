using Mirage.Server.Players;
using Mirage.Server.Repositories;
using Mirage.Shared.Constants;
using Mirage.Shared.Data;

namespace Mirage.Server.Chat.Commands;

public sealed class DestroyBanListCommand() : Command(ChatCommandNames.DestroyBanList, AccessLevel.Administrator)
{
    public override void Execute(Player player, ReadOnlySpan<char> args)
    {
        BanRepository.Clear();

        player.Tell("Ban list destroyed.", ColorCode.White);
    }
}