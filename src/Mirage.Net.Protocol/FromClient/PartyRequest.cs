namespace Mirage.Net.Protocol.FromClient;

public sealed record PartyRequest(string TargetName) : IPacket<PartyRequest>
{
    public static string PacketId => "party";

    public static PartyRequest ReadFrom(PacketReader reader)
    {
        return new PartyRequest(TargetName: reader.ReadString());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteString(TargetName);
    }
}