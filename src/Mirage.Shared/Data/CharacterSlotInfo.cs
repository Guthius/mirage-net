namespace Mirage.Shared.Data;

public sealed record CharacterSlotInfo
{
    public string CharacterId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string JobName { get; set; } = string.Empty;
    public int Level { get; set; } = 1;
}