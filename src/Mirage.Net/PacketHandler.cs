namespace Mirage.Net;

public delegate void PacketHandler<in TPacket>(int playerId, TPacket packet) where TPacket : IPacket<TPacket>;