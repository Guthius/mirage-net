using Mirage.Server.Players;
using Mirage.Shared.Data;

namespace Mirage.Server.Chat.Commands;

public sealed class WhoCommand() : Command(ChatCommandNames.Who, AccessLevel.None)
{
    public override void Execute(Player player, ReadOnlySpan<char> args)
    {
        player.SendWhosOnline();
    }
}