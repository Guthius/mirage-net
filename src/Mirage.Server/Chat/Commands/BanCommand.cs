using Microsoft.Extensions.Logging;
using Mirage.Net.Protocol.FromServer;
using Mirage.Server.Players;
using Mirage.Server.Repositories.Bans;
using Mirage.Shared.Constants;
using Mirage.Shared.Data;

namespace Mirage.Server.Chat.Commands;

public sealed class BanCommand(ILogger<BanCommand> logger, IPlayerService players, IBanRepository banRepository) : Command(ChatCommandNames.Ban, AccessLevel.Mapper)
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
            player.Tell("You cannot ban yourself!", ColorCode.White);
            return;
        }

        if (targetPlayer.Character.AccessLevel > player.Character.AccessLevel)
        {
            player.Tell("That is a higher access admin then you!", ColorCode.White);
            return;
        }

        banRepository.AddBan(targetPlayer.Address, player.Character.Name);

        players.Send(new ChatCommand($"{targetPlayer.Character.Name} has been banned by {player.Character.Name}!", ColorCode.White));

        logger.LogInformation("{CharacterName} has banned {BannedCharacterName}",
            targetPlayer.Character.Name, player.Character);

        targetPlayer.Disconnect($"You have been banned by {player.Character.Name}!");
    }
}