namespace Mirage.Net.Protocol.FromClient;

public sealed record LeavePartyRequest : IPacket<LeavePartyRequest>
{
    public static string PacketId => "leaveparty";

    public static LeavePartyRequest ReadFrom(PacketReader reader)
    {
        return EmptyPacket<LeavePartyRequest>.Value;
    }

    public void WriteTo(PacketWriter writer)
    {
    }
}