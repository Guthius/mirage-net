namespace Mirage.Net.Protocol.FromServer;

public sealed record PlayerPosition(int X, int Y) : IPacket<PlayerPosition>
{
    public static string PacketId => "playerxy";

    public static PlayerPosition ReadFrom(PacketReader reader)
    {
        return new PlayerPosition(
            X: reader.ReadInt32(),
            Y: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(X);
        writer.WriteInt32(Y);
    }
}