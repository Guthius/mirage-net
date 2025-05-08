namespace Mirage.Net.Protocol.FromServer;

public sealed record Attack(int PlayerId) : IPacket<Attack>
{
    public static string PacketId => "attack";

    public static Attack ReadFrom(PacketReader reader)
    {
        return new Attack(PlayerId: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(PlayerId);
    }
}