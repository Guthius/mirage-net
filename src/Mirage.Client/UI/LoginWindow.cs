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
        Size = new Vector2i(300, 175);
        Text = SR.Login;

        var accountNameTextBox = new TextBox(TempStyle.Style) {Position = new Vector2f(125, 40), Size = new Vector2i(160, 25)};
        var passwordTextBox = new TextBox(TempStyle.Style) {Position = new Vector2f(125, 70), Size = new Vector2i(160, 25), IsPassword = true};

        var loginButton = new Button(TempStyle.Style)
        {
            Position = new Vector2f(115, 135),
            Size = new Vector2i(80, 25),
            Text = SR.Login
        };

        loginButton.Click += () => Login?.Invoke(
            new LoginEventArgs(
                accountNameTextBox.Text,
                passwordTextBox.Text));

        var cancelButton = new Button(TempStyle.Style)
        {
            Position = new Vector2f(205, 135),
            Size = new Vector2i(80, 25),
            Text = SR.Cancel
        };

        cancelButton.Click += () => Cancel?.Invoke();

        Add(new Label(TempStyle.Style) {Position = new Vector2f(20, 40), Size = new Vector2i(95, 25), Text = SR.AccountName, HorizontalAlignment = HorizontalAlignment.Right});
        Add(accountNameTextBox);
        Add(new Label(TempStyle.Style) {Position = new Vector2f(20, 70), Size = new Vector2i(95, 25), Text = SR.Password, HorizontalAlignment = HorizontalAlignment.Right});
        Add(passwordTextBox);
        Add(new CheckBox(TempStyle.Style) {Position = new Vector2f(125, 98), Size = new Vector2i(160, 25), Text = SR.RememberMe, Checked = rememberMe});
        Add(loginButton);
        Add(cancelButton);
    }
}