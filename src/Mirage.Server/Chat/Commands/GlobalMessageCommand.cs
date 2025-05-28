using Mirage.Net.Protocol.FromServer.New;
using Mirage.Server.Players;
using Mirage.Shared.Constants;
using Mirage.Shared.Data;

namespace Mirage.Server.Chat.Commands;

public sealed class GlobalMessageCommand(IPlayerService players) : Command(ChatCommandNames.GlobalMessage, AccessLevel.Moderator)
{
    public override void Execute(Player player, ReadOnlySpan<char> args)
    {
        if (!args.IsEmpty)
        {
            players.Send(new ChatCommand($"(global) {player.Character.Name}: {args}", ColorCode.GlobalColor));
        }
    }
}