using Mirage.Server.Players;
using Mirage.Shared.Constants;
using Mirage.Shared.Data;

namespace Mirage.Server.Chat.Commands;

public sealed class LeaveCommand() : Command(ChatCommandNames.Leave, AccessLevel.None)
{
    public override void Execute(Player player, ReadOnlySpan<char> args)
    {
        if (player.PartyMember is null)
        {
            player.Tell("You are not in a party!", ColorCodes.Pink);
            return;
        }

        if (player.InParty)
        {
            player.Tell("You have left the party.", ColorCodes.Pink);
            player.PartyMember.Tell($"{player.Character.Name} has left the party.", ColorCodes.Pink);
        }
        else
        {
            player.Tell("Declined party request.", ColorCodes.Pink);
            player.PartyMember.Tell($"{player.Character.Name} declined your request.", ColorCodes.Pink);
        }

        player.PartyMember.PartyMember = null;
        player.PartyMember.IsPartyStarter = false;
        player.PartyMember.InParty = false;

        player.PartyMember = null;
        player.IsPartyStarter = false;
        player.InParty = false;
    }
}