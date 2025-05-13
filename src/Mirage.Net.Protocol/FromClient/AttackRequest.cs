namespace Mirage.Net.Protocol.FromClient;

public sealed record AttackRequest : IPacket<AttackRequest>
{
    public static string PacketId => "attack";
    
    public static AttackRequest ReadFrom(PacketReader reader)
    {
        return EmptyPacket<AttackRequest>.Value;
    }

    public void WriteTo(PacketWriter writer)
    {
    }
}