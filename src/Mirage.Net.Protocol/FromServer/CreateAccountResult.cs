namespace Mirage.Net.Protocol.FromServer;

public enum CreateAccountResult
{
    Ok,
    
    AccountNameInvalid,
    AccountNameOrPasswordTooShort,
    AccountNameTaken
}