namespace Mirage.Net.Protocol.FromServer.New;

public sealed record DestroyItemCommand(int Id) : IPacket<DestroyItemCommand>
{
    public static string PacketId => nameof(DestroyItemCommand);

    public static DestroyItemCommand ReadFrom(PacketReader reader)
    {
        return new DestroyItemCommand(Id: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(Id);
    }
}