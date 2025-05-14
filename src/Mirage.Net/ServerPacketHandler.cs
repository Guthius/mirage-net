namespace Mirage.Net;

public delegate void ServerPacketHandler<in TPacket>(int playerId, TPacket packet) where TPacket : IPacket<TPacket>;