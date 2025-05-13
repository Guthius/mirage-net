namespace Mirage.Net.Protocol.FromServer;

public sealed record MapKey(int X, int Y, bool Unlocked) : IPacket<MapKey>
{
    public static string PacketId => "mapkey";

    public static MapKey ReadFrom(PacketReader reader)
    {
        return new MapKey(
            X: reader.ReadInt32(),
            Y: reader.ReadInt32(),
            Unlocked: reader.ReadInt32() == 1);
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(X);
        writer.WriteInt32(Y);
        writer.WriteInt32(Unlocked ? 1 : 0);
    }
}