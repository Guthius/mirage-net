using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mirage.Shared.Data;

public sealed record MapLayerInfo
{
    [BsonElement("name"), BsonRepresentation(BsonType.String)]
    public string Name { get; set; } = string.Empty;
    
    [BsonElement("tiles")]
    public int[] Tiles { get; set; } = [];
}