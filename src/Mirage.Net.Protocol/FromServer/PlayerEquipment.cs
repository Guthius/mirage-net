namespace Mirage.Net.Protocol.FromServer;

public sealed record PlayerEquipment(int ArmorSlot, int WeaponSlot, int HelmetSlot, int ShieldSlot) : IPacket<PlayerEquipment>
{
    public static string PacketId => nameof(PlayerEquipment);

    public static PlayerEquipment ReadFrom(PacketReader reader)
    {
        return new PlayerEquipment(
            ArmorSlot: reader.ReadInt32(),
            WeaponSlot: reader.ReadInt32(),
            HelmetSlot: reader.ReadInt32(),
            ShieldSlot: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(ArmorSlot);
        writer.WriteInt32(WeaponSlot);
        writer.WriteInt32(HelmetSlot);
        writer.WriteInt32(ShieldSlot);
    }
}