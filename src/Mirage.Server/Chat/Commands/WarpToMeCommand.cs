using Microsoft.Extensions.Logging;
using Mirage.Server.Players;
using Mirage.Shared.Constants;
using Mirage.Shared.Data;

namespace Mirage.Server.Chat.Commands;

public sealed class WarpToMeCommand(ILogger<WarpToMeCommand> logger, IPlayerService players) : Command(ChatCommandNames.WarpToMe, AccessLevel.Mapper)
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
            player.Tell("You cannot warp yourself to yourself!", ColorCode.White);
            return;
        }

        targetPlayer.WarpTo(player.Map, player.Character.X, player.Character.Y);

        logger.LogInformation("{CharacterName} has warped {TargetCharacterName} to self [Map: {MapName}]",
            player.Character.Name,
            targetPlayer.Character.Name,
            player.Character.Map);

        targetPlayer.Tell($"You have been summoned by {player.Character.Name}.", ColorCode.BrightBlue);

        player.Tell($"{targetPlayer.Character.Name} has been summoned.", ColorCode.BrightBlue);
    }
}