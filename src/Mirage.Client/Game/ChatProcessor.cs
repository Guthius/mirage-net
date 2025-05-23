using Mirage.Client.Modules;
using Mirage.Client.Net;
using Mirage.Net.Protocol.FromClient;

namespace Mirage.Client.Game;

public static class ChatProcessor
{
    public static void Handle(ReadOnlySpan<char> message)
    {
        message = message.Trim();
        if (message.IsEmpty)
        {
            return;
        }

        if (message.StartsWith("/fps", StringComparison.OrdinalIgnoreCase))
        {
            modText.AddText($"FPS: {modGameLogic.GameFPS}", modText.Pink);
            return;
        }

        if (message.StartsWith("/train", StringComparison.OrdinalIgnoreCase))
        {
            // using var frmTraining = new frmTraining();
            // frmTraining.ShowDialog();
            return;
        }

        Network.Send(new SayRequest(new string(message)));
    }
}