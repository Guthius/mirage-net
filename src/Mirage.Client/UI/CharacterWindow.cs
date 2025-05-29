using ImGuiNET;

namespace Mirage.Client.UI;

public static class CharacterWindow
{
    private static bool _open;

    public static void Open()
    {
        _open = true;
    }

    public static void Show()
    {
        if (!_open)
        {
            return;
        }

        if (!ImGui.Begin("Character", ref _open, ImGuiWindowFlags.AlwaysAutoResize))
        {
            ImGui.End();
        }
        
        ImGui.End();
    }
}