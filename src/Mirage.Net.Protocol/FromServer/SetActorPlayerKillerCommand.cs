namespace Mirage.Net.Protocol.FromServer;

public sealed record SetActorPlayerKillerCommand(int ActorId, bool PlayerKiller) : IPacket<SetActorPlayerKillerCommand>
{
    public static string PacketId => nameof(SetActorPlayerKillerCommand);

    public static SetActorPlayerKillerCommand ReadFrom(PacketReader reader)
    {
        return new SetActorPlayerKillerCommand(
            ActorId: reader.ReadInt32(),
            PlayerKiller: reader.ReadBoolean());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(ActorId);
        writer.WriteBoolean(PlayerKiller);
    }
}