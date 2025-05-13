namespace Mirage.Net.Protocol.FromServer;

public sealed record NpcDead(int Slot) : IPacket<NpcDead>
{
    public static string PacketId => "npcdead";

    public static NpcDead ReadFrom(PacketReader reader)
    {
        return new NpcDead(Slot: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(Slot);
    }
}