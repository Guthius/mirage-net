using CommunityToolkit.Mvvm.DependencyInjection;
using Mirage.Client.Net;
using Mirage.Net.Protocol.FromClient;

namespace Mirage.Client;

public static class ChatProcessor
{
    private static class Commands
    {
        public const string Fps = "/fps";
        public const string Train = "/train";
    }

    public static void Handle(ReadOnlySpan<char> message)
    {
        var gameState = Ioc.Default.GetRequiredService<Game>();

        message = message.Trim();
        if (message.IsEmpty)
        {
            return;
        }

        if (message.StartsWith(Commands.Fps, StringComparison.OrdinalIgnoreCase))
        {
            gameState.ShowFps = !gameState.ShowFps;

            return;
        }

        if (message.StartsWith(Commands.Train, StringComparison.OrdinalIgnoreCase))
        {
            // TODO: Show stat training window...
            return;
        }

        Network.Send(new SayRequest(new string(message)));
    }
}