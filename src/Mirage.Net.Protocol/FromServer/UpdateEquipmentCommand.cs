namespace Mirage.Net.Protocol.FromServer;

public sealed record UpdateEquipmentCommand(UpdateEquipmentCommand.Slot? Weapon, UpdateEquipmentCommand.Slot? Armor, UpdateEquipmentCommand.Slot? Helmet, UpdateEquipmentCommand.Slot? Shield) : IPacket<UpdateEquipmentCommand>
{
    public sealed record Slot(int Sprite, string ItemName, int Damage, int Protection);

    public static string PacketId => nameof(UpdateEquipmentCommand);

    public static UpdateEquipmentCommand ReadFrom(PacketReader reader)
    {
        return new UpdateEquipmentCommand(
            Weapon: ReadSlot(reader),
            Armor: ReadSlot(reader),
            Helmet: ReadSlot(reader),
            Shield: ReadSlot(reader));
    }

    private static Slot? ReadSlot(PacketReader reader)
    {
        var exists = reader.ReadBoolean();
        if (!exists)
        {
            return null;
        }

        return new Slot(
            Sprite: reader.ReadInt32(),
            ItemName: reader.ReadString(),
            Damage: reader.ReadInt32(),
            Protection: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        WriteSlot(writer, Weapon);
        WriteSlot(writer, Armor);
        WriteSlot(writer, Helmet);
        WriteSlot(writer, Shield);
    }

    private static void WriteSlot(PacketWriter writer, Slot? slot)
    {
        if (slot is null)
        {
            writer.WriteBoolean(false);
            return;
        }

        writer.WriteBoolean(true);
        writer.WriteInt32(slot.Sprite);
        writer.WriteString(slot.ItemName);
        writer.WriteInt32(slot.Damage);
        writer.WriteInt32(slot.Protection);
    }
}