namespace Mirage.Net.Protocol.FromServer;

public sealed record GlobalMessage(string Message, int Color) : IPacket<GlobalMessage>
{
    public static string PacketId => "globalmsg";

    public static GlobalMessage ReadFrom(PacketReader reader)
    {
        return new GlobalMessage(
            Message: reader.ReadString(),
            Color: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteString(Message);
        writer.WriteInt32(Color);
    }
}