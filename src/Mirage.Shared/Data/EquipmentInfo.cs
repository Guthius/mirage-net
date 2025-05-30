using MongoDB.Bson.Serialization.Attributes;

namespace Mirage.Shared.Data;

public sealed class EquipmentInfo
{
    [BsonElement("weapon")]
    public EquipmentSlotInfo? Weapon { get; set; }

    [BsonElement("armor")]
    public EquipmentSlotInfo? Armor { get; set; }

    [BsonElement("helmet")]
    public EquipmentSlotInfo? Helmet { get; set; }

    [BsonElement("shield")]
    public EquipmentSlotInfo? Shield { get; set; }
}