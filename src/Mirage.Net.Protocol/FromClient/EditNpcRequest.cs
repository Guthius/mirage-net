namespace Mirage.Net.Protocol.FromClient;

public sealed record EditNpcRequest(int NpcId) : IPacket<EditNpcRequest>
{
    public static string PacketId => "editnpc";

    public static EditNpcRequest ReadFrom(PacketReader reader)
    {
        return new EditNpcRequest(NpcId: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(NpcId);
    }
}