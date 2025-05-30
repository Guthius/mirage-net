namespace Mirage.Net.Protocol.FromServer;

public enum AuthResult
{
    Ok,

    InvalidAccountNameOrPassword,
    InvalidProtocolVersion,

    AlreadyLoggedIn
}