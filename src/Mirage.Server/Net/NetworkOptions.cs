namespace Mirage.Server.Net;

public sealed record NetworkOptions
{
    public int Port { get; set; } = 4000;
    public int MaxConnections { get; set; } = 1000;
}