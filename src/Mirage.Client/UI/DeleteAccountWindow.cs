using Mirage.Client.Localization;
using Mirage.Engine.UI.Controls;
using SFML.System;

namespace Mirage.Client.UI;

public sealed class DeleteAccountWindow : Window
{
    public event Action<DeleteAccountEventArgs>? DeleteAccount;
    public event Action? Cancel;

    public DeleteAccountWindow() : base(TempStyle.Style)
    {
        Width = 300;
        Height = 200;
        Text = SR.DeleteAccount;

        var accountNameTextBox = new TextBox(TempStyle.Style) {Position = new Vector2f(125, 85), Width = 160, Height = 25};
        var passwordTextBox = new TextBox(TempStyle.Style) {Position = new Vector2f(125, 115), Width = 160, Height = 25, IsPassword = true};

        var loginButton = new Button(TempStyle.Style)
        {
            Position = new Vector2f(115, 160),
            Width = 80, Height = 25,
            Text = SR.Delete
        };

        loginButton.Click += () => DeleteAccount?.Invoke(
            new DeleteAccountEventArgs(
                accountNameTextBox.Text,
                passwordTextBox.Text));

        var cancelButton = new Button(TempStyle.Style)
        {
            Position = new Vector2f(205, 160),
            Width = 80, Height = 25,
            Text = SR.Cancel
        };

        cancelButton.Click += () => Cancel?.Invoke();

        Add(new Label(TempStyle.Style)
        {
            Position = new Vector2f(10, 32),
            Width = 280, Height = 46,
            Text = SR.DeleteAccountInstruction,
            HorizontalAlignment = HorizontalAlignment.Center
        });

        Add(new Label(TempStyle.Style) {Position = new Vector2f(20, 85), Width = 95, Height = 25, Text = SR.AccountName, HorizontalAlignment = HorizontalAlignment.Right});
        Add(accountNameTextBox);
        Add(new Label(TempStyle.Style) {Position = new Vector2f(20, 115), Width = 95, Height = 25, Text = SR.Password, HorizontalAlignment = HorizontalAlignment.Right});
        Add(passwordTextBox);
        Add(loginButton);
        Add(cancelButton);
    }
}