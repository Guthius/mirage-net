namespace Mirage.Net.Protocol.FromClient;

public sealed record SetMotdRequest(string Motd) : IPacket<SetMotdRequest>
{
    public static string PacketId => "setmotd";

    public static SetMotdRequest ReadFrom(PacketReader reader)
    {
        return new SetMotdRequest(Motd: reader.ReadString());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteString(Motd);
    }
}