using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mirage.Shared.Data;

public sealed record MapInfo
{
    [BsonId]
    [BsonElement("id"), BsonRepresentation(BsonType.Int32)]
    public int Id { get; set; }

    [BsonElement("name"), BsonRepresentation(BsonType.String)]
    public string Name { get; set; } = string.Empty;

    [BsonElement("moral"), BsonRepresentation(BsonType.Int32)]
    public MapMoral Moral { get; set; } = MapMoral.None;

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
}