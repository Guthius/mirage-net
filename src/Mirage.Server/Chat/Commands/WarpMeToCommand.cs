using Microsoft.Extensions.Logging;
using Mirage.Server.Players;
using Mirage.Shared.Constants;
using Mirage.Shared.Data;

namespace Mirage.Server.Chat.Commands;

public sealed class WarpMeToCommand(ILogger<WarpMeToCommand> logger, IPlayerService players) : Command(ChatCommandNames.WarpMeTo, AccessLevel.Mapper)
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
            player.Tell("You cannot warp to yourself!", ColorCode.White);
            return;
        }

        player.WarpTo(targetPlayer.Map, targetPlayer.Character.X, targetPlayer.Character.Y);

        logger.LogInformation("{CharacterName} has warped to {TargetCharacterName} [Map: {MapName}]",
            player.Character.Name,
            targetPlayer.Character.Name,
            targetPlayer.Character.Map);

        targetPlayer.Tell($"{player.Character.Name} has warped to you.", ColorCode.BrightBlue);

        player.Tell($"You have been warped to {targetPlayer.Character.Name}.", ColorCode.BrightBlue);
    }
}