using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mirage.Shared.Data;

[BsonIgnoreExtraElements]
public sealed record ItemInfo : ObjectInfo
{
    [BsonElement("name"), BsonRepresentation(BsonType.String)]
    public string Name { get; set; } = string.Empty;

    [BsonElement("sprite"), BsonRepresentation(BsonType.Int32)]
    public int Sprite { get; set; }

    [BsonElement("type"), BsonRepresentation(BsonType.Int32)]
    public ItemType Type { get; set; } = ItemType.None;

    [BsonElement("data1"), BsonRepresentation(BsonType.Int32)]
    public int Data1 { get; set; }

    [BsonElement("data2"), BsonRepresentation(BsonType.Int32)]
    public int Data2 { get; set; }

    [BsonElement("data3"), BsonRepresentation(BsonType.Int32)]
    public int Data3 { get; set; }

    [BsonIgnore]
    public bool IsEquipment => Type is ItemType.Weapon or ItemType.Armor or ItemType.Helmet or ItemType.Shield;
}