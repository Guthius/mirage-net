using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Mirage.Server.Repositories.Characters.Data;

public sealed class CharacterInventoryInfo
{
    [BsonElement("size"), BsonRepresentation(BsonType.Int32)]
    public int Size { get; set; } = 20;

    [BsonElement("slots")]
    [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)]
    public Dictionary<int, CharacterInventorySlotInfo> Slots { get; set; } = [];

    [BsonElement("equipment")]
    public CharacterEquipmentInfo Equipment { get; set; } = new();
}