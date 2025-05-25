using System.Collections.Concurrent;
using Mirage.Client.Net;
using Mirage.Net.Protocol.FromClient.New;

namespace Mirage.Client.Assets;

public delegate void AssetCallback(MemoryStream stream);

public static class AssetDownloader
{
    private static readonly ConcurrentDictionary<int, State> ActiveDownloads = new();
    
    private static int _nextHandle;
    
    private sealed record State(MemoryStream Stream, AssetCallback Callback);
    
    public static void Download(string hash, AssetCallback callback)
    {
        var handle = Interlocked.Increment(ref _nextHandle);
        
        ActiveDownloads[handle] = new State(new MemoryStream(), callback);
        
        Network.Send(new DownloadAssetRequest(handle, hash));
    }

    public static void WriteChunk(int handle, ReadOnlySpan<byte> data)
    {
        if (ActiveDownloads.TryGetValue(handle, out var state))
        {
            state.Stream.Write(data);
        }
    }

    public static void End(int handle)
    {
        if (!ActiveDownloads.Remove(handle, out var state))
        {
            return;
        }
        
        state.Callback(state.Stream);
        state.Stream.Dispose();
    }
}