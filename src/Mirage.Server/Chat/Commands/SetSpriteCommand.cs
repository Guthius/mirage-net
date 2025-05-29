using Mirage.Net.Protocol.FromServer.New;
using Mirage.Server.Players;
using Mirage.Shared.Data;

namespace Mirage.Server.Chat.Commands;

public sealed class SetSpriteCommand() : Command(ChatCommandNames.SetSprite, AccessLevel.Mapper)
{
    public override void Execute(Player player, ReadOnlySpan<char> args)
    {
        if (args.IsEmpty)
        {
            return;
        }

        if (!int.TryParse(args, out var sprite))
        {
            return;
        }

        player.Character.Sprite = sprite;
        player.Map.Send(new SetActorSpriteCommand(player.Id, player.Character.Sprite));
    }
}