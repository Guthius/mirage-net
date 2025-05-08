using Mirage.Game.Data;

namespace Mirage.Net.Protocol.FromServer;

public sealed record PlayerMove(int PlayerId, int X, int Y, Direction Direction, MovementType Movement) : IPacket<PlayerMove>
{
    public static string PacketId => "playermove";

    public static PlayerMove ReadFrom(PacketReader reader)
    {
        return new PlayerMove(
            PlayerId: reader.ReadInt32(),
            X: reader.ReadInt32(),
            Y: reader.ReadInt32(),
            Direction: reader.ReadEnum<Direction>(),
            Movement: reader.ReadEnum<MovementType>());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(PlayerId);
        writer.WriteInt32(X);
        writer.WriteInt32(Y);
        writer.WriteEnum(Direction);
        writer.WriteEnum(Movement);
    }
}