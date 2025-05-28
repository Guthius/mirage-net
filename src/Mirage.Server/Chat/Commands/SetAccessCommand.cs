using Microsoft.Extensions.Logging;
using Mirage.Net.Protocol.FromServer.New;
using Mirage.Server.Players;
using Mirage.Shared.Constants;
using Mirage.Shared.Data;

namespace Mirage.Server.Chat.Commands;

public sealed class SetAccessCommand(ILogger<SetAccessCommand> logger, IPlayerService players) : Command(ChatCommandNames.SetAccess, AccessLevel.Administrator)
{
    public override void Execute(Player player, ReadOnlySpan<char> args)
    {
        var space = args.IndexOf(' ');
        if (space == -1)
        {
            return;
        }

        if (int.TryParse(args[..space], out var accessLevel) || !Enum.IsDefined(typeof(AccessLevel), accessLevel))
        {
            return;
        }

        var targetName = args[space..].Trim();
        if (targetName.IsEmpty)
        {
            return;
        }

        var targetPlayer = players.Find(targetName);
        if (targetPlayer is null)
        {
            player.Tell("Player is not online.", ColorCode.White);
            return;
        }

        if (targetPlayer.Character.AccessLevel <= AccessLevel.None)
        {
            players.Send(new ChatCommand($"{targetPlayer.Character.Name} has been blessed with administrative access.", ColorCode.BrightBlue));
        }

        targetPlayer.Character.AccessLevel = (AccessLevel) accessLevel;

        logger.LogInformation("{CharacterName} has modified {TargetCharacterName}'s access..", player.Character.Name, targetPlayer.Character.Name);

        targetPlayer.SendPlayerData();
    }
}