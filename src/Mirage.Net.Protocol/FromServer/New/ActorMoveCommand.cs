using Mirage.Shared.Data;

namespace Mirage.Net.Protocol.FromServer.New;

public sealed record ActorMoveCommand(int ActorId, Direction Direction, MovementType MovementType) : IPacket<ActorMoveCommand>
{
    public static string PacketId => nameof(ActorMoveCommand);

    public static ActorMoveCommand ReadFrom(PacketReader reader)
    {
        return new ActorMoveCommand(
            ActorId: reader.ReadInt32(),
            Direction: reader.ReadEnum<Direction>(),
            MovementType: reader.ReadEnum<MovementType>());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(ActorId);
        writer.WriteEnum(Direction);
        writer.WriteEnum(MovementType);
    }
}