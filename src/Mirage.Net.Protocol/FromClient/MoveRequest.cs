using Mirage.Game.Data;

namespace Mirage.Net.Protocol.FromClient;

public sealed record MoveRequest(Direction Direction, MovementType Movement) : IPacket<MoveRequest>
{
    public static string PacketId => "playermove";

    public static MoveRequest ReadFrom(PacketReader reader)
    {
        return new MoveRequest(
            Direction: reader.ReadEnum<Direction>(),
            Movement: reader.ReadEnum<MovementType>());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteEnum(Direction);
        writer.WriteEnum(Movement);
    }
}