using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mirage.Game.Data;

public sealed record TileInfo
{
    [BsonElement("ground"), BsonRepresentation(BsonType.Int32)]
    public int Ground { get; set; }

    [BsonElement("mask"), BsonRepresentation(BsonType.Int32)]
    public int Mask { get; set; }

    [BsonElement("anim"), BsonRepresentation(BsonType.Int32)]
    public int Anim { get; set; }

    [BsonElement("fringe"), BsonRepresentation(BsonType.Int32)]
    public int Fringe { get; set; }

    [BsonElement("type"), BsonRepresentation(BsonType.Int32)]
    public TileType Type { get; set; } = TileType.Walkable;

    [BsonElement("data1"), BsonRepresentation(BsonType.Int32)]
    public int Data1 { get; set; }

    [BsonElement("data2"), BsonRepresentation(BsonType.Int32)]
    public int Data2 { get; set; }

    [BsonElement("data3"), BsonRepresentation(BsonType.Int32)]
    public int Data3 { get; set; }
}