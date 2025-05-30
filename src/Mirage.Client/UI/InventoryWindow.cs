using System.Numerics;
using ImGuiNET;
using Mirage.Client.Inventory;
using Mirage.Client.Net;
using Mirage.Net.Protocol.FromClient;
using Mirage.Shared.Data;

namespace Mirage.Client.UI;

public static class InventoryWindow
{
    private static bool _open;
    private static int _selectedSlotIndex;
    private static InventorySlot? _selectedSlot;
    private static int _dropQuantity;
    private static bool _wantOpenDropItemWindow;
    private static bool _dropWindowOpen;

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

        if (!ImGui.Begin($"Inventory", ref _open, ImGuiWindowFlags.AlwaysAutoResize))
        {
            ImGui.End();
            return;
        }

        if (_wantOpenDropItemWindow)
        {
            _dropQuantity = 1;
            _dropWindowOpen = true;

            ImGui.OpenPopup("Drop");

            _wantOpenDropItemWindow = false;
        }

        ShowDropItemWindow();

        ImGui.BeginListBox("##InventorySlots", new Vector2(200, 300));

        var index = 0;

        foreach (var (slotIndex, slot) in game.Inventory.Slots)
        {
            var itemName = slot.Type == ItemType.Currency ? $"{slot.ItemName} ({slot.Quantity})" : slot.ItemName;

            if (ImGui.Selectable(itemName, _selectedSlotIndex == index))
            {
                _selectedSlotIndex = index;
                _selectedSlot = slot;
            }

            if (ImGui.IsItemHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Right))
            {
                ImGui.OpenPopup($"item_context_menu_{index}");

                _selectedSlotIndex = index;
                _selectedSlot = slot;
            }

            if (ImGui.BeginPopup($"item_context_menu_{index}"))
            {
                var usable = slot.Type != ItemType.Currency;

                ImGui.BeginDisabled(!usable);
                if (ImGui.MenuItem("Use"))
                {
                    UseItem(slotIndex);
                }

                ImGui.EndDisabled();
                ImGui.Separator();

                if (ImGui.MenuItem("Drop"))
                {
                    _wantOpenDropItemWindow = true;
                }

                ImGui.EndPopup();
            }

            if (ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
            {
                UseItem(slotIndex);
            }

            index++;
        }

        ImGui.EndListBox();
        ImGui.End();
    }

    private static void ShowDropItemWindow()
    {
        if (_selectedSlot is null || !_dropWindowOpen)
        {
            return;
        }

        if (!ImGui.BeginPopupModal("Drop", ref _dropWindowOpen, ImGuiWindowFlags.AlwaysAutoResize))
        {
            return;
        }

        ImGui.SetItemDefaultFocus();
        ImGui.Text($"Drop {_selectedSlot.ItemName}");
        ImGui.Spacing();
        ImGui.SetNextItemWidth(100);
        ImGui.InputInt("##QuantityInput", ref _dropQuantity, 1, 10);

        _dropQuantity = Math.Min(_dropQuantity, _selectedSlot.Quantity);

        ImGui.SameLine();
        ImGui.Text($"of {_selectedSlot.Quantity}");
        ImGui.Spacing();
        ImGui.Separator();

        if (ImGui.Button("OK", new Vector2(70, 26)))
        {
            DropItem(_selectedSlotIndex, _dropQuantity);

            ImGui.CloseCurrentPopup();
        }

        ImGui.SameLine();
        ImGui.SetItemDefaultFocus();
        if (ImGui.Button("Cancel", new Vector2(70, 26)))
        {
            ImGui.CloseCurrentPopup();
        }

        ImGui.EndPopup();
    }

    private static void UseItem(int slotIndex)
    {
        Network.Send(new UseItemRequest(slotIndex));
    }

    private static void DropItem(int slotIndex, int quantity)
    {
        Network.Send(new DropItemRequest(slotIndex, quantity));
    }
}