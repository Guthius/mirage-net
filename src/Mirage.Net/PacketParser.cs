namespace Mirage.Net;

public sealed class PacketParser(Action<int, string>? reportBadPacket = null)
{
    private readonly Dictionary<string, Action<int, PacketReader>> _handlers = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Registers a handler for packets of type <typeparamref name="TPacket"/>.
    /// </summary>
    /// <param name="handler">The handler.</param>
    /// <typeparam name="TPacket">The packet type.</typeparam>
    public void Register<TPacket>(ServerPacketHandler<TPacket> handler) where TPacket : IPacket<TPacket>
    {
        _handlers[TPacket.PacketId] = (playerId, packetReader) =>
        {
            var packet = TPacket.ReadFrom(packetReader);

            handler(playerId, packet);
        };
    }

    /// <summary>
    /// Registers a handler for packets of type <typeparamref name="TPacket"/>.
    /// </summary>
    /// <param name="handler">The handler.</param>
    /// <typeparam name="TPacket">The packet type.</typeparam>
    public void Register<TPacket>(ClientPacketHandler<TPacket> handler) where TPacket : IPacket<TPacket>
    {
        _handlers[TPacket.PacketId] = (_, packetReader) =>
        {
            var packet = TPacket.ReadFrom(packetReader);

            handler(packet);
        };
    }

    /// <summary>
    /// Registers a handler for packets of type <typeparamref name="TPacket"/>.
    /// </summary>
    /// <param name="handler">The handler.</param>
    /// <typeparam name="TPacket">The packet type.</typeparam>
    public void Register<TPacket>(Action handler) where TPacket : IPacket<TPacket>
    {
        _handlers[TPacket.PacketId] = (_, _) => { handler(); };
    }

    /// <summary>
    /// Parses the specified <paramref name="bytes"/> for data packets and calls the registered handler for each packet.
    /// </summary>
    /// <param name="playerId">The ID of the player that sent the packets.</param>
    /// <param name="bytes">The raw packet data.</param>
    /// <returns>The number of bytes processed from the input bytes.</returns>
    public int Parse(int playerId, ReadOnlyMemory<byte> bytes)
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
                handler(playerId, packetReader);
            }
            else
            {
                reportBadPacket?.Invoke(playerId, packetId);
            }

            bytes = bytes[(end + 1)..];
        }

        return byteCount - bytes.Length;
    }

    /// <summary>
    /// Parses the specified <paramref name="bytes"/> for data packets and calls the registered handler for each packet.
    /// </summary>
    /// <param name="bytes">The raw packet data.</param>
    /// <returns>The number of bytes processed from the input bytes.</returns>
    public int Parse(ReadOnlyMemory<byte> bytes) => Parse(0, bytes);
}