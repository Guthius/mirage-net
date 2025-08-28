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
        Size = new Vector2i(300, 225);
        Text = SR.CreateAccount;

        var accountNameTextBox = new TextBox(TempStyle.Style) {Position = new Vector2f(125, 110), Size = new Vector2i(160, 25)};
        var passwordTextBox = new TextBox(TempStyle.Style) {Position = new Vector2f(125, 140), Size = new Vector2i(160, 25), IsPassword = true};

        var loginButton = new Button(TempStyle.Style)
        {
            Position = new Vector2f(115, 185),
            Size = new Vector2i(80, 25),
            Text = SR.Create
        };

        loginButton.Click += () => CreateAccount?.Invoke(
            new CreateAccountEventArgs(
                accountNameTextBox.Text,
                passwordTextBox.Text));

        var cancelButton = new Button(TempStyle.Style)
        {
            Position = new Vector2f(205, 185),
            Size = new Vector2i(80, 25),
            Text = SR.Cancel
        };

        cancelButton.Click += () => Cancel?.Invoke();

        Add(new Label(TempStyle.Style)
        {
            Position = new Vector2f(10, 32),
            Size = new Vector2i(280, 69),
            Text = SR.CreateAccountInstruction,
            HorizontalAlignment = HorizontalAlignment.Center
        });

        Add(new Label(TempStyle.Style) {Position = new Vector2f(20, 110), Size = new Vector2i(95, 25), Text = SR.AccountName, HorizontalAlignment = HorizontalAlignment.Right});
        Add(accountNameTextBox);
        Add(new Label(TempStyle.Style) {Position = new Vector2f(20, 140), Size = new Vector2i(95, 25), Text = SR.Password, HorizontalAlignment = HorizontalAlignment.Right});
        Add(passwordTextBox);
        Add(loginButton);
        Add(cancelButton);
    }
}