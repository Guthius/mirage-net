using MongoDB.Bson.Serialization.Attributes;

namespace Mirage.Server.Repositories.Characters.Data;

public sealed class CharacterEquipmentInfo
{
    [BsonElement("weapon")]
    public CharacterEquipmentSlotInfo? Weapon { get; set; }

    [BsonElement("armor")]
    public CharacterEquipmentSlotInfo? Armor { get; set; }

    [BsonElement("helmet")]
    public CharacterEquipmentSlotInfo? Helmet { get; set; }

    [BsonElement("shield")]
    public CharacterEquipmentSlotInfo? Shield { get; set; }
}