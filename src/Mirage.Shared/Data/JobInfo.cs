using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mirage.Shared.Data;

public sealed record JobInfo
{
    [BsonId]
    [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;
    
    [BsonElement("name"), BsonRepresentation(BsonType.String)]
    public string Name { get; set; } = string.Empty;

    [BsonElement("sprite"), BsonRepresentation(BsonType.Int32)]
    public int Sprite { get; set; }

    [BsonElement("strength"), BsonRepresentation(BsonType.Int32)]
    public int Strength { get; set; }

    [BsonElement("defense"), BsonRepresentation(BsonType.Int32)]
    public int Defense { get; set; }

    [BsonElement("speed"), BsonRepresentation(BsonType.Int32)]
    public int Speed { get; set; }

    [BsonElement("intelligence"), BsonRepresentation(BsonType.Int32)]
    public int Intelligence { get; set; }

    [BsonIgnore]
    public int MaxHP => (1 + Strength / 2 + Strength) * 2;

    [BsonIgnore]
    public int MaxMP => (1 + Intelligence / 2 + Intelligence) * 2;

    [BsonIgnore]
    public int MaxSP => (1 + Speed / 2 + Speed) * 2;
}