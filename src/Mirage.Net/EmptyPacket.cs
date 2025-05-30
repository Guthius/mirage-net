namespace Mirage.Net;

/// <summary>
///     <para>
///         This is a generic storage class that provides a singleton instance for packets without payload (empty packets).
///     </para>
///     <para>
///         Instead of creating new instances of empty packets each time they are needed, this class
///         provides a reusable singleton instance.
///     </para>
/// </summary>
/// <typeparam name="TPacket">
///     The type of packet. Must implement <see cref="IPacket{TSelf}"/> interface and have a parameterless constructor.
/// </typeparam>
/// <remarks>
///     This class is thread-safe as it only contains a readonly static field initialized during type initialization.
/// </remarks>
public static class EmptyPacket<TPacket> where TPacket : IPacket<TPacket>, new()
{
    /// <summary>
    /// Gets the singleton instance of the empty packet.
    /// </summary>
    public static readonly TPacket Value = new();
}