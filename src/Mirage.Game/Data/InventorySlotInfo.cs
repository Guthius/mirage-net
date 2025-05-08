using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mirage.Game.Data;

public sealed record InventorySlotInfo
{
    [BsonElement("item_id"), BsonRepresentation(BsonType.Int32)]
    public int ItemId { get; set; }

    [BsonElement("quantity"), BsonRepresentation(BsonType.Int32)]
    public int Quantity { get; set; }

    [BsonElement("durability"), BsonRepresentation(BsonType.Int32)]
    public int Durability { get; set; }
}