using Microsoft.Extensions.Logging;
using Mirage.Net.Protocol.FromServer.New;
using Mirage.Server.Players;
using Mirage.Shared.Constants;
using Mirage.Shared.Data;

namespace Mirage.Server.Chat.Commands;

public sealed class SetMotdCommand(ILogger<SetMotdCommand> logger, IPlayerService players) : Command(ChatCommandNames.SetMotd, AccessLevel.Mapper)
{
    public override void Execute(Player player, ReadOnlySpan<char> args)
    {
        if (args.IsEmpty)
        {
            return;
        }

        File.WriteAllText("Motd.txt", args);

        logger.LogInformation("{CharacterName} changed MOTD to: {NewMotd}", player.Character.Name, new string(args));

        players.Send(new ChatCommand($"MOTD changed to: {args}", ColorCode.BrightCyan));
    }
}