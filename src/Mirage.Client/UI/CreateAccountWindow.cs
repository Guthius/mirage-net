using Mirage.Client.Localization;
using Mirage.Engine.UI.Controls;
using SFML.System;

namespace Mirage.Client.UI;

public sealed class CreateAccountWindow : Window
{
    public event Action<CreateAccountEventArgs>? CreateAccount;
    public event Action? Cancel;

    public CreateAccountWindow() : base(TempStyle.Style)
    {
        Width = 300;
        Height = 225;
        Text = SR.CreateAccount;

        var accountNameTextBox = new TextBox(TempStyle.Style) {Position = new Vector2f(125, 110), Width = 160, Height = 25};
        var passwordTextBox = new TextBox(TempStyle.Style) {Position = new Vector2f(125, 140), Width = 160, Height = 25, IsPassword = true};

        var loginButton = new Button(TempStyle.Style)
        {
            Position = new Vector2f(115, 185),
            Width = 80, Height = 25,
            Text = SR.Create
        };

        loginButton.Click += () => CreateAccount?.Invoke(
            new CreateAccountEventArgs(
                accountNameTextBox.Text,
                passwordTextBox.Text));

        var cancelButton = new Button(TempStyle.Style)
        {
            Position = new Vector2f(205, 185),
            Width = 80, Height = 25,
            Text = SR.Cancel
        };

        cancelButton.Click += () => Cancel?.Invoke();

        Add(new Label(TempStyle.Style)
        {
            Position = new Vector2f(10, 32),
            Width = 280, Height = 69,
            Text = SR.CreateAccountInstruction,
            HorizontalAlignment = HorizontalAlignment.Center
        });

        Add(new Label(TempStyle.Style) {Position = new Vector2f(20, 110), Width = 95, Height = 25, Text = SR.AccountName, HorizontalAlignment = HorizontalAlignment.Right});
        Add(accountNameTextBox);
        Add(new Label(TempStyle.Style) {Position = new Vector2f(20, 140), Width = 95, Height = 25, Text = SR.Password, HorizontalAlignment = HorizontalAlignment.Right});
        Add(passwordTextBox);
        Add(loginButton);
        Add(cancelButton);
    }
}