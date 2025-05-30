using MongoDB.Bson.Serialization.Attributes;

namespace Mirage.Shared.Data;

public sealed class EquipmentSlotInfo
{
    [BsonElement("item_id")]
    public string ItemId { get; set; } = string.Empty;
    
    [BsonElement("durability")]
    public int Durability { get; set; }
}