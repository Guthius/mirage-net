using Microsoft.Extensions.Logging;
using Mirage.Net.Protocol.FromServer;
using Mirage.Server.Players;
using Mirage.Shared.Constants;
using Mirage.Shared.Data;

namespace Mirage.Server.Chat.Commands;

public sealed class KickCommand(ILogger<KickCommand> logger, IPlayerService players) : Command(ChatCommandNames.Kick, AccessLevel.Moderator)
{
    public override void Execute(Player player, ReadOnlySpan<char> args)
    {
        if (args.IsEmpty)
        {
            return;
        }

        var targetPlayer = players.Find(args);
        if (targetPlayer is null)
        {
            player.Tell("Player is not online.", ColorCode.White);
            return;
        }

        if (targetPlayer == player)
        {
            player.Tell("You cannot kick yourself!", ColorCode.White);
            return;
        }

        if (targetPlayer.Character.AccessLevel > player.Character.AccessLevel)
        {
            player.Tell("That is a higher access admin then you!", ColorCode.White);
            return;
        }

        players.Send(new ChatCommand($"{targetPlayer.Character.Name} has been kicked by {player.Character.Name}!", ColorCode.White));

        logger.LogInformation("{CharacterName} has kicked {TargetCharacterName}.", player.Character.Name, targetPlayer.Character.Name);

        targetPlayer.Disconnect($"You have been kicked by {player.Character.Name}!");
    }
}