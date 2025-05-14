using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mirage.Game.Data;

public sealed record CharacterSlotInfo
{
    [BsonId]
    [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
    public string CharacterId { get; set; } = string.Empty;

    [BsonElement("name"), BsonRepresentation(BsonType.String)]
    public string Name { get; set; } = string.Empty;

    [BsonElement("job_id"), BsonRepresentation(BsonType.String)]
    public string JobId { get; set; } = string.Empty;

    [BsonElement("level"), BsonRepresentation(BsonType.Int32)]
    public int Level { get; set; } = 1;
}