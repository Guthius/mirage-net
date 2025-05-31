using MongoDB.Bson.Serialization.Attributes;

namespace Mirage.Server.Repositories.Characters.Data;

public sealed class CharacterEquipmentSlotInfo
{
    [BsonElement("item_id")]
    public string ItemId { get; set; } = string.Empty;
    
    [BsonElement("durability")]
    public int Durability { get; set; }
}