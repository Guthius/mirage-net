using Mirage.Shared.Data;

namespace Mirage.Net.Protocol.FromClient;

public sealed record SetDirectionRequest(Direction Direction) : IPacket<SetDirectionRequest>
{
    public static string PacketId => nameof(SetDirectionRequest);

    public static SetDirectionRequest ReadFrom(PacketReader reader)
    {
        return new SetDirectionRequest(Direction: reader.ReadEnum<Direction>());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteEnum(Direction);
    }
}