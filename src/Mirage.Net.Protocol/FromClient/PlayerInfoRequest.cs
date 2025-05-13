namespace Mirage.Net.Protocol.FromClient;

public sealed record PlayerInfoRequest(string TargetName) : IPacket<PlayerInfoRequest>
{
    public static string PacketId => "playerinforequest";

    public static PlayerInfoRequest ReadFrom(PacketReader reader)
    {
        return new PlayerInfoRequest(TargetName: reader.ReadString());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteString(TargetName);
    }
}