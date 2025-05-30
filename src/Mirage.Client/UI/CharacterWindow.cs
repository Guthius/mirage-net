using System.Numerics;
using ImGuiNET;
using Mirage.Client.Inventory;

namespace Mirage.Client.UI;

public static class CharacterWindow
{
    private static bool _open;

    public static void Open()
    {
        _open = true;
    }

    public static void Show(Game game)
    {
        if (!_open)
        {
            return;
        }

        if (!ImGui.Begin("Character", ref _open, ImGuiWindowFlags.AlwaysAutoResize))
        {
            ImGui.End();
        }
        
        ImGui.Spacing();
        ImGui.SeparatorText("Equipment");
        ShowSlot("Weapon", game.Inventory.Weapon);
        ShowSlot("Armor", game.Inventory.Armor);
        ShowSlot("Helmet", game.Inventory.Helmet);
        ShowSlot("Shield", game.Inventory.Shield);
        
        ImGui.End();
    }

    private static void ShowSlot(string name, EquipmentSlot? slot)
    {
        ImGui.BeginChild($"equip_{name}", new Vector2(200, 50), ImGuiChildFlags.Border);
        ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.75f);
        ImGui.Text(name);
        ImGui.PopStyleVar();
        ImGui.Spacing();
        ImGui.SetCursorPosX(15);
        if (slot is not null)
        {
            ImGui.Text(slot.ItemName);
        }
        else
        {
            ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.5f);
            ImGui.Text("None");
            ImGui.PopStyleVar();
        }
        
        ImGui.EndChild();
    }
}