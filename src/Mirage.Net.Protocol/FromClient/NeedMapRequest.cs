namespace Mirage.Net.Protocol.FromClient;

public sealed record NeedMapRequest(bool NeedMap) : IPacket<NeedMapRequest>
{
    public static string PacketId => "needmap";

    public static NeedMapRequest ReadFrom(PacketReader reader)
    {
        return new NeedMapRequest(reader.ReadString() == "yes");
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteString(NeedMap ? "yes" : "no");
    }
}