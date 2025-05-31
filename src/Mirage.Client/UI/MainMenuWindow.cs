using Mirage.Client.Localization;
using Mirage.Engine.UI.Controls;
using SFML.System;

namespace Mirage.Client.UI;

public sealed class MainMenuWindow : Window
{
    public event Action? GoToLogin;
    public event Action? GoToNewAccount;
    public event Action? GoToDeleteAccount;
    public event Action? Quit;

    public MainMenuWindow() : base(TempStyle.Style)
    {
        Text = SR.GameName;

        Width = 190;

        CreateButtons(
            (SR.Login, () => GoToLogin?.Invoke()),
            (SR.NewAccount, () => GoToNewAccount?.Invoke()),
            (SR.DeleteAccount, () => GoToDeleteAccount?.Invoke()),
            (SR.Quit, () => Quit?.Invoke()));
    }

    private void CreateButtons(params (string, Action)[] buttons)
    {
        const int buttonHeight = 35;
        const int padding = 15;
        const int spacing = 5;

        var y = 35;

        foreach (var (text, action) in buttons)
        {
            var button = new Button(TempStyle.Style)
            {
                Position = new Vector2f(padding, y),
                Width = Width - padding * 2,
                Height = buttonHeight,
                Text = text
            };

            button.Click += () => action();

            Add(button);

            y += buttonHeight + spacing;
        }

        Height = y + padding - spacing;
    }
}