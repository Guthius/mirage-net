namespace Mirage.Net.Protocol.FromServer;

public sealed record CreateItemCommand(int Id, int Sprite, int X, int Y) : IPacket<CreateItemCommand>
{
    public static string PacketId => nameof(CreateItemCommand);

    public static CreateItemCommand ReadFrom(PacketReader reader)
    {
        return new CreateItemCommand(
            Id: reader.ReadInt32(),
            Sprite: reader.ReadInt32(),
            X: reader.ReadInt32(),
            Y: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(Id);
        writer.WriteInt32(Sprite);
        writer.WriteInt32(X);
        writer.WriteInt32(Y);
    }
}