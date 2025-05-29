using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mirage.Shared.Data;

public sealed record InventorySlotInfo
{
    [BsonElement("item_id"), BsonRepresentation(BsonType.String)]
    public string ItemId { get; set; } = string.Empty;

    [BsonElement("quantity"), BsonRepresentation(BsonType.Int32)]
    public int Quantity { get; set; }

    [BsonElement("durability"), BsonRepresentation(BsonType.Int32)]
    public int Durability { get; set; }
}