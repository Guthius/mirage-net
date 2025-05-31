namespace Mirage.Net.Protocol.FromServer;

public sealed record MoveMapResponse(MoveMapResult Result) : IPacket<MoveMapResponse>
{
    public static string PacketId => nameof(MoveMapResponse);
    
    public static MoveMapResponse ReadFrom(PacketReader reader)
    {
        return new MoveMapResponse(Result: reader.ReadEnum<MoveMapResult>());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteEnum(Result);
    }
}