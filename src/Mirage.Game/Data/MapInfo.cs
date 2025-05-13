using Mirage.Game.Constants;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mirage.Game.Data;

public sealed record MapInfo
{
    [BsonId]
    [BsonElement("id"), BsonRepresentation(BsonType.Int32)]
    public int Id { get; set; }

    [BsonElement("name"), BsonRepresentation(BsonType.String)]
    public string Name { get; set; } = string.Empty;

    [BsonElement("revision"), BsonRepresentation(BsonType.Int32)]
    public int Revision { get; set; }

    [BsonElement("moral"), BsonRepresentation(BsonType.Int32)]
    public MapMoral Moral { get; set; } = MapMoral.None;

    [BsonElement("up"), BsonRepresentation(BsonType.Int32)]
    public int Up { get; set; }

    [BsonElement("down"), BsonRepresentation(BsonType.Int32)]
    public int Down { get; set; }

    [BsonElement("left"), BsonRepresentation(BsonType.Int32)]
    public int Left { get; set; }

    [BsonElement("right"), BsonRepresentation(BsonType.Int32)]
    public int Right { get; set; }

    [BsonElement("music"), BsonRepresentation(BsonType.Int32)]
    public int Music { get; set; }

    [BsonElement("boot_map_id"), BsonRepresentation(BsonType.Int32)]
    public int BootMapId { get; set; }

    [BsonElement("boot_x"), BsonRepresentation(BsonType.Int32)]
    public int BootX { get; set; }

    [BsonElement("boot_y"), BsonRepresentation(BsonType.Int32)]
    public int BootY { get; set; }

    [BsonElement("shop_id"), BsonRepresentation(BsonType.Int32)]
    public int ShopId { get; set; }

    [BsonElement("tiles")]
    public TileInfo[,] Tiles { get; set; } = CreateTiles(Limits.MaxMapWidth, Limits.MaxMapHeight);

    [BsonElement("npc_ids")]
    public int[] NpcIds { get; set; } = CreateNpcIds();

    private static TileInfo[,] CreateTiles(int width, int height)
    {
        var tiles = new TileInfo[width + 1, height + 1];

        for (var y = 0; y <= height; y++)
        {
            for (var x = 0; x <= width; x++)
            {
                tiles[x, y] = new TileInfo();
            }
        }

        return tiles;
    }

    private static int[] CreateNpcIds()
    {
        return new int[Limits.MaxMapNpcs + 1];
    }
    
    public (int MapId, int X, int Y) GetAdjacentMap(Direction direction, int x, int y)
    {
        return direction switch
        {
            Direction.Up => (Up, x, Limits.MaxMapHeight),
            Direction.Down => (Down, x, 0),
            Direction.Left => (Left, Limits.MaxMapWidth, y),
            Direction.Right => (Right, 0, y),
            _ => (0, x, y)
        };
    }
}