using Mirage.Game.Data;

namespace Mirage.Net.Protocol.FromServer.New;

public sealed record MovePlayerCommand(int PlayerId, Direction Direction, MovementType MovementType) : IPacket<MovePlayerCommand>
{
    public static string PacketId => nameof(MovePlayerCommand);

    public static MovePlayerCommand ReadFrom(PacketReader reader)
    {
        return new MovePlayerCommand(
            PlayerId: reader.ReadInt32(),
            Direction: reader.ReadEnum<Direction>(),
            MovementType: reader.ReadEnum<MovementType>());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(PlayerId);
        writer.WriteEnum(Direction);
        writer.WriteEnum(MovementType);
    }
}