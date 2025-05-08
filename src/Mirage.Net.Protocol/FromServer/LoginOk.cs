namespace Mirage.Net.Protocol.FromServer;

public sealed record LoginOk(int PlayerId) : IPacket<LoginOk>
{
    public static string PacketId => "loginok";

    public static LoginOk ReadFrom(PacketReader reader)
    {
        return new LoginOk(PlayerId: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(PlayerId);
    }
}