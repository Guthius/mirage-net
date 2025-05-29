using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mirage.Shared.Data;

public sealed record CharacterInfo
{
    [BsonId]
    [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("account_id"), BsonRepresentation(BsonType.String)]
    public string AccountId { get; set; } = string.Empty;

    [BsonElement("name"), BsonRepresentation(BsonType.String)]
    public string Name { get; set; } = string.Empty;

    [BsonElement("gender"), BsonRepresentation(BsonType.Int32)]
    public Gender Gender { get; set; }

    [BsonElement("job_id"), BsonRepresentation(BsonType.String)]
    public string JobId { get; set; } = string.Empty;

    [BsonElement("sprite"), BsonRepresentation(BsonType.Int32)]
    public int Sprite { get; set; }

    [BsonElement("level"), BsonRepresentation(BsonType.Int32)]
    public int Level { get; set; } = 1;

    [BsonElement("exp"), BsonRepresentation(BsonType.Int32)]
    public int Exp { get; set; }

    [BsonElement("access_level"), BsonRepresentation(BsonType.Int32)]
    public AccessLevel AccessLevel { get; set; } = AccessLevel.None;

    [BsonElement("player_killer"), BsonRepresentation(BsonType.Boolean)]
    public bool PlayerKiller { get; set; }

    [BsonElement("health"), BsonRepresentation(BsonType.Int32)]
    public int Health { get; set; }

    [BsonElement("mana"), BsonRepresentation(BsonType.Int32)]
    public int Mana { get; set; }

    [BsonElement("stamina"), BsonRepresentation(BsonType.Int32)]
    public int Stamina { get; set; }

    [BsonElement("strength"), BsonRepresentation(BsonType.Int32)]
    public int Strength { get; set; }

    [BsonElement("defense"), BsonRepresentation(BsonType.Int32)]
    public int Defense { get; set; }

    [BsonElement("speed"), BsonRepresentation(BsonType.Int32)]
    public int Speed { get; set; }

    [BsonElement("intelligence"), BsonRepresentation(BsonType.Int32)]
    public int Intelligence { get; set; }

    [BsonElement("base_strength"), BsonRepresentation(BsonType.Int32)]
    public int BaseStrength { get; set; }

    [BsonElement("base_defense"), BsonRepresentation(BsonType.Int32)]
    public int BaseDefense { get; set; }

    [BsonElement("base_speed"), BsonRepresentation(BsonType.Int32)]
    public int BaseSpeed { get; set; }

    [BsonElement("base_intelligence"), BsonRepresentation(BsonType.Int32)]
    public int BaseIntelligence { get; set; }

    [BsonElement("stat_points"), BsonRepresentation(BsonType.Int32)]
    public int StatPoints { get; set; }

    [BsonElement("inventory")]
    public CharacterInventoryInfo Inventory { get; set; } = new();

    [BsonElement("map"), BsonRepresentation(BsonType.String)]
    public string Map { get; set; } = string.Empty;

    [BsonElement("x"), BsonRepresentation(BsonType.Int32)]
    public int X { get; set; }

    [BsonElement("y"), BsonRepresentation(BsonType.Int32)]
    public int Y { get; set; }

    [BsonElement("direction"), BsonRepresentation(BsonType.Int32)]
    public Direction Direction { get; set; }

    [BsonIgnore]
    public int MaxHealth => (Level + Strength / 2 + BaseStrength) * 2;

    [BsonIgnore]
    public int MaxMana => (Level + Intelligence / 2 + BaseIntelligence) * 2;

    [BsonIgnore]
    public int MaxStamina => (Level + Speed / 2 + BaseSpeed) * 2;

    [BsonIgnore]
    public int CriticalHitRate => Math.Min(100, Strength / 2 + Level / 2);

    [BsonIgnore]
    public int BlockRate => Math.Min(100, Defense / 2 + Level / 2);

    [BsonIgnore]
    public int RequiredExp => (Level + 1) * (Strength + Defense + Intelligence + Speed + StatPoints) * 25;

    [BsonIgnore]
    public int HealthRegen => Math.Min(2, Defense / 2);

    [BsonIgnore]
    public int ManaRegen => Math.Min(2, Intelligence / 2);

    [BsonIgnore]
    public int StaminaRegen => Math.Min(2, Speed / 2);
}