using Mirage.Net.Protocol.FromServer.New;
using Mirage.Server.Players;
using Mirage.Shared.Constants;
using Mirage.Shared.Data;

namespace Mirage.Server.Chat.Commands;

public sealed class BroadcastCommand(IPlayerService players) : Command(ChatCommandNames.Broadcast, AccessLevel.None)
{
    public override void Execute(Player player, ReadOnlySpan<char> args)
    {
        if (!args.IsEmpty)
        {
            players.Send(new ChatCommand($"{player.Character.Name}: {args}", ColorCode.BroadcastColor));
        }
    }
}