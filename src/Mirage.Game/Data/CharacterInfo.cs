using Mirage.Game.Constants;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mirage.Game.Data;

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
    public AccessLevel AccessLevel { get; set; } = AccessLevel.Player;

    [BsonElement("player_killer"), BsonRepresentation(BsonType.Boolean)]
    public bool PlayerKiller { get; set; }

    [BsonElement("hp"), BsonRepresentation(BsonType.Int32)]
    public int HP
    {
        get;
        set => field = Math.Clamp(value, 0, MaxHP);
    }

    [BsonElement("mp"), BsonRepresentation(BsonType.Int32)]
    public int MP
    {
        get;
        set => field = Math.Clamp(value, 0, MaxMP);
    }

    [BsonElement("sp"), BsonRepresentation(BsonType.Int32)]
    public int SP
    {
        get;
        set => field = Math.Clamp(value, 0, MaxSP);
    }

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

    [BsonElement("armor_slot"), BsonRepresentation(BsonType.Int32)]
    public int ArmorSlot { get; set; }

    [BsonElement("weapon_slot"), BsonRepresentation(BsonType.Int32)]
    public int WeaponSlot { get; set; }

    [BsonElement("helmet_slot"), BsonRepresentation(BsonType.Int32)]
    public int HelmetSlot { get; set; }

    [BsonElement("shield_slot"), BsonRepresentation(BsonType.Int32)]
    public int ShieldSlot { get; set; }

    [BsonElement("inventory")]
    public InventorySlotInfo[] Inventory { get; set; } = CreateInventory();

    [BsonElement("spell_ids")]
    public int[] SpellIds { get; set; } = CreateSpellIds();

    [BsonElement("map_id"), BsonRepresentation(BsonType.Int32)]
    public int MapId { get; set; }

    [BsonElement("map"), BsonRepresentation(BsonType.String)]
    public string Map { get; set; } = string.Empty;

    [BsonElement("x"), BsonRepresentation(BsonType.Int32)]
    public int X { get; set; }

    [BsonElement("y"), BsonRepresentation(BsonType.Int32)]
    public int Y { get; set; }

    [BsonElement("direction"), BsonRepresentation(BsonType.Int32)]
    public Direction Direction { get; set; }

    [BsonIgnore]
    public int MaxHP => (Level + Strength / 2 + BaseStrength) * 2;

    [BsonIgnore]
    public int MaxMP => (Level + Intelligence / 2 + BaseIntelligence) * 2;

    [BsonIgnore]
    public int MaxSP => (Level + Speed / 2 + BaseSpeed) * 2;

    [BsonIgnore]
    public int CriticalHitRate => Math.Min(100, Strength / 2 + Level / 2);

    [BsonIgnore]
    public int BlockRate => Math.Min(100, Defense / 2 + Level / 2);

    [BsonIgnore]
    public int RequiredExp => (Level + 1) * (Strength + Defense + Intelligence + Speed + StatPoints) * 25;

    [BsonIgnore]
    public int HPRegen => Math.Min(2, Defense / 2);

    [BsonIgnore]
    public int MPRegen => Math.Min(2, Intelligence / 2);

    [BsonIgnore]
    public int SPRegen => Math.Min(2, Speed / 2);

    private static InventorySlotInfo[] CreateInventory()
    {
        var inventory = new InventorySlotInfo[Limits.MaxInventory + 1];

        for (var slot = 0; slot < inventory.Length; slot++)
        {
            inventory[slot] = new InventorySlotInfo();
        }

        return inventory;
    }

    private static int[] CreateSpellIds()
    {
        return new int[Limits.MaxPlayerSpells + 1];
    }
}