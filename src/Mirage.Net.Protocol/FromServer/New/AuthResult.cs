namespace Mirage.Net.Protocol.FromServer.New;

public enum AuthResult
{
    Ok,

    InvalidAccountNameOrPassword,
    InvalidProtocolVersion,

    AlreadyLoggedIn
}