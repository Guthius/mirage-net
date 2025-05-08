using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mirage.Game.Data;

public sealed record CharacterSlotInfo
{
    [BsonElement("slot"), BsonRepresentation(BsonType.Int32)]
    public int Slot { get; set; }

    [BsonElement("name"), BsonRepresentation(BsonType.String)]
    public string Name { get; set; } = string.Empty;
    
    [BsonElement("class_id"), BsonRepresentation(BsonType.Int32)]
    public int ClassId { get; set; }
    
    [BsonIgnore]
    public string ClassName { get; set; } = string.Empty;
    
    [BsonElement("level"), BsonRepresentation(BsonType.Int32)]
    public int Level { get; set; } = 1;
}