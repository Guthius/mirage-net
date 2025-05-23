using CommunityToolkit.Mvvm.DependencyInjection;
using Mirage.Client.Net;
using Mirage.Net.Protocol.FromClient.New;
using Mirage.Shared.Constants;
using Mirage.Shared.Data;

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
        var gameState = Ioc.Default.GetRequiredService<GameClient>();
        
        message = message.Trim();
        if (message.IsEmpty)
        {
            return;
        }

        if (message.StartsWith(Commands.Fps, StringComparison.OrdinalIgnoreCase))
        {
            // TODO: gameState.ChatHistory.Add(new ChatInfo($"FPS: {modGameLogic.GameFPS}", ColorCode.Pink));
            gameState.ChatHistoryUpdated = true;
            
            return;
        }

        if (message.StartsWith(Commands.Train, StringComparison.OrdinalIgnoreCase))
        {
            // using var frmTraining = new frmTraining();
            // frmTraining.ShowDialog();
            return;
        }

        Network.Send(new SayRequest(new string(message)));
    }
}