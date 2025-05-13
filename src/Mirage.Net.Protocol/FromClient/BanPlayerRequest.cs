namespace Mirage.Net.Protocol.FromClient;

public sealed record BanPlayerRequest(string TargetName) : IPacket<BanPlayerRequest>
{
    public static string PacketId => "banplayer";

    public static BanPlayerRequest ReadFrom(PacketReader reader)
    {
        return new BanPlayerRequest(TargetName: reader.ReadString());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteString(TargetName);
    }
}