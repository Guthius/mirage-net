namespace Mirage.Net.Protocol.FromClient.New;

public sealed record SearchRequest(int X, int Y) : IPacket<SearchRequest>
{
    public static string PacketId => "search";
    
    public static SearchRequest ReadFrom(PacketReader reader)
    {
        return new SearchRequest(X: reader.ReadInt32(), Y: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(X);
        writer.WriteInt32(Y);
    }
}