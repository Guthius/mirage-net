using Mirage.Game.Constants;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mirage.Game.Data;

public sealed record NewMapInfo
{
    [BsonId]
    [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("name"), BsonRepresentation(BsonType.String)]
    public string Name { get; set; } = string.Empty;

    [BsonElement("revision"), BsonRepresentation(BsonType.Int32)]
    public int Revision { get; set; }

    [BsonElement("width"), BsonRepresentation(BsonType.Int32)]
    public int Width { get; set; }

    [BsonElement("height"), BsonRepresentation(BsonType.Int32)]
    public int Height { get; set; }

    [BsonElement("layers")]
    public List<MapLayerInfo> Layers { get; set; } = [];

    [BsonElement("tiles")]
    public NewTileInfo[] Tiles { get; set; } = [];

    /// <summary>
    /// Checks whether the specified coordinates are within the bounds of the map.
    /// </summary>
    /// <param name="x">The X position.</param>
    /// <param name="y">The Y position.</param>
    /// <returns></returns>
    public bool InBounds(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    public NewTileInfo GetTile(int x, int y)
    {
        if (!InBounds(x, y))
        {
            throw new IndexOutOfRangeException();
        }

        return Tiles[y * Width + x];
    }

    public TileType GetTileType(int x, int y)
    {
        if (!InBounds(x, y))
        {
            return TileType.Blocked;
        }

        return Tiles[y * Width + x].Type;
    }

    public NewTileInfo this[int x, int y] => GetTile(x, y);
}