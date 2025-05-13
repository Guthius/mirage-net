namespace Mirage.Net.Protocol.FromClient;

public sealed record SayRequest(string Message) : IPacket<SayRequest>
{
    public static string PacketId => "saymsg";

    public static SayRequest ReadFrom(PacketReader reader)
    {
        return new SayRequest(Message: reader.ReadString());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteString(Message);
    }
}