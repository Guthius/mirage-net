using ImGuiNET;
using Mirage.Client.Modules;

namespace Mirage.Client.Game;

public static class GameEditor
{
    private static readonly string[] _morals = ["None", "Safe Zone"];
    
    public static void ShowMapEditor()
    {
        ShowMapProperties();
    }

    public static void ShowMapProperties()
    {
        ImGui.Begin("Map Properties");
        
        ImGui.BeginGroup();
        ImGui.InputText("Name", ref modTypes.Map.Name, 32);
        ImGui.InputInt("Up", ref modTypes.Map.Up);
        ImGui.InputInt("Down", ref modTypes.Map.Down);
        ImGui.InputInt("Left", ref modTypes.Map.Left);
        ImGui.InputInt("Right", ref modTypes.Map.Right);
        ImGui.Combo("Moral", ref modTypes.Map.Moral, _morals, _morals.Length);
        ImGui.SliderInt("Music", ref modTypes.Map.Music, 0, 1000);
        ImGui.InputInt("Boot Map", ref modTypes.Map.BootMap);
        ImGui.InputInt("Boot X", ref modTypes.Map.BootX);
        ImGui.InputInt("Boot Y", ref modTypes.Map.BootY);
        
        ImGui.EndGroup();
    }
}