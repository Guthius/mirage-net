namespace Mirage.Net.Protocol.FromServer;

public sealed record MapMessage(string Message, int Color) : IPacket<MapMessage>
{
    public static string PacketId => "mapmsg";

    public static MapMessage ReadFrom(PacketReader reader)
    {
        return new MapMessage(
            Message: reader.ReadString(),
            Color: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteString(Message);
        writer.WriteInt32(Color);
    }
}