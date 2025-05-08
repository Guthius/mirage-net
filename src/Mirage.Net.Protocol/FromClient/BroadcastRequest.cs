namespace Mirage.Net.Protocol.FromClient;

public sealed record BroadcastRequest(string Message) : IPacket<BroadcastRequest>
{
    public static string PacketId => "broadcastmsg";
    
    public static BroadcastRequest ReadFrom(PacketReader reader)
    {
        return new BroadcastRequest(Message: reader.ReadString());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteString(Message);
    }
}