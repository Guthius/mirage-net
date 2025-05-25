namespace Mirage.Net.Protocol.FromClient.New;

public sealed record DownloadAssetRequest(int Handle, string Hash) : IPacket<DownloadAssetRequest>
{
    public static string PacketId => nameof(DownloadAssetRequest);

    public static DownloadAssetRequest ReadFrom(PacketReader reader)
    {
        return new DownloadAssetRequest(
            Handle: reader.ReadInt32(), 
            Hash: reader.ReadString());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(Handle);
        writer.WriteString(Hash);
    }
}