namespace Mirage.Net.Protocol.FromServer;

public sealed record PlayerMessage(string Message, int Color) : IPacket<PlayerMessage>
{
    public static string PacketId => "playermsg";

    public static PlayerMessage ReadFrom(PacketReader reader)
    {
        return new PlayerMessage(
            Message: reader.ReadString(),
            Color: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteString(Message);
        writer.WriteInt32(Color);
    }
}