using Mirage.Shared.Data;

namespace Mirage.Net.Protocol.FromServer.New;

public sealed record SetActorAccessLevelCommand(int ActorId, AccessLevel AccessLevel) : IPacket<SetActorAccessLevelCommand>
{
    public static string PacketId => nameof(SetActorAccessLevelCommand);

    public static SetActorAccessLevelCommand ReadFrom(PacketReader reader)
    {
        return new SetActorAccessLevelCommand(
            ActorId: reader.ReadInt32(),
            AccessLevel: reader.ReadEnum<AccessLevel>());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(ActorId);
        writer.WriteEnum(AccessLevel);
    }
}