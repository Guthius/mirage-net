namespace Mirage.Net.Protocol.FromServer;

public enum CreateCharacterResult
{
    Ok,

    CharacterNameInvalid,
    CharacterNameTooShort,
    CharacterNameInUse,

    InvalidJob
}