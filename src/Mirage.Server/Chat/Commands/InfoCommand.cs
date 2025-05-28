using Mirage.Server.Players;
using Mirage.Shared.Constants;
using Mirage.Shared.Data;

namespace Mirage.Server.Chat.Commands;

public sealed class InfoCommand(IPlayerService players) : Command(ChatCommandNames.Info, AccessLevel.None)
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

        player.Tell($"Name: {targetPlayer.Character.Name}", ColorCode.BrightGreen);
        if (player.Character.AccessLevel <= AccessLevel.Moderator)
        {
            return;
        }

        player.Tell($"-=- Stats for {targetPlayer.Character.Name} -=-", ColorCode.BrightGreen);
        player.Tell($"Level: {targetPlayer.Character.Level}  Exp: {targetPlayer.Character.Exp}/{targetPlayer.Character.RequiredExp}", ColorCode.BrightGreen);
        player.Tell($"HP: {targetPlayer.Character.HP}/{targetPlayer.Character.MaxHP}  MP: {targetPlayer.Character.MP}/{targetPlayer.Character.MaxMP}  SP: {targetPlayer.Character.SP}/{targetPlayer.Character.MaxSP}", ColorCode.BrightGreen);
        player.Tell($"STR: {targetPlayer.Character.Strength}  DEF: {targetPlayer.Character.Defense}  MAGI: {targetPlayer.Character.Intelligence}  SPEED: {targetPlayer.Character.Speed}", ColorCode.BrightGreen);

        player.Tell($"Critical Hit Chance: {targetPlayer.Character.CriticalHitRate}%, Block Chance: {targetPlayer.Character.BlockRate}%", ColorCode.BrightGreen);
    }
}