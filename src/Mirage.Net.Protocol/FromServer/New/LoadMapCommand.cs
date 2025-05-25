namespace Mirage.Net.Protocol.FromServer.New;

public sealed record LoadMapCommand(string MapId) : IPacket<LoadMapCommand>
{
    public static string PacketId => nameof(LoadMapCommand);

    public static LoadMapCommand ReadFrom(PacketReader reader)
    {
        return new LoadMapCommand(MapId: reader.ReadString());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteString(MapId);
    }
}