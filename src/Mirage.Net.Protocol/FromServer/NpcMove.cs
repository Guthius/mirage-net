using Mirage.Shared.Data;

namespace Mirage.Net.Protocol.FromServer;

public sealed record NpcMove(int Slot, int X, int Y, Direction Direction, MovementType MovementType) : IPacket<NpcMove>
{
    public static string PacketId => "npcmove";

    public static NpcMove ReadFrom(PacketReader reader)
    {
        return new NpcMove(
            Slot: reader.ReadInt32(),
            X: reader.ReadInt32(),
            Y: reader.ReadInt32(),
            Direction: reader.ReadEnum<Direction>(),
            MovementType: reader.ReadEnum<MovementType>());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(Slot);
        writer.WriteInt32(X);
        writer.WriteInt32(Y);
        writer.WriteEnum(Direction);
        writer.WriteEnum(MovementType);
    }
}