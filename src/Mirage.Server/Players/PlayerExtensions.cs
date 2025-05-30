using Mirage.Net;

namespace Mirage.Server.Players;

public static class PlayerExtensions
{
    public static void Send<TPacket>(this IEnumerable<Player> players, TPacket packet) where TPacket : IPacket<TPacket>
    {
        var bytes = PacketSerializer.GetBytes(packet);

        foreach (var player in players)
        {
            player.Send(bytes);
        }
    }
}