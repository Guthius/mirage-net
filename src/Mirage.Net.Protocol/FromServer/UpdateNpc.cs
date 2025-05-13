namespace Mirage.Net.Protocol.FromServer;

public sealed record UpdateNpc(int NpcId, string Name, int Sprite) : IPacket<UpdateNpc>
{
    public static string PacketId => "updatenpc";

    public static UpdateNpc ReadFrom(PacketReader reader)
    {
        return new UpdateNpc(NpcId: reader.ReadInt32(), Name: reader.ReadString(), Sprite: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(NpcId);
        writer.WriteString(Name);
        writer.WriteInt32(Sprite);
    }
}