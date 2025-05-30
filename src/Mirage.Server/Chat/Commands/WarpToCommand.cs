using Microsoft.Extensions.Logging;
using Mirage.Server.Maps;
using Mirage.Server.Players;
using Mirage.Shared.Constants;
using Mirage.Shared.Data;

namespace Mirage.Server.Chat.Commands;

public sealed class WarpToCommand(ILogger<WarpToCommand> logger, IMapService mapService) : Command(ChatCommandNames.WarpTo, AccessLevel.Mapper)
{
    public override void Execute(Player player, ReadOnlySpan<char> args)
    {
        if (args.IsEmpty)
        {
            return;
        }

        var map = mapService.GetByName(new string(args));
        if (map is null)
        {
            player.Tell("The specified map does not exist.", ColorCode.Red);
            return;
        }

        player.WarpTo(map, player.Character.X, player.Character.Y);
        player.Tell($"You have been warped to {map.Name}", ColorCode.BrightBlue);

        logger.LogInformation("{CharacterName} warped to {MapName}", player.Character.Name, map.FileName);
    }
}