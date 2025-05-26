using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mirage.Shared.Data;

public sealed class CharacterInventoryInfo
{
    [BsonElement("defense"), BsonRepresentation(BsonType.Int32)]
    public int Size { get; set; }

    [BsonElement("slots")]
    public List<InventorySlotInfo> Slots { get; set; } = [];
}