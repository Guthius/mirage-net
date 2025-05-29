using Mirage.Net.Protocol.FromServer.New;
using Mirage.Server.Repositories.Jobs;
using Mirage.Shared.Constants;
using Mirage.Shared.Data;
using MongoDB.Driver;

namespace Mirage.Server.Repositories.Characters;

public sealed class CharacterRepository(IJobRepository jobRepository) : ICharacterRepository
{
    private static IMongoCollection<CharacterInfo> GetCollection()
    {
        return Database.GetCollection<CharacterInfo>("characters");
    }

    private static bool Exists(string characterName)
    {
        var count = GetCollection().CountDocuments(x => x.Name == characterName);

        return count > 0;
    }

    public CharacterInfo? Get(string characterId, string accountId)
    {
        return GetCollection()
            .Find(x => x.Id == characterId && x.AccountId == accountId)
            .FirstOrDefault();
    }

    public List<CharacterSlotInfo> GetCharacterList(string accountId)
    {
        var projection = Builders<CharacterInfo>.Projection
            .Include(x => x.Name)
            .Include(x => x.JobId)
            .Include(x => x.Level);

        return GetCollection()
            .Find(characterInfo => characterInfo.AccountId == accountId)
            .Project(projection)
            .ToEnumerable()
            .Select(document => new CharacterSlotInfo
            {
                CharacterId = document["_id"].AsObjectId.ToString(),
                Name = document["name"].AsString,
                JobName = jobRepository.GetName(document["job_id"].AsString),
                Level = document["level"].AsInt32,
            })
            .ToList();
    }

    public CreateCharacterResult Create(string accountId, string characterName, Gender gender, string jobId)
    {
        if (characterName.Length < 3)
        {
            return CreateCharacterResult.CharacterNameTooShort;
        }

        foreach (var ch in characterName)
        {
            if (char.IsLetterOrDigit(ch) || ch == '_' || ch == ' ')
            {
                continue;
            }

            return CreateCharacterResult.CharacterNameInvalid;
        }

        var jobInfo = jobRepository.Get(jobId);
        if (jobInfo is null)
        {
            return CreateCharacterResult.InvalidJob;
        }

        if (Exists(characterName))
        {
            return CreateCharacterResult.CharacterNameInUse;
        }

        var characterInfo = new CharacterInfo
        {
            AccountId = accountId,
            Name = characterName,
            Gender = gender,
            JobId = jobId,
            Sprite = jobInfo.Sprite,
            Level = 1,
            AccessLevel = AccessLevel.None,
            Strength = jobInfo.Strength,
            Defense = jobInfo.Defense,
            Speed = jobInfo.Speed,
            Intelligence = jobInfo.Intelligence,
            BaseStrength = jobInfo.Strength,
            BaseDefense = jobInfo.Defense,
            BaseSpeed = jobInfo.Speed,
            BaseIntelligence = jobInfo.Intelligence,
            Map = Options.StartMapName,
            X = Options.StartX,
            Y = Options.StartY,
            Direction = Direction.Down
        };

        characterInfo.HP = characterInfo.MaxHP;
        characterInfo.MP = characterInfo.MaxMP;
        characterInfo.SP = characterInfo.MaxSP;

        GetCollection().InsertOne(characterInfo);

        return CreateCharacterResult.Ok;
    }

    public void Save(CharacterInfo characterInfo)
    {
        GetCollection().ReplaceOne(x => x.AccountId == characterInfo.AccountId && x.Id == characterInfo.Id, characterInfo);
    }

    public void Delete(string characterId, string accountId)
    {
        GetCollection().DeleteOne(x => x.Id == characterId && x.AccountId == accountId);
    }

    public void DeleteForAccount(string accountId)
    {
        GetCollection().DeleteMany(x => x.AccountId == accountId);
    }
}