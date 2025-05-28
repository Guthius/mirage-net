using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mirage.Shared.Data;

public abstract record ObjectInfo
{
    [BsonId]
    [BsonElement("id"), BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;
}