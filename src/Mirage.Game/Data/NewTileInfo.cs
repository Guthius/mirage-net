using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mirage.Game.Data;

public struct NewTileInfo
{
    [BsonElement("type"), BsonRepresentation(BsonType.Int32)]
    public TileType Type { get; set; }

    [BsonElement("data1"), BsonRepresentation(BsonType.Int32)]
    public int Data1 { get; set; }

    [BsonElement("data2"), BsonRepresentation(BsonType.Int32)]
    public int Data2 { get; set; }

    [BsonElement("data3"), BsonRepresentation(BsonType.Int32)]
    public int Data3 { get; set; }
}