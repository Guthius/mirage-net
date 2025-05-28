using Mirage.Net;
using Mirage.Server.Players;
using Mirage.Server.Repositories.Accounts;

namespace Mirage.Server.Net;

public sealed class NetworkParser(Action<NetworkConnection, string> reportBadPacket)
{
    private readonly Dictionary<string, Action<NetworkConnection, PacketReader>> _handlers = new(StringComparer.OrdinalIgnoreCase);

    public void Register<TPacket>(Action<NetworkConnection, TPacket> handler) where TPacket : IPacket<TPacket>
    {
        _handlers[TPacket.PacketId] = (playerId, packetReader) =>
        {
            var packet = TPacket.ReadFrom(packetReader);

            handler(playerId, packet);
        };
    }

    public void Register<TPacket>(Action<NetworkConnection, AccountInfo, TPacket> handler) where TPacket : IPacket<TPacket>
    {
        Register<TPacket>((connection, packet) =>
        {
            if (connection.Account is null)
            {
                return; // Player not logged in
            }

            handler(connection, connection.Account, packet);
        });
    }

    public void Register<TPacket>(Action<Player, TPacket> handler) where TPacket : IPacket<TPacket>
    {
        Register<TPacket>((connection, packet) =>
        {
            if (connection.Player is null)
            {
                return; // Player not in game
            }

            handler(connection.Player, packet);
        });
    }

    public int Parse(NetworkConnection connection, ReadOnlyMemory<byte> bytes)
    {
        var byteCount = bytes.Length;

        while (bytes.Length > 0)
        {
            var end = bytes.Span.IndexOf(PacketOptions.PacketDelimiter);
            if (end == -1)
            {
                break;
            }

            var packetData = bytes[..end];
            var packetReader = new PacketReader(packetData);
            var packetId = packetReader.ReadString();

            if (_handlers.TryGetValue(packetId, out var handler))
            {
                handler(connection, packetReader);
            }
            else
            {
                reportBadPacket(connection, packetId);
            }

            bytes = bytes[(end + 1)..];
        }

        return byteCount - bytes.Length;
    }
}