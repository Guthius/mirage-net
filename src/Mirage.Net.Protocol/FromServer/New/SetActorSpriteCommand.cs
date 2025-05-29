namespace Mirage.Net.Protocol.FromServer.New;

public sealed record SetActorSpriteCommand(int ActorId, int Sprite) : IPacket<SetActorSpriteCommand>
{
    public static string PacketId => nameof(SetActorSpriteCommand);

    public static SetActorSpriteCommand ReadFrom(PacketReader reader)
    {
        return new SetActorSpriteCommand(
            ActorId: reader.ReadInt32(),
            Sprite: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(ActorId);
        writer.WriteInt32(Sprite);
    }
}