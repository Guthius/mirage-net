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
        player.Tell($"HP: {player.Character.Health}/{player.Character.MaxHealth}  MP: {player.Character.Mana}/{player.Character.MaxMana}  SP: {player.Character.Stamina}/{player.Character.MaxStamina}", ColorCode.White);
        player.Tell($"STR: {player.Character.Strength}  DEF: {player.Character.Defense}  MAGI: {player.Character.Intelligence}  SPEED: {player.Character.Speed}", ColorCode.White);
        player.Tell($"Critical Hit Chance: {player.Character.CriticalHitRate}%, Block Chance: {player.Character.BlockRate}%", ColorCode.White);
    }
}