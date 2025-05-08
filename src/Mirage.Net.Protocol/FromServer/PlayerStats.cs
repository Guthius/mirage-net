namespace Mirage.Net.Protocol.FromServer;

public sealed record PlayerStats(int Strength, int Defense, int Speed, int Magi) : IPacket<PlayerStats>
{
    public static string PacketId => "playerstats";

    public static PlayerStats ReadFrom(PacketReader reader)
    {
        return new PlayerStats(
            Strength: reader.ReadInt32(),
            Defense: reader.ReadInt32(),
            Speed: reader.ReadInt32(),
            Magi: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(Strength);
        writer.WriteInt32(Defense);
        writer.WriteInt32(Speed);
        writer.WriteInt32(Magi);
    }
}