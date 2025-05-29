namespace Mirage.Net.Protocol.FromClient;

public sealed record LookAtRequest(int X, int Y) : IPacket<LookAtRequest>
{
    public static string PacketId => nameof(LookAtRequest);

    public static LookAtRequest ReadFrom(PacketReader reader)
    {
        return new LookAtRequest(X: reader.ReadInt32(), Y: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(X);
        writer.WriteInt32(Y);
    }
}