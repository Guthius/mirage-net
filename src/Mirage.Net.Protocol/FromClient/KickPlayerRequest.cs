namespace Mirage.Net.Protocol.FromClient;

public sealed record KickPlayerRequest(string TargetName) : IPacket<KickPlayerRequest>
{
    public static string PacketId => "kickplayer";

    public static KickPlayerRequest ReadFrom(PacketReader reader)
    {
        return new KickPlayerRequest(TargetName: reader.ReadString());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteString(TargetName);
    }
}