namespace Mirage.Net.Protocol.FromServer;

public enum DeleteAccountResult
{
    Ok,

    InvalidAccountNameOrPassword,

    AccountNameOrPasswordTooShort
}