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
            player.Tell("You are not in a party!", ColorCode.Pink);
            return;
        }

        if (player.InParty)
        {
            player.Tell("You have left the party.", ColorCode.Pink);
            player.PartyMember.Tell($"{player.Character.Name} has left the party.", ColorCode.Pink);
        }
        else
        {
            player.Tell("Declined party request.", ColorCode.Pink);
            player.PartyMember.Tell($"{player.Character.Name} declined your request.", ColorCode.Pink);
        }

        player.PartyMember.PartyMember = null;
        player.PartyMember.IsPartyStarter = false;
        player.PartyMember.InParty = false;

        player.PartyMember = null;
        player.IsPartyStarter = false;
        player.InParty = false;
    }
}