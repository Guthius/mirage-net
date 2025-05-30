using Mirage.Net.Protocol.FromServer;
using Mirage.Shared.Data;

namespace Mirage.Server.Repositories.Characters;

public interface ICharacterRepository
{
    CharacterInfo? Get(string characterId, string accountId);
    List<CharacterSlotInfo> GetCharacterList(string accountId);
    CreateCharacterResult Create(string accountId, string characterName, Gender gender, string jobId);
    void Save(CharacterInfo characterInfo);
    void Delete(string characterId, string accountId);
    void DeleteForAccount(string accountId);
}