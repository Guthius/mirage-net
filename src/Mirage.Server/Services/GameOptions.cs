namespace Mirage.Server.Services;

public sealed record GameOptions
{
    public string StartMap { get; set; } = "Default.tmx";
    public int StartX { get; set; }
    public int StartY { get; set; }
    
    public int MaxCharactersPerAccount { get; set; } = 3;
}