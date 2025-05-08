namespace Mirage.Net.Protocol.FromClient;

public sealed record EmoteRequest(string Message) : IPacket<EmoteRequest>
{
    public static string PacketId => "emotemsg";
    
    public static EmoteRequest ReadFrom(PacketReader reader)
    {
        return new EmoteRequest(Message: reader.ReadString());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteString(Message);
    }
}