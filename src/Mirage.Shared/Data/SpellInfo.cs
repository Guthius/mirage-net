using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mirage.Shared.Data;

[BsonIgnoreExtraElements]
public sealed record SpellInfo
{
    [BsonElement("id"), BsonRepresentation(BsonType.Int32)]
    public int Id { get; set; }

    [BsonElement("name"), BsonRepresentation(BsonType.String)]
    public string Name { get; set; } = string.Empty;

    [BsonElement("req_class_id"), BsonRepresentation(BsonType.String)]
    public string RequiredClassId { get; set; } = string.Empty;

    [BsonElement("req_level"), BsonRepresentation(BsonType.Int32)]
    public int RequiredLevel { get; set; }

    [BsonIgnore]
    public int RequiredMp => RequiredLevel + Data1 + Data2 + Data3;

    [BsonElement("type"), BsonRepresentation(BsonType.Int32)]
    public SpellType Type { get; set; }

    [BsonElement("data1"), BsonRepresentation(BsonType.Int32)]
    public int Data1 { get; set; }

    [BsonElement("data2"), BsonRepresentation(BsonType.Int32)]
    public int Data2 { get; set; }

    [BsonElement("data3"), BsonRepresentation(BsonType.Int32)]
    public int Data3 { get; set; }
}