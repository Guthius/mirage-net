namespace Mirage.Net;

/// <summary>
/// Represent a client or group of clients capable of receiving data packets.
/// </summary>
public interface IPacketRecipient
{
    /// <summary>
    /// Sends the specified <paramref name="packet"/> to the recipient.
    /// </summary>
    /// <param name="packet">The packet to send.</param>
    /// <typeparam name="TPacket">The packet type.</typeparam>
    void Send<TPacket>(TPacket packet) where TPacket : IPacket<TPacket>;

    /// <summary>
    /// Send an empty packet of type <typeparamref name="TPacket"/>.
    /// </summary>
    /// <typeparam name="TPacket">The packet type.</typeparam>
    void Send<TPacket>() where TPacket : IPacket<TPacket>, new() => Send(EmptyPacket<TPacket>.Value);
}