using Mirage.Shared.Data;

namespace Mirage.Net.Protocol.FromServer;

public sealed record EditNpc(NpcInfo NpcInfo) : IPacket<EditNpc>
{
    public static string PacketId => "editnpc";

    public static EditNpc ReadFrom(PacketReader reader)
    {
        return new EditNpc(new NpcInfo
        {
            Id = reader.ReadInt32(),
            Name = reader.ReadString(),
            AttackSay = reader.ReadString(),
            Sprite = reader.ReadInt32(),
            SpawnSecs = reader.ReadInt32(),
            Behavior = reader.ReadEnum<NpcBehavior>(),
            Range = reader.ReadInt32(),
            DropChance = reader.ReadInt32(),
            DropItemId = reader.ReadInt32(),
            DropItemQuantity = reader.ReadInt32(),
            Strength = reader.ReadInt32(),
            Defense = reader.ReadInt32(),
            Speed = reader.ReadInt32(),
            Intelligence = reader.ReadInt32()
        });
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(NpcInfo.Id);
        writer.WriteString(NpcInfo.Name);
        writer.WriteString(NpcInfo.AttackSay);
        writer.WriteInt32(NpcInfo.Sprite);
        writer.WriteInt32(NpcInfo.SpawnSecs);
        writer.WriteEnum(NpcInfo.Behavior);
        writer.WriteInt32(NpcInfo.Range);
        writer.WriteInt32(NpcInfo.DropChance);
        writer.WriteInt32(NpcInfo.DropItemId);
        writer.WriteInt32(NpcInfo.DropItemQuantity);
        writer.WriteInt32(NpcInfo.Strength);
        writer.WriteInt32(NpcInfo.Defense);
        writer.WriteInt32(NpcInfo.Speed);
        writer.WriteInt32(NpcInfo.Intelligence);
    }
}