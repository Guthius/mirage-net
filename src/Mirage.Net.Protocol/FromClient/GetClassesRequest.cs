namespace Mirage.Net.Protocol.FromClient;

public sealed record GetClassesRequest : IPacket<GetClassesRequest>
{
    public static string PacketId => "getclasses";
    
    public static GetClassesRequest ReadFrom(PacketReader reader)
    {
        return EmptyPacket<GetClassesRequest>.Value;
    }

    public void WriteTo(PacketWriter writer)
    {
    }
}