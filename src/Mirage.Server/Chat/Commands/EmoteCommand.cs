using Mirage.Server.Players;
using Mirage.Shared.Constants;
using Mirage.Shared.Data;

namespace Mirage.Server.Chat.Commands;

public sealed class EmoteCommand() : Command(ChatCommandNames.Emote, AccessLevel.None)
{
    public override void Execute(Player player, ReadOnlySpan<char> args)
    {
        if (!args.IsEmpty)
        {
            player.Map.SendMessage($"{player.Character.Name} {args}", ColorCode.EmoteColor);
        }
    }
}