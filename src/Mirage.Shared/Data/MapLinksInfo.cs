namespace Mirage.Shared.Data;

public sealed record MapLinksInfo
{
    public string Up { get; set; } = string.Empty;
    public string Down { get; set; } = string.Empty;
    public string Left { get; set; } = string.Empty;
    public string Right { get; set; } = string.Empty;
}