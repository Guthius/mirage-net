namespace Mirage.Net.Protocol.FromServer;

public sealed record UpdateInventoryCommand(int InventorySize) : IPacket<UpdateInventoryCommand>
{
    public static string PacketId => nameof(UpdateInventoryCommand);

    public static UpdateInventoryCommand ReadFrom(PacketReader reader)
    {
        return new UpdateInventoryCommand(InventorySize: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(InventorySize);
    }
}