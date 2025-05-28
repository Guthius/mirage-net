using Microsoft.Extensions.Logging;
using Mirage.Server.Players;
using Mirage.Shared.Constants;
using Mirage.Shared.Data;

namespace Mirage.Server.Chat.Commands;

public sealed class WhisperCommand(ILogger<WhisperCommand> logger, IPlayerService players) : Command(ChatCommandNames.Whisper, AccessLevel.None)
{
    public override void Execute(Player player, ReadOnlySpan<char> args)
    {
        var space = args.IndexOf(' ');
        if (space == -1)
        {
            return;
        }

        var targetName = args[..space].Trim();
        var chatMesage = args[(space + 1)..].Trim();

        if (targetName.IsEmpty || chatMesage.IsEmpty)
        {
            player.Tell("Usage: !playername msghere", ColorCode.AlertColor);
            return;
        }

        var targetPlayer = players.Find(targetName);
        if (targetPlayer is null)
        {
            player.Tell("Player is not online.", ColorCode.White);
            return;
        }

        if (targetPlayer == player)
        {
            player.Map.SendMessage($"{player.Character.Name} begins to mumble to himself, what a wierdo...", ColorCode.Green);

            return;
        }

        logger.LogInformation("{FromCharacterName} tells {ToCharacterName}, '{Message}'", player.Character.Name, targetPlayer.Character.Name, new string(chatMesage));

        targetPlayer.Tell($"{player.Character.Name} tells you, '{chatMesage}'", ColorCode.TellColor);

        player.Tell($"You tell {targetPlayer.Character.Name}, '{chatMesage}'", ColorCode.TellColor);
    }
}