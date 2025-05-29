namespace Mirage.Net.Protocol.FromServer;

public sealed record DisconnectCommand(string Message) : IPacket<DisconnectCommand>
{
    public static string PacketId => nameof(DisconnectCommand);

    public static DisconnectCommand ReadFrom(PacketReader reader)
    {
        return new DisconnectCommand(reader.ReadString());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteString(Message);
    }
}