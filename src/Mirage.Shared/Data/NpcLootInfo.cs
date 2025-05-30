using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mirage.Shared.Data;

public sealed record NpcLootInfo
{
    [BsonElement("item_id"), BsonRepresentation(BsonType.String)]
    public string ItemId { get; set; } = string.Empty;

    [BsonElement("drop_rate"), BsonRepresentation(BsonType.Double)]
    public float DropRatePercentage { get; set; }

    [BsonElement("min_quantity"), BsonRepresentation(BsonType.Int32)]
    public int MinQuantity { get; set; }

    [BsonElement("max_quantity"), BsonRepresentation(BsonType.Int32)]
    public int MaxQuantity { get; set; }
}