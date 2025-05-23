namespace Mirage.Net.Protocol.FromClient.New;

public sealed record AttackRequest : IPacket<AttackRequest>
{
    public static string PacketId => nameof(AttackRequest);
    
    public static AttackRequest ReadFrom(PacketReader reader)
    {
        return EmptyPacket<AttackRequest>.Value;
    }

    public void WriteTo(PacketWriter writer)
    {
    }
}