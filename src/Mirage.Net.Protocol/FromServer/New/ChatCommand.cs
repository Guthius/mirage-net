namespace Mirage.Net.Protocol.FromServer.New;

public sealed record ChatCommand(string Message, int Color) : IPacket<ChatCommand>
{
    public static string PacketId => "playermsg";

    public static ChatCommand ReadFrom(PacketReader reader)
    {
        return new ChatCommand(
            Message: reader.ReadString(),
            Color: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteString(Message);
        writer.WriteInt32(Color);
    }
}