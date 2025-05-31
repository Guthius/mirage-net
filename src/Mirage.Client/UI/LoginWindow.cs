using Mirage.Client.Localization;
using Mirage.Engine.UI.Controls;
using SFML.System;

namespace Mirage.Client.UI;

public sealed class LoginWindow : Window
{
    public event Action<LoginEventArgs>? Login;
    public event Action? Cancel;

    public LoginWindow(bool rememberMe) : base(TempStyle.Style)
    {
        Width = 300;
        Height = 175;
        Text = SR.Login;

        var accountNameTextBox = new TextBox(TempStyle.Style) {Position = new Vector2f(125, 40), Width = 160, Height = 25};
        var passwordTextBox = new TextBox(TempStyle.Style) {Position = new Vector2f(125, 70), Width = 160, Height = 25, IsPassword = true};

        var loginButton = new Button(TempStyle.Style)
        {
            Position = new Vector2f(115, 135),
            Width = 80, Height = 25,
            Text = SR.Login
        };

        loginButton.Click += () => Login?.Invoke(
            new LoginEventArgs(
                accountNameTextBox.Text,
                passwordTextBox.Text));

        var cancelButton = new Button(TempStyle.Style)
        {
            Position = new Vector2f(205, 135),
            Width = 80, Height = 25,
            Text = SR.Cancel
        };

        cancelButton.Click += () => Cancel?.Invoke();

        Add(new Label(TempStyle.Style) {Position = new Vector2f(20, 40), Width = 95, Height = 25, Text = SR.AccountName, HorizontalAlignment = HorizontalAlignment.Right});
        Add(accountNameTextBox);
        Add(new Label(TempStyle.Style) {Position = new Vector2f(20, 70), Width = 95, Height = 25, Text = SR.Password, HorizontalAlignment = HorizontalAlignment.Right});
        Add(passwordTextBox);
        Add(new CheckBox {Position = new Vector2f(125, 98), Width = 160, Height = 25, Text = SR.RememberMe, Checked = rememberMe});
        Add(loginButton);
        Add(cancelButton);
    }
}