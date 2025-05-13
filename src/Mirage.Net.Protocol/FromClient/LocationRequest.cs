namespace Mirage.Net.Protocol.FromClient;

public sealed record LocationRequest : IPacket<LocationRequest>
{
    public static string PacketId => "requestlocation";

    public static LocationRequest ReadFrom(PacketReader reader)
    {
        return EmptyPacket<LocationRequest>.Value;
    }

    public void WriteTo(PacketWriter writer)
    {
    }
}