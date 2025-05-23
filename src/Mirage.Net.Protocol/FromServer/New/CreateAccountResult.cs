namespace Mirage.Net.Protocol.FromServer.New;

public enum CreateAccountResult
{
    Ok,
    
    AccountNameInvalid,
    AccountNameOrPasswordTooShort,
    AccountNameTaken
}