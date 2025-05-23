using Mirage.Shared.Data;

namespace Mirage.Net.Protocol.FromClient;

public sealed record NewMapRequest(Direction Direction) : IPacket<NewMapRequest>
{
    public static string PacketId => "requestnewmap";

    public static NewMapRequest ReadFrom(PacketReader reader)
    {
        return new NewMapRequest(Direction: reader.ReadEnum<Direction>());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteEnum(Direction);
    }
}