using Mirage.Server.Players;
using Mirage.Shared.Constants;
using Mirage.Shared.Data;

namespace Mirage.Server.Chat.Commands;

public sealed class PartyCommand(IPlayerService players) : Command(ChatCommandNames.Party, AccessLevel.None)
{
    public override void Execute(Player player, ReadOnlySpan<char> args)
    {
        if (args.IsEmpty)
        {
            player.Tell("Usage: /party playernamehere", ColorCodes.AlertColor);
            return;
        }

        if (player.InParty)
        {
            player.Tell("You are already in a party!", ColorCodes.Pink);
            return;
        }

        var targetPlayer = players.Find(args);
        if (targetPlayer is null)
        {
            player.Tell("Player is not online.", ColorCodes.White);
            return;
        }

        if (player.Character.AccessLevel > AccessLevel.Moderator)
        {
            player.Tell("You can't join a party, you are an admin!", ColorCodes.Blue);
            return;
        }

        if (targetPlayer.Character.AccessLevel > AccessLevel.Moderator)
        {
            player.Tell("Admins cannot join parties!", ColorCodes.Blue);
            return;
        }

        var levelDifference = Math.Abs(player.Character.Level - targetPlayer.Character.Level);
        if (levelDifference > 5)
        {
            player.Tell("There is more then a 5 level gap between you two, party failed.", ColorCodes.Pink);
            return;
        }

        if (targetPlayer.InParty)
        {
            player.Tell("Player is already in a party!", ColorCodes.Pink);
            return;
        }

        player.Tell($"Party request has been sent to {targetPlayer.Character.Name}.", ColorCodes.Pink);
        targetPlayer.Tell($"{player.Character.Name} wants you to join their party.  Type /join to join, or /leave to decline.", ColorCodes.Pink);

        player.IsPartyStarter = true;
        player.PartyMember = targetPlayer;

        targetPlayer.PartyMember = player;
    }
}