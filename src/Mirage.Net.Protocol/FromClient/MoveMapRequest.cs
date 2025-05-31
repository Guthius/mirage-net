using Mirage.Shared.Data;

namespace Mirage.Net.Protocol.FromClient;

public sealed record MoveMapRequest(Direction Direction) : IPacket<MoveMapRequest>
{
    public static string PacketId => nameof(MoveMapRequest);

    public static MoveMapRequest ReadFrom(PacketReader reader)
    {
        return new MoveMapRequest(Direction: reader.ReadEnum<Direction>());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteEnum(Direction);
    }
}