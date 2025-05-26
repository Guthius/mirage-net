using Mirage.Shared.Data;

namespace Mirage.Net.Protocol.FromServer.New;

public sealed record SetActorDirectionCommand(int ActorId, Direction Direction) : IPacket<SetActorDirectionCommand>
{
    public static string PacketId => nameof(SetActorDirectionCommand);
    
    public static SetActorDirectionCommand ReadFrom(PacketReader reader)
    {
        return new SetActorDirectionCommand(
            ActorId: reader.ReadInt32(), 
            Direction: reader.ReadEnum<Direction>());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(ActorId);
        writer.WriteEnum(Direction);
    }
}