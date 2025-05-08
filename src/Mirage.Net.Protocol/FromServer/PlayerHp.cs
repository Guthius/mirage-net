namespace Mirage.Net.Protocol.FromServer;

public sealed record PlayerHp(int MaxHealth, int Health) : IPacket<PlayerHp>
{
    public static string PacketId => "playerhp";

    public static PlayerHp ReadFrom(PacketReader reader)
    {
        return new PlayerHp(
            MaxHealth: reader.ReadInt32(),
            Health: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(MaxHealth);
        writer.WriteInt32(Health);
    }
}