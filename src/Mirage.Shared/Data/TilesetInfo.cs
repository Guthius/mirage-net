namespace Mirage.Shared.Data;

public sealed record TilesetInfo
{
    public string Id { get; set; } = string.Empty;
    public int FirstGid { get; set; }
    public int TileWidth { get; set; }
    public int TileHeight { get; set; }
}