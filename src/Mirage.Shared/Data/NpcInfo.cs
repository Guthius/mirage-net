using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mirage.Shared.Data;

[BsonIgnoreExtraElements]
public sealed record NpcInfo
{
    [BsonElement("id"), BsonRepresentation(BsonType.Int32)]
    public int Id { get; set; }

    [BsonElement("name"), BsonRepresentation(BsonType.String)]
    public string Name { get; set; } = string.Empty;

    [BsonElement("attack_say"), BsonRepresentation(BsonType.String)]
    public string AttackSay { get; set; } = string.Empty;

    [BsonElement("sprite"), BsonRepresentation(BsonType.Int32)]
    public int Sprite { get; set; }

    [BsonElement("spawn_secs"), BsonRepresentation(BsonType.Int32)]
    public int SpawnSecs { get; set; }

    [BsonElement("behavior"), BsonRepresentation(BsonType.Int32)]
    public NpcBehavior Behavior { get; set; }

    [BsonElement("range"), BsonRepresentation(BsonType.Int32)]
    public int Range { get; set; }

    [BsonElement("drop_chance"), BsonRepresentation(BsonType.Int32)]
    public int DropChance { get; set; }

    [BsonElement("drop_item_id"), BsonRepresentation(BsonType.Int32)]
    public int DropItemId { get; set; }

    [BsonElement("drop_item_quantity"), BsonRepresentation(BsonType.Int32)]
    public int DropItemQuantity { get; set; }

    [BsonElement("strength"), BsonRepresentation(BsonType.Int32)]
    public int Strength { get; set; }

    [BsonElement("defense"), BsonRepresentation(BsonType.Int32)]
    public int Defense { get; set; }

    [BsonElement("speed"), BsonRepresentation(BsonType.Int32)]
    public int Speed { get; set; }

    [BsonElement("intelligence"), BsonRepresentation(BsonType.Int32)]
    public int Intelligence { get; set; }

    [BsonIgnore]
    public int MaxHealth => Strength * Defense;

    [BsonIgnore]
    public int MaxMana => Intelligence * 2;

    [BsonIgnore]
    public int MaxStamina => Speed * 2;

    [BsonIgnore]
    public int HealthRegen => Math.Max(1, Defense / 3);
}