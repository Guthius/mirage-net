namespace Mirage.Net.Protocol.FromClient;

public sealed record WarpMeToRequest(string TargetName) : IPacket<WarpMeToRequest>
{
    public static string PacketId => "warpmeto";

    public static WarpMeToRequest ReadFrom(PacketReader reader)
    {
        return new WarpMeToRequest(TargetName: reader.ReadString());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteString(TargetName);
    }
}