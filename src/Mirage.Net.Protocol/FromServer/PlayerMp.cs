namespace Mirage.Net.Protocol.FromServer;

public sealed record PlayerMp(int MaxMana, int Mana) : IPacket<PlayerMp>
{
    public static string PacketId => "playermp";

    public static PlayerMp ReadFrom(PacketReader reader)
    {
        return new PlayerMp(
            MaxMana: reader.ReadInt32(),
            Mana: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(MaxMana);
        writer.WriteInt32(Mana);
    }
}