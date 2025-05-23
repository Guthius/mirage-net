using Mirage.Shared.Data;

namespace Mirage.Net.Protocol.FromClient;

public sealed record UseStatPointRequest(StatType PointType) : IPacket<UseStatPointRequest>
{
    public static string PacketId => "usestatpoint";

    public static UseStatPointRequest ReadFrom(PacketReader reader)
    {
        return new UseStatPointRequest(PointType: reader.ReadEnum<StatType>());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteEnum(PointType);
    }
}