using Mirage.Shared.Data;

namespace Mirage.Net.Protocol.FromServer.New;

public sealed record SetActorPositionCommand(int ActorId, Direction Direction, int X, int Y) : IPacket<SetActorPositionCommand>
{
    public static string PacketId => nameof(SetActorPositionCommand);

    public static SetActorPositionCommand ReadFrom(PacketReader reader)
    {
        return new SetActorPositionCommand(
            ActorId: reader.ReadInt32(),
            Direction: reader.ReadEnum<Direction>(),
            X: reader.ReadInt32(),
            Y: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(ActorId);
        writer.WriteEnum(Direction);
        writer.WriteInt32(X);
        writer.WriteInt32(Y);
    }
}