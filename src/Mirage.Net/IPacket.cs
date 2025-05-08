namespace Mirage.Net;

public interface IPacket<out TSelf>
{
    static abstract string PacketId { get; }
    
    static abstract TSelf ReadFrom(PacketReader reader);
    
    void WriteTo(PacketWriter writer);
}