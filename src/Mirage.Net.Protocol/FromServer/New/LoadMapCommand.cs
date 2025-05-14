namespace Mirage.Net.Protocol.FromServer.New;

public sealed record LoadMapCommand(string MapName, int Revision) : IPacket<LoadMapCommand>
{
    public static string PacketId => nameof(LoadMapCommand);

    public static LoadMapCommand ReadFrom(PacketReader reader)
    {
        return new LoadMapCommand(
            MapName: reader.ReadString(),
            Revision: reader.ReadInt32());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteString(MapName);
        writer.WriteInt32(Revision);
    }
}