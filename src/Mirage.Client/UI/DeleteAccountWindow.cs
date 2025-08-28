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
        Size = new Vector2i(300, 200);
        Text = SR.DeleteAccount;

        var accountNameTextBox = new TextBox(TempStyle.Style) {Position = new Vector2f(125, 85), Size = new Vector2i(160, 25)};
        var passwordTextBox = new TextBox(TempStyle.Style) {Position = new Vector2f(125, 115), Size = new Vector2i(160, 25), IsPassword = true};

        var loginButton = new Button(TempStyle.Style)
        {
            Position = new Vector2f(115, 160),
            Size = new Vector2i(80, 25),
            Text = SR.Delete
        };

        loginButton.Click += () => DeleteAccount?.Invoke(
            new DeleteAccountEventArgs(
                accountNameTextBox.Text,
                passwordTextBox.Text));

        var cancelButton = new Button(TempStyle.Style)
        {
            Position = new Vector2f(205, 160),
            Size = new Vector2i(80, 25),
            Text = SR.Cancel
        };

        cancelButton.Click += () => Cancel?.Invoke();

        Add(new Label(TempStyle.Style)
        {
            Position = new Vector2f(10, 32),
            Size = new Vector2i(280, 46),
            Text = SR.DeleteAccountInstruction,
            HorizontalAlignment = HorizontalAlignment.Center
        });

        Add(new Label(TempStyle.Style) {Position = new Vector2f(20, 85), Size = new Vector2i(95, 25), Text = SR.AccountName, HorizontalAlignment = HorizontalAlignment.Right});
        Add(accountNameTextBox);
        Add(new Label(TempStyle.Style) {Position = new Vector2f(20, 115), Size = new Vector2i(95, 25), Text = SR.Password, HorizontalAlignment = HorizontalAlignment.Right});
        Add(passwordTextBox);
        Add(loginButton);
        Add(cancelButton);
    }
}