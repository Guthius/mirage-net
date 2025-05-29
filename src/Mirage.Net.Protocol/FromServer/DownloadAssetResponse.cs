namespace Mirage.Net.Protocol.FromServer;

public sealed record DownloadAssetResponse(int Handle, DownloadAssetResult Result) : IPacket<DownloadAssetResponse>
{
    public static string PacketId => nameof(DownloadAssetResponse);

    public static DownloadAssetResponse ReadFrom(PacketReader reader)
    {
        return new DownloadAssetResponse(
            Handle: reader.ReadInt32(),
            Result: reader.ReadEnum<DownloadAssetResult>());
    }

    public void WriteTo(PacketWriter writer)
    {
        writer.WriteInt32(Handle);
        writer.WriteEnum(Result);
    }
}