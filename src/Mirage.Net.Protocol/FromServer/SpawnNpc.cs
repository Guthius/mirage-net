using Mirage.Shared.Data;

namespace Mirage.Net.Protocol.FromServer;

public sealed record SpawnNpc(int Slot, int NpcId, int X, int Y, Direction Direction) : IPacket<SpawnNpc>
{
    public static string PacketId => "spawnnpc";

    public static SpawnNpc ReadFrom(PacketReader reader)
    {
        return new SpawnNpc(
            Slot: reader.ReadInt32(),
            NpcId: reader.ReadInt32(),
            X: reader.ReadInt32(),
            Y: reader.ReadInt32(),
            Direction: reader.ReadEnum<Direction>());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(Slot);
        writer.WriteInt32(NpcId);
        writer.WriteInt32(X);
        writer.WriteInt32(Y);
        writer.WriteEnum(Direction);
    }
}