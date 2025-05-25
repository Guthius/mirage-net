namespace Mirage.Shared.Data;

public sealed record MapLayerInfo
{
    public bool DrawOverActors { get; set; }
    public int[] Tiles { get; set; } = [];
}