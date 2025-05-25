using Mirage.Shared.Data;

namespace Mirage.Client.Assets;

public sealed class MapManager() : AssetManager<NewMapInfo>(Placeholder)
{
    private static readonly NewMapInfo Placeholder = new();

    protected override NewMapInfo OnLoad(Stream stream)
    {
        return NewMapInfo.ReadFrom(stream);
    }
}