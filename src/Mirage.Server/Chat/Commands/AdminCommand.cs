using Mirage.Server.Players;
using Mirage.Shared.Constants;
using Mirage.Shared.Data;

namespace Mirage.Server.Chat.Commands;

public sealed class AdminCommand() : Command(ChatCommandNames.Admin, AccessLevel.Moderator)
{
    public override void Execute(Player player, ReadOnlySpan<char> args)
    {
        player.Tell("Social Commands:", ColorCodes.HelpColor);
        player.Tell($"/{ChatCommandNames.GlobalMessage} msghere = Global Admin Message", ColorCodes.HelpColor);
        player.Tell($"/{ChatCommandNames.AdminMessage} msghere = Private Admin Message", ColorCodes.HelpColor);

        player.Tell("Available Commands: " +
                    $"/{ChatCommandNames.Admin}, " +
                    $"/{ChatCommandNames.Location}, " +
                    $"/{ChatCommandNames.WarpMeTo}, " +
                    $"/{ChatCommandNames.WarpToMe}, " +
                    $"/{ChatCommandNames.WarpTo}, " +
                    $"/{ChatCommandNames.SetSprite}, " +
                    $"/{ChatCommandNames.Kick}, " +
                    $"/{ChatCommandNames.Ban}, " +
                    $"/{ChatCommandNames.SetMotd}, " +
                    $"/{ChatCommandNames.Ban}",
            ColorCodes.HelpColor);
    }
}