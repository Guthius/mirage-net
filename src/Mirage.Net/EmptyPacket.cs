namespace Mirage.Net;

public abstract class EmptyPacket<TPacket> where TPacket : IPacket<TPacket>, new()
{
    public static readonly TPacket Value = new();
}