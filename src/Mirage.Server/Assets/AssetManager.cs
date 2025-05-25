using System.Security.Cryptography;
using Serilog;

namespace Mirage.Server.Assets;

public static class AssetManager
{
    private static readonly Dictionary<string, Asset> Assets = new(StringComparer.OrdinalIgnoreCase);

    public static Asset Register(string id, string path)
    {
        return Assets[id] = new Asset(path, id);
    }

    public static Asset? Register(string path)
    {
        try
        {
            return Register(ComputeHash(path), path);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to register asset {FileName}", path);

            return null;
        }
    }

    public static string ComputeHash(string path)
    {
        using var stream = File.OpenRead(path);

        var sha1HashBytes = SHA1.HashData(stream);
        var sha1Hash = string.Concat(sha1HashBytes.Select(b => b.ToString("x2")));

        return sha1Hash;
    }

    public static Asset? Get(string hash)
    {
        return Assets.GetValueOrDefault(hash);
    }
}