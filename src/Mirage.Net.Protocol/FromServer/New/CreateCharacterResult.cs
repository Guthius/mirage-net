namespace Mirage.Net.Protocol.FromServer.New;

public enum CreateCharacterResult
{
    Ok,

    CharacterNameInvalid,
    CharacterNameTooShort,
    CharacterNameInUse,

    InvalidJob
}