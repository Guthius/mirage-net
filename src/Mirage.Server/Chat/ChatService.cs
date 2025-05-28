using Microsoft.Extensions.Logging;
using Mirage.Server.Chat.Commands;
using Mirage.Server.Players;
using Mirage.Shared.Constants;

namespace Mirage.Server.Chat;

public sealed class ChatService : IChatService
{
    private readonly List<Command> _commands;
    private readonly ILogger<ChatService> _logger;

    public ChatService(ILogger<ChatService> logger, IEnumerable<Command> commands)
    {
        _logger = logger;
        _commands = commands.ToList();

        logger.LogInformation("Chat service initialized with {Count} commands", _commands.Count);
    }

    public void Handle(Player player, ReadOnlySpan<char> message)
    {
        message = message.Trim();
        if (message.IsEmpty)
        {
            return;
        }

        _logger.LogInformation("[{Map}] {CharacterName}: '{Message}'", player.Character.Map, player.Character.Name, new string(message));

        if (message[0] == '/')
        {
            message = message[1..];
            foreach (var command in _commands)
            {
                if (!message.StartsWith(command.Name, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (player.Character.AccessLevel < command.MinimumAccessLevel)
                {
                    continue;
                }

                var args = message[command.Name.Length..].Trim();

                command.Execute(player, args);
                return;
            }

            _logger.LogDebug("[{Map}] {CharacterName} attempted to execute invalid or unauthorized command",
                player.Character.Map, player.Character.Name);

            return;
        }

        player.Map.SendMessage($"{player.Character.Name} says '{message}'", ColorCode.SayColor);
    }
}