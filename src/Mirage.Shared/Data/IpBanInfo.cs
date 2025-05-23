using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mirage.Shared.Data;

[BsonIgnoreExtraElements]
public sealed record IpBanInfo
{
    [BsonElement("ip"), BsonRepresentation(BsonType.String)]
    public string Ip { get; set; } = string.Empty;

    [BsonElement("banned_by"), BsonRepresentation(BsonType.String)]
    public string BannedBy { get; set; } = string.Empty;
}