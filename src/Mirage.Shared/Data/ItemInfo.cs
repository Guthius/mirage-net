using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mirage.Shared.Data;

public sealed record ItemInfo : ObjectInfo
{
    [BsonElement("name"), BsonRepresentation(BsonType.String)]
    public string Name { get; set; } = string.Empty;

    [BsonElement("sprite"), BsonRepresentation(BsonType.Int32)]
    public int Sprite { get; set; }

    [BsonElement("type"), BsonRepresentation(BsonType.Int32)]
    public ItemType Type { get; set; } = ItemType.None;

    [BsonElement("durability"), BsonRepresentation(BsonType.Int32)]
    public int Durability { get; set; }

    [BsonElement("damage"), BsonRepresentation(BsonType.Int32)]
    public int Damage { get; set; }

    [BsonElement("protection"), BsonRepresentation(BsonType.Int32)]
    public int Protection { get; set; }

    [BsonElement("req_strength"), BsonRepresentation(BsonType.Int32)]
    public int RequiredStrength { get; set; }

    [BsonElement("req_defense"), BsonRepresentation(BsonType.Int32)]
    public int RequiredDefense { get; set; }

    [BsonElement("req_speed"), BsonRepresentation(BsonType.Int32)]
    public int RequiredSpeed { get; set; }

    [BsonElement("skill_id"), BsonRepresentation(BsonType.Int32)]
    public int SpellId { get; set; }

    [BsonElement("potion_strength"), BsonRepresentation(BsonType.Int32)]
    public int PotionStrength { get; set; }

    [BsonIgnore]
    public bool IsEquipment => Type is ItemType.Weapon or ItemType.Armor or ItemType.Helmet or ItemType.Shield;
}