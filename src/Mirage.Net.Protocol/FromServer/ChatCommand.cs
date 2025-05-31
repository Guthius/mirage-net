using Mirage.Shared.Data;

namespace Mirage.Net.Protocol.FromServer;

public sealed record ChatCommand(string Message, ColorCode Color) : IPacket<ChatCommand>
{
    public static string PacketId => nameof(ChatCommand);

    public static ChatCommand ReadFrom(PacketReader reader)
    {
        return new ChatCommand(
            Message: reader.ReadString(),
            Color: new ColorCode(
                reader.ReadByte(),
                reader.ReadByte(),
                reader.ReadByte(),
                reader.ReadByte()));
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteString(Message);
        writer.WriteByte(Color.R);
        writer.WriteByte(Color.G);
        writer.WriteByte(Color.B);
        writer.WriteByte(Color.A);
    }
}