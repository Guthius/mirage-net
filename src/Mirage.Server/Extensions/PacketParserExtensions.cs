using Mirage.Game.Data;
using Mirage.Net;
using Mirage.Server.Game;

namespace Mirage.Server.Extensions;

public static class PacketParserExtensions
{
    // GameSession, Packet
    public static void Register<TPacket>(this PacketParser parser, Action<GameSession, TPacket> handler) where TPacket : IPacket<TPacket>
    {
        parser.Register<TPacket>((playerId, packet) =>
        {
            var session = GameState.GetSession(playerId);
            if (session is null)
            {
                return;
            }

            handler(session, packet);
        });
    }

    // GameSession, AccountInfo, Packet
    public static void Register<TPacket>(this PacketParser parser, Action<GameSession, AccountInfo, TPacket> handler) where TPacket : IPacket<TPacket>
    {
        parser.Register<TPacket>((playerId, packet) =>
        {
            var session = GameState.GetSession(playerId);
            if (session?.Account is null)
            {
                return; // Player not logged in
            }

            handler(session, session.Account, packet);
        });
    }

    // GamePlayer, Packet
    public static void Register<TPacket>(this PacketParser parser, Action<GamePlayer, TPacket> handler) where TPacket : IPacket<TPacket>
    {
        parser.Register<TPacket>((playerId, packet) =>
        {
            var session = GameState.GetSession(playerId);
            if (session?.Player is null)
            {
                return; // Player not in game
            }

            handler(session.Player, packet);
        });
    }

    // GamePlayer, Packet
    public static void Register<TPacket>(this PacketParser parser, Action<GamePlayer, TPacket> handler, AccessLevel minimumAccessLevel) where TPacket : IPacket<TPacket>
    {
        parser.Register<TPacket>((playerId, packet) =>
        {
            var session = GameState.GetSession(playerId);
            if (session?.Player is null)
            {
                return; // Player not in game
            }

            if (session.Player.Character.AccessLevel < minimumAccessLevel)
            {
                return; // Admin cloning
            }

            handler(session.Player, packet);
        });
    }
}