using Mirage.Game.Data;

namespace Mirage.Net.Protocol.FromServer;

public sealed record NpcDir(int Slot, Direction Direction) : IPacket<NpcDir>
{
    public static string PacketId => "npcdir";

    public static NpcDir ReadFrom(PacketReader reader)
    {
        return new NpcDir(
            Slot: reader.ReadInt32(),
            Direction: reader.ReadEnum<Direction>());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(Slot);
        writer.WriteEnum(Direction);
    }
}