using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mirage.Shared.Data;

public sealed record AccountInfo
{
    [BsonId]
    [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("name"), BsonRepresentation(BsonType.String)]
    public string Name { get; set; } = string.Empty;

    [BsonElement("password"), BsonRepresentation(BsonType.String)]
    public string Password { get; set; } = string.Empty;
}