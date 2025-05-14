using Mirage.Game.Data;

namespace Mirage.Net.Protocol.FromServer.New;

public sealed record CreatePlayerCommand(int PlayerId, string Name, string JobId, int Sprite, AccessLevel AccessLevel, int X, int Y, Direction Direction) : IPacket<CreatePlayerCommand>
{
    public static string PacketId => nameof(CreatePlayerCommand);

    public static CreatePlayerCommand ReadFrom(PacketReader reader)
    {
        return new CreatePlayerCommand(
            PlayerId: reader.ReadInt32(),
            Name: reader.ReadString(),
            JobId: reader.ReadString(),
            Sprite: reader.ReadInt32(),
            AccessLevel: reader.ReadEnum<AccessLevel>(),
            X: reader.ReadInt32(),
            Y: reader.ReadInt32(),
            Direction: reader.ReadEnum<Direction>());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(PlayerId);
        writer.WriteString(Name);
        writer.WriteString(JobId);
        writer.WriteInt32(Sprite);
        writer.WriteEnum(AccessLevel);
        writer.WriteInt32(X);
        writer.WriteInt32(Y);
        writer.WriteEnum(Direction);
    }
}