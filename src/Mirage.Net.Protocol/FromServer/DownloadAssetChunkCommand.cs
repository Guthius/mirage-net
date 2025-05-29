namespace Mirage.Net.Protocol.FromServer;

public sealed record DownloadAssetChunkCommand(int Handle, byte[] Data) : IPacket<DownloadAssetChunkCommand>
{
    public static string PacketId => nameof(DownloadAssetChunkCommand);

    public static DownloadAssetChunkCommand ReadFrom(PacketReader reader)
    {
        return new DownloadAssetChunkCommand(
            Handle: reader.ReadInt32(),
            Data: reader.ReadBytes());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(Handle);
        writer.WriteBytes(Data);
    }
}