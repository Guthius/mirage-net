using Mirage.Net.Protocol.FromClient;
using Mirage.Shared.Data;
using Terestrium.Client.Net;
using Terestrium.Client.Scenes;
using Terestrium.Client.UI.Controls;

namespace Terestrium.Client.Windows;

internal sealed class WinCharacterSelect : WinBase
{
    private readonly WinConfirm _winConfirm;

    public WinCharacterSelect(Window window, ISceneMainMenu menu, WinConfirm winConfirm) : base(window)
    {
        _winConfirm = winConfirm;

        Get<Button>("Slot1Select").Click += Select;
        Get<Button>("Slot1Delete").Click += Delete;
        Get<Button>("Slot2Select").Click += Select;
        Get<Button>("Slot2Delete").Click += Delete;
        Get<Button>("Slot3Select").Click += Select;
        Get<Button>("Slot3Delete").Click += Delete;

        Get<Button>("CreateButton").Click += (_, _) => menu.ShowCharacterCreation();
        Get<Button>("CancelButton").Click += (_, _) => menu.ShowLogin();
    }

    private static void Select(object? sender, EventArgs e)
    {
        if (sender is not Button {Tag: CharacterSlotInfo slot})
        {
            return;
        }

        var request = new SelectCharacterRequest(slot.CharacterId);

        Network.Send(request);
    }

    private void Delete(object? sender, EventArgs e)
    {
        if (sender is not Button {Tag: CharacterSlotInfo slot})
        {
            return;
        }

        var request = new DeleteCharacterRequest(slot.CharacterId);

        _winConfirm.Show("Are you sure you want to delete this character?", () => Network.Send(request));
    }

    private void SetSlotData(int slot, CharacterSlotInfo character)
    {
        var buttonSelect = Get<Button>("Slot" + slot + "Select");
        var buttonDelete = Get<Button>("Slot" + slot + "Delete");

        Get<Label>("Slot" + slot + "Name").Text = character.Name;

        buttonSelect.Tag = character;
        buttonSelect.Visible = true;
        buttonDelete.Tag = character;
        buttonDelete.Visible = true;
    }

    private void SetSlotEmpty(int slot)
    {
        var buttonSelect = Get<Button>("Slot" + slot + "Select");
        var buttonDelete = Get<Button>("Slot" + slot + "Delete");

        Get<Label>("Slot" + slot + "Name").Text = "Empty";

        buttonSelect.Tag = null;
        buttonSelect.Visible = false;
        buttonDelete.Tag = null;
        buttonDelete.Visible = false;
    }

    protected override void OnShown()
    {
        MoveToCenter();
    }

    public void Show(List<CharacterSlotInfo> characterSlotInfos, int maxCharacters)
    {
        for (var i = 0; i < 3; i++)
        {
            if (i < characterSlotInfos.Count)
            {
                SetSlotData(i + 1, characterSlotInfos[i]);
            }
            else
            {
                SetSlotEmpty(i + 1);
            }
        }

        Show();
    }
}