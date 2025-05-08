namespace Mirage.Net.Protocol.FromClient;

public sealed record GlobalMessageRequest(string Message) : IPacket<GlobalMessageRequest>
{
    public static string PacketId => "globalmsg";
    
    public static GlobalMessageRequest ReadFrom(PacketReader reader)
    {
        return new GlobalMessageRequest(Message: reader.ReadString());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteString(Message);
    }
}