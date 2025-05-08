namespace Mirage.Net.Protocol.FromServer;

public sealed record NpcAttack(int Slot) : IPacket<NpcAttack>
{
    public static string PacketId => "npcattack";

    public static NpcAttack ReadFrom(PacketReader reader)
    {
        return new NpcAttack(Slot: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(Slot);
    }
}