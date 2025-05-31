using Mirage.Client.Net;
using Mirage.Net.Protocol.FromClient;

namespace Mirage.Client.Chat;

public sealed class ChatService(Game game)
{
    private static class LocalChatCommands
    {
        public const string Fps = "/fps";
        public const string Train = "/train";
    }
    
    public void Send(ReadOnlySpan<char> message)
    {
        message = message.Trim();
        if (message.IsEmpty)
        {
            return;
        }

        if (message.StartsWith(LocalChatCommands.Fps, StringComparison.OrdinalIgnoreCase))
        {
            game.ShowFps = !game.ShowFps;

            return;
        }

        if (message.StartsWith(LocalChatCommands.Train, StringComparison.OrdinalIgnoreCase))
        {
            // TODO: Show stat training window...

            return;
        }

        Network.Send(new SayRequest(new string(message)));
    }
}