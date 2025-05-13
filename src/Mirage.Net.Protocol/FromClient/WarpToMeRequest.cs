namespace Mirage.Net.Protocol.FromClient;

public sealed record WarpToMeRequest(string TargetName) : IPacket<WarpToMeRequest>
{
    public static string PacketId => "warptome";

    public static WarpToMeRequest ReadFrom(PacketReader reader)
    {
        return new WarpToMeRequest(TargetName: reader.ReadString());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteString(TargetName);
    }
}