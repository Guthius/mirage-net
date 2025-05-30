namespace Mirage.Net;

public delegate void ClientPacketHandler<in TPacket>(TPacket packet) where TPacket : IPacket<TPacket>;