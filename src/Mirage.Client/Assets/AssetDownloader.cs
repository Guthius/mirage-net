using System.Collections.Concurrent;
using Mirage.Client.Net;
using Mirage.Net.Protocol.FromClient;

namespace Mirage.Client.Assets;

public static class AssetDownloader
{
    private sealed record DownloadInfo(MemoryStream Stream, AssetCallback Callback);
    
    private static readonly ConcurrentDictionary<int, DownloadInfo> ActiveDownloads = new();
    
    private static int _nextHandle;
    
    public static void Download(string hash, AssetCallback callback)
    {
        var handle = Interlocked.Increment(ref _nextHandle);
        
        ActiveDownloads[handle] = new DownloadInfo(new MemoryStream(), callback);
        
        Network.Send(new DownloadAssetRequest(handle, hash));
    }

    public static void WriteChunk(int handle, ReadOnlySpan<byte> data)
    {
        if (ActiveDownloads.TryGetValue(handle, out var downloadInfo))
        {
            downloadInfo.Stream.Write(data);
        }
    }

    public static void End(int handle)
    {
        if (!ActiveDownloads.Remove(handle, out var downloadInfo))
        {
            return;
        }
        
        downloadInfo.Callback(downloadInfo.Stream);
        downloadInfo.Stream.Dispose();
    }
}