namespace Mirage.Net.Protocol.FromServer;

public sealed record PlayerSp(int MaxStamina, int Stamina) : IPacket<PlayerSp>
{
    public static string PacketId => "playersp";

    public static PlayerSp ReadFrom(PacketReader reader)
    {
        return new PlayerSp(
            MaxStamina: reader.ReadInt32(),
            Stamina: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(MaxStamina);
        writer.WriteInt32(Stamina);
    }
}