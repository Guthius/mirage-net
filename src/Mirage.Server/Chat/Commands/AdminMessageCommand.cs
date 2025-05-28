using Mirage.Net.Protocol.FromServer.New;
using Mirage.Server.Players;
using Mirage.Shared.Constants;
using Mirage.Shared.Data;

namespace Mirage.Server.Chat.Commands;

public sealed class AdminMessageCommand(IPlayerService players) : Command(ChatCommandNames.AdminMessage, AccessLevel.Moderator)
{
    public override void Execute(Player player, ReadOnlySpan<char> args)
    {
        if (!args.IsEmpty)
        {
            players
                .Where(x => x.Character.AccessLevel > AccessLevel.None)
                .Send(new ChatCommand($"(admin {player.Character.Name}) {args}", ColorCode.AdminColor));
        }
    }
}