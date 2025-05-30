namespace Mirage.Net.Protocol.FromServer;

public sealed record ActorAttackCommand(int ActorId) : IPacket<ActorAttackCommand>
{
    public static string PacketId => nameof(ActorAttackCommand);

    public static ActorAttackCommand ReadFrom(PacketReader reader)
    {
        return new ActorAttackCommand(ActorId: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(ActorId);
    }
}