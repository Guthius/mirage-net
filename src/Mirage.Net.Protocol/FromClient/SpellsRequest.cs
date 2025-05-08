namespace Mirage.Net.Protocol.FromClient;

public sealed record SpellsRequest : IPacket<SpellsRequest>
{
    public static string PacketId => "spells";
    
    public static SpellsRequest ReadFrom(PacketReader reader)
    {
        return EmptyPacket<SpellsRequest>.Value;
    }

    public void WriteTo(PacketWriter writer)
    {
    }
}