using Mirage.Server.Players;
using Mirage.Shared.Constants;
using Mirage.Shared.Data;

namespace Mirage.Server.Chat.Commands;

public sealed class StatsCommand() : Command(ChatCommandNames.Stats, AccessLevel.None)
{
    public override void Execute(Player player, ReadOnlySpan<char> args)
    {
        player.Tell($"-=- Stats for {player.Character.Name} -=-", ColorCode.White);
        player.Tell($"Level: {player.Character.Level}  Exp: {player.Character.Exp}/{player.Character.RequiredExp}", ColorCode.White);
        player.Tell($"HP: {player.Character.HP}/{player.Character.MaxHP}  MP: {player.Character.MP}/{player.Character.MaxMP}  SP: {player.Character.SP}/{player.Character.MaxSP}", ColorCode.White);
        player.Tell($"STR: {player.Character.Strength}  DEF: {player.Character.Defense}  MAGI: {player.Character.Intelligence}  SPEED: {player.Character.Speed}", ColorCode.White);
        player.Tell($"Critical Hit Chance: {player.Character.CriticalHitRate}%, Block Chance: {player.Character.BlockRate}%", ColorCode.White);
    }
}