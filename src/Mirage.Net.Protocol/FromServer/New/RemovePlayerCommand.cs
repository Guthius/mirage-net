namespace Mirage.Net.Protocol.FromServer.New;

public sealed record RemovePlayerCommand(int PlayerId) : IPacket<RemovePlayerCommand>
{
    public static string PacketId => nameof(RemovePlayerCommand);

    public static RemovePlayerCommand ReadFrom(PacketReader reader)
    {
        return new RemovePlayerCommand(PlayerId: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(PlayerId);
    }
}