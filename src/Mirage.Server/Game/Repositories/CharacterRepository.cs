using LanguageExt;
using LanguageExt.Common;
using Mirage.Game.Constants;
using Mirage.Game.Data;
using MongoDB.Driver;

namespace Mirage.Server.Game.Repositories;

public static class CharacterRepository
{
    private static IMongoCollection<CharacterInfo> GetCollection()
    {
        return Database.GetCollection<CharacterInfo>("characters");
    }

    public static bool Exists(string characterName)
    {
        var count = GetCollection().CountDocuments(x => x.Name == characterName);

        return count > 0;
    }

    public static CharacterInfo? Get(string accountId, int slot)
    {
        return GetCollection()
            .Find(x => x.AccountId == accountId &&
                       x.Slot == slot)
            .FirstOrDefault();
    }

    public static List<CharacterSlotInfo> GetCharacterSlots(string accountId)
    {
        var projection = Builders<CharacterInfo>.Projection
            .Exclude("_id")
            .Include(x => x.Slot)
            .Include(x => x.Name)
            .Include(x => x.ClassId)
            .Include(x => x.Level);

        var characterSlotInfos = GetCollection()
            .Find(x => x.AccountId == accountId)
            .Project<CharacterSlotInfo>(projection)
            .ToList();

        foreach (var characterSlotInfo in characterSlotInfos)
        {
            characterSlotInfo.ClassName = ClassRepository.GetName(characterSlotInfo.ClassId);
        }

        return characterSlotInfos;
    }

    public static Option<Error> Create(string accountId, string characterName, Gender gender, int classId, int slot)
    {
        if (slot is < 1 or > Limits.MaxCharacters)
        {
            return Error.New("Invalid character slot");
        }

        var classInfo = ClassRepository.Get(classId);
        if (classInfo is null)
        {
            return Error.New("Invalid class");
        }

        if (Exists(characterName))
        {
            return Error.New("Sorry, but that name is in use!");
        }

        var characterInfo = new CharacterInfo
        {
            AccountId = accountId,
            Slot = slot,
            Name = characterName,
            Gender = gender,
            ClassId = classId,
            Sprite = classInfo.Sprite,
            Level = 1,
            AccessLevel = AccessLevel.Player,
            Strength = classInfo.Strength,
            Defense = classInfo.Defense,
            Speed = classInfo.Speed,
            Intelligence = classInfo.Intelligence,
            BaseStrength = classInfo.Strength,
            BaseDefense = classInfo.Defense,
            BaseSpeed = classInfo.Speed,
            BaseIntelligence = classInfo.Intelligence,
            MapId = Options.StartMap,
            X = Options.StartX,
            Y = Options.StartY,
            Direction = Direction.Down
        };

        characterInfo.HP = characterInfo.MaxHP;
        characterInfo.MP = characterInfo.MaxMP;
        characterInfo.SP = characterInfo.MaxSP;

        GetCollection().InsertOne(characterInfo);

        return Option<Error>.None;
    }

    public static void Save(CharacterInfo characterInfo)
    {
        GetCollection().ReplaceOne(x => x.AccountId == characterInfo.AccountId && x.Slot == characterInfo.Slot, characterInfo);
    }

    public static void Delete(string accountId, int slot)
    {
        GetCollection().DeleteOne(x => x.AccountId == accountId && x.Slot == slot);
    }

    public static void DeleteForAccount(string accountId)
    {
        GetCollection().DeleteMany(x => x.AccountId == accountId);
    }
}