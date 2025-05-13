namespace Mirage.Net.Protocol.FromServer;

public sealed record AlertMessage(string Message) : IPacket<AlertMessage>
{
    public static string PacketId => "alertmsg";

    public static AlertMessage ReadFrom(PacketReader reader)
    {
        return new AlertMessage(reader.ReadString());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteString(Message);
    }
}