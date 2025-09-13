using Mirage.Shared.Data;
using Terestrium.Client.Assets;

namespace Terestrium.Client.Maps;

public sealed class MapManager() : AssetManager<MapInfo>(Placeholder)
{
    private static readonly MapInfo Placeholder = new();

    protected override MapInfo OnLoad(Stream stream)
    {
        return MapInfo.ReadFrom(stream);
    }
}