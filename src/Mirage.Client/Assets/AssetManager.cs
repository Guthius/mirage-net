namespace Mirage.Client.Assets;

public abstract class AssetManager<TAsset>(TAsset placeholder)
{
    private readonly Dictionary<string, Asset<TAsset>> _assets = new(StringComparer.OrdinalIgnoreCase);

    public Asset<TAsset> Get(string assetId, Action<TAsset>? afterLoad = null)
    {
        if (_assets.TryGetValue(assetId, out var asset))
        {
            return asset;
        }

        var path = GetPath(assetId, typeof(TAsset).Name.ToLowerInvariant());

        asset = _assets[assetId] = new Asset<TAsset>(placeholder);

        _ = Task.Run(async () =>
        {
            try
            {
                await using var stream = File.OpenRead(path);

                asset.Instance = Load(stream);

                afterLoad?.Invoke(asset.Instance);
            }
            catch (FileNotFoundException)
            {
                AssetDownloader.Download(assetId, stream =>
                {
                    File.WriteAllBytes(path, stream.ToArray());

                    stream.Position = 0;

                    asset.Instance = Load(stream);

                    afterLoad?.Invoke(asset.Instance);
                });
            }
        });

        return asset;
    }

    private static string GetPath(string fileName, string category)
    {
        var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        path = Path.Combine(path, "Mirage", "Cache");
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        return Path.Combine(path, fileName + "." + category);
    }

    private TAsset Load(Stream stream)
    {
        try
        {
            return OnLoad(stream);
        }
        catch
        {
            return placeholder;
        }
    }

    protected abstract TAsset OnLoad(Stream stream);
}