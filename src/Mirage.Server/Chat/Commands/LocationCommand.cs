using Mirage.Server.Players;
using Mirage.Shared.Constants;
using Mirage.Shared.Data;

namespace Mirage.Server.Chat.Commands;

public sealed class LocationCommand() : Command(ChatCommandNames.Location, AccessLevel.Mapper)
{
    public override void Execute(Player player, ReadOnlySpan<char> args)
    {
        player.Tell($"Map: {player.Character.Map}, X: {player.Character.X}, Y: {player.Character.Y}", ColorCode.Pink);
    }
}