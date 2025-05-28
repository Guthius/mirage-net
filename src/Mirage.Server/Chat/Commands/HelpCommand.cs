using Mirage.Server.Players;
using Mirage.Shared.Constants;
using Mirage.Shared.Data;

namespace Mirage.Server.Chat.Commands;

public sealed class HelpCommand() : Command(ChatCommandNames.Help, AccessLevel.None)
{
    public override void Execute(Player player, ReadOnlySpan<char> args)
    {
        player.Tell("Social commands:", ColorCode.HelpColor);
        player.Tell($"/{ChatCommandNames.Broadcast} msghere = Broadcast Message", ColorCode.HelpColor);
        player.Tell($"/{ChatCommandNames.Emote} msghere = Emote Message", ColorCode.HelpColor);
        player.Tell($"/{ChatCommandNames.Whisper} namehere msghere = Player Message", ColorCode.HelpColor);

        player.Tell("Available Commands: " +
                    $"/{ChatCommandNames.Help}, " +
                    $"/{ChatCommandNames.Info}, " +
                    $"/{ChatCommandNames.Who}, " +
                    "/fps, " +
                    $"/{ChatCommandNames.Stats}, " +
                    "/train, " +
                    $"/{ChatCommandNames.Party}, " +
                    $"/{ChatCommandNames.Join}, " +
                    $"/{ChatCommandNames.Leave}",
            ColorCode.HelpColor);
    }
}