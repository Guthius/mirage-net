namespace Mirage.Net.Protocol.FromClient;

public sealed record ShopTradeRequest(int Slot) : IPacket<ShopTradeRequest>
{
    public static string PacketId => "traderequest";
    
    public static ShopTradeRequest ReadFrom(PacketReader reader)
    {
        return new ShopTradeRequest(Slot: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(Slot);
    }
}