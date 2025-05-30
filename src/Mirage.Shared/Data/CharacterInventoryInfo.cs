using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Mirage.Shared.Data;

public sealed class CharacterInventoryInfo
{
    [BsonElement("size"), BsonRepresentation(BsonType.Int32)]
    public int Size { get; set; } = 20;

    [BsonElement("slots")]
    [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)]
    public Dictionary<int, InventorySlotInfo> Slots { get; set; } = [];

    [BsonElement("equipment")]
    public EquipmentInfo Equipment { get; set; } = new();
}