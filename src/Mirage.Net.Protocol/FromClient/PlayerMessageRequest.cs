namespace Mirage.Net.Protocol.FromClient;

public sealed record PlayerMessageRequest(string TargetName, string Message) : IPacket<PlayerMessageRequest>
{
    public static string PacketId => "adminmsg";

    public static PlayerMessageRequest ReadFrom(PacketReader reader)
    {
        return new PlayerMessageRequest(
            TargetName: reader.ReadString(),
            Message: reader.ReadString());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteString(TargetName);
        writer.WriteString(Message);
    }
}