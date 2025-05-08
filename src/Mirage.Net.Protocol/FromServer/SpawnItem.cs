namespace Mirage.Net.Protocol.FromServer;

public sealed record SpawnItem(int Slot, int ItemId, int Quantity, int Durability, int X, int Y) : IPacket<SpawnItem>
{
    public static string PacketId => "spawnitem";

    public static SpawnItem ReadFrom(PacketReader reader)
    {
        return new SpawnItem(
            Slot: reader.ReadInt32(),
            ItemId: reader.ReadInt32(),
            Quantity: reader.ReadInt32(),
            Durability: reader.ReadInt32(),
            X: reader.ReadInt32(),
            Y: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(Slot);
        writer.WriteInt32(ItemId);
        writer.WriteInt32(Quantity);
        writer.WriteInt32(Durability);
        writer.WriteInt32(X);
        writer.WriteInt32(Y);
    }

    public static SpawnItem Cleared(int slot)
    {
        return new SpawnItem(slot, 0, 0, 0, 0, 0);
    }
}