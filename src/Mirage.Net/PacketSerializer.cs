using System.Text;

namespace Mirage.Net;

public static class PacketSerializer
{
    public static byte[] GetBytes<TPacket>(IPacket<TPacket> packet) where TPacket : IPacket<TPacket>
    {
        var packetWriter = new PacketWriter();

        packetWriter.WriteString(TPacket.PacketId);
        packet.WriteTo(packetWriter);

        var packetData = packetWriter.ToString();
        var packetLen = Encoding.UTF8.GetByteCount(packetData);

        var data = new byte[packetLen + 1];

        Encoding.UTF8.GetBytes(packetData, data);

        data[packetLen] = PacketOptions.PacketDelimiter;

        return data;
    }
}