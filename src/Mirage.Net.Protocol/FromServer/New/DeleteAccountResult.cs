namespace Mirage.Net.Protocol.FromServer.New;

public enum DeleteAccountResult
{
    Ok,

    InvalidAccountNameOrPassword,

    AccountNameOrPasswordTooShort
}