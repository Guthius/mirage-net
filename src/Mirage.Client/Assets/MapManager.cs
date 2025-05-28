using Mirage.Shared.Data;

namespace Mirage.Client.Assets;

public sealed class MapManager() : AssetManager<MapInfo>(Placeholder)
{
    private static readonly MapInfo Placeholder = new();

    protected override MapInfo OnLoad(Stream stream)
    {
        return MapInfo.ReadFrom(stream);
    }
}