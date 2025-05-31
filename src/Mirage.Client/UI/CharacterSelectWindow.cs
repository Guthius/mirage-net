using Mirage.Client.Localization;
using Mirage.Engine.UI.Controls;
using Mirage.Shared.Data;
using SFML.Graphics;
using SFML.System;

namespace Mirage.Client.UI;

public sealed class CharacterSelectWindow : Window
{
    public event Action? CreateNewCharacter;
    public event Action<CharacterEventArgs>? SelectCharacter;
    public event Action<CharacterEventArgs>? DeleteCharacter;
    public event Action? Cancel;

    public CharacterSelectWindow(List<CharacterSlotInfo> characterSlotInfos, int maxCharacters) : base(TempStyle.Style)
    {
        Width = 300;
        Height = CreateSlots(characterSlotInfos) + 60;
        Text = SR.CharacterSelect;

        var newCharacterButton = new Button(TempStyle.Style)
        {
            Position = new Vector2f(15, Height - 40),
            Width = 140, Height = 25,
            Enabled = characterSlotInfos.Count < maxCharacters,
            Text = "+ New Character"
        };

        newCharacterButton.Click += () => CreateNewCharacter?.Invoke();

        var cancelButton = new Button(TempStyle.Style)
        {
            Position = new Vector2f(205, Height - 40),
            Width = 80, Height = 25,
            Text = SR.Cancel
        };

        cancelButton.Click += () => Cancel?.Invoke();

        Add(newCharacterButton);
        Add(cancelButton);
    }

    private int CreateSlots(List<CharacterSlotInfo> characterSlotInfos)
    {
        const int height = 50;
        const int spacing = 5;

        var y = 35;

        foreach (var slotInfo in characterSlotInfos)
        {
            var characterId = slotInfo.CharacterId;

            var selectButton = new Button(TempStyle.Style)
            {
                Position = new Vector2f(15, y),
                Width = 225,
                Height = height
            };

            selectButton.Add(new Label(TempStyle.Style)
            {
                Position = new Vector2f(10, 5),
                Height = 20,
                Text = slotInfo.Name,
                TextColor = Color.White
            });

            selectButton.Add(new Label(TempStyle.Style)
            {
                Position = new Vector2f(10, 25),
                Height = 20,
                Text = $"a level {slotInfo.Level} {slotInfo.JobName}"
            });

            selectButton.Click += () => SelectCharacter?.Invoke(new CharacterEventArgs(characterId));

            Add(selectButton);

            var deleteButton = new Button(TempStyle.Style)
            {
                Position = new Vector2f(245, y),
                Width = 40,
                Height = height,
                Text = "x"
            };

            deleteButton.Click += () => DeleteCharacter?.Invoke(new CharacterEventArgs(characterId));

            Add(deleteButton);

            y += height + spacing;
        }

        return y;
    }
}