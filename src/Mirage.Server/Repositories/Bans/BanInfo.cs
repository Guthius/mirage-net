using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mirage.Server.Repositories.Bans;

[BsonIgnoreExtraElements]
public sealed record BanInfo
{
    [BsonElement("ip"), BsonRepresentation(BsonType.String)]
    public string Ip { get; set; } = string.Empty;

    [BsonElement("banned_by"), BsonRepresentation(BsonType.String)]
    public string BannedBy { get; set; } = string.Empty;
}