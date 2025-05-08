namespace Mirage.Net.Protocol.FromClient;

public sealed record AdminMessageRequest(string Message) : IPacket<AdminMessageRequest>
{
    public static string PacketId => "adminmsg";

    public static AdminMessageRequest ReadFrom(PacketReader reader)
    {
        return new AdminMessageRequest(Message: reader.ReadString());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteString(Message);
    }
}