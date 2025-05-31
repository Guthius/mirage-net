using Mirage.Server.Players;
using Mirage.Shared.Constants;
using Mirage.Shared.Data;

namespace Mirage.Server.Chat.Commands;

public sealed class JoinCommand() : Command(ChatCommandNames.Join, AccessLevel.None)
{
    public override void Execute(Player player, ReadOnlySpan<char> args)
    {
        if (player.PartyMember is null || player.IsPartyStarter)
        {
            player.Tell("You have not been invited into a party!", ColorCodes.Pink);
            return;
        }

        if (player.PartyMember.PartyMember != player)
        {
            player.Tell("Party failed.", ColorCodes.Pink);
            return;
        }

        player.InParty = true;
        player.Tell($"You have joined {player.PartyMember.Character.Name}'s party!", ColorCodes.Pink);

        player.PartyMember.InParty = true;
        player.PartyMember.Tell($"{player.Character.Name} has joined your party!", ColorCodes.Pink);
    }
}