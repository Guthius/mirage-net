using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mirage.Shared.Data;

public sealed record ShopTradeInfo
{
    [BsonElement("give_item_id"), BsonRepresentation(BsonType.Int32)]
    public int GiveItemId { get; set; }

    [BsonElement("give_item_quantity"), BsonRepresentation(BsonType.Int32)]
    public int GiveItemQuantity { get; set; }

    [BsonElement("get_item_id"), BsonRepresentation(BsonType.Int32)]
    public int GetItemId { get; set; }

    [BsonElement("get_item_quantity"), BsonRepresentation(BsonType.Int32)]
    public int GetItemQuantity { get; set; }
}