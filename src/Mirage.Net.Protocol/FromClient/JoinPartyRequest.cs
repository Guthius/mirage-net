namespace Mirage.Net.Protocol.FromClient;

public sealed record JoinPartyRequest : IPacket<JoinPartyRequest>
{
    public static string PacketId => "joinparty";
    
    public static JoinPartyRequest ReadFrom(PacketReader reader)
    {
        return EmptyPacket<JoinPartyRequest>.Value;
    }

    public void WriteTo(PacketWriter writer)
    {
    }
}