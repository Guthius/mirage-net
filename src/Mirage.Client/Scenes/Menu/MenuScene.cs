using Mirage.Client.Net;
using Mirage.Client.Scenes.Menu.Windows;
using Mirage.Engine.UI.Controls;
using Mirage.Engine.UI.Styles;
using Mirage.Net.Protocol.FromClient;
using Mirage.Shared.Data;
using SFML.Graphics;
using SFML.System;

namespace Mirage.Client.Scenes.Menu;

public sealed class MenuScene : Scene, IMenuScene
{
    private readonly WinConfirm _winConfirm;
    private readonly WinCharacterSelect _winCharacterSelect;
    private readonly WinCreateAccount _winCreateAccount;
    private readonly WinDeleteAccount _winDeleteAccount;
    private readonly WinLogin _winLogin;
    private readonly WinMainMenu _winMainMenu;
    private readonly Label _statusLabel;
    private float _statusTimer;

    public MenuScene()
    {
        _winConfirm = new WinConfirm(UI.CreateWindow("WinConfirm"));
        _winCharacterSelect = new WinCharacterSelect(UI.CreateWindow("WinCharacterSelect"), this, _winConfirm);
        _winCreateAccount = new WinCreateAccount(UI.CreateWindow("WinCreateAccount"), this);
        _winDeleteAccount = new WinDeleteAccount(UI.CreateWindow("WinDeleteAccount"), this);
        _winLogin = new WinLogin(UI.CreateWindow("WinLogin"), this);
        _winMainMenu = new WinMainMenu(UI.CreateWindow("WinMainMenu"), this);
        _statusLabel = new Label(Style.Empty)
        {
            Position = new Vector2i(10, 570),
            Size = new Vector2i(200, 25),
            TextColor = Color.White,
            Font = UI.Skin.GetFont("Regular")
        };

        UI.Add(_statusLabel);
        
        ShowAlert("Test");
    }

    protected override void OnUpdate(float dt)
    {
        if (_statusTimer <= 0)
        {
            return;
        }

        _statusTimer -= dt;
        if (_statusTimer <= 0)
        {
            _statusLabel.Text = "";
        }
    }

    public void ShowAlert(string alertMessage)
    {
        _winConfirm.Show(alertMessage, () => { });
    }

    public void ShowStatus(string status, Color? color = null)
    {
        _statusLabel.Text = status;
        _statusLabel.TextColor = color ?? Color.White;
        _statusTimer = 2.5f;
    }

    public void ShowMainMenu()
    {
        UI.HideAllWindows();

        _winMainMenu.Show();
    }

    public void ShowLogin()
    {
        UI.HideAllWindows();

        _winLogin.Show();
    }

    public void ShowCreateAccount()
    {
        UI.HideAllWindows();

        _winCreateAccount.Show();
    }

    public void ShowDeleteAccount()
    {
        UI.HideAllWindows();

        _winDeleteAccount.Show();
    }

    public void ShowCharacterSelect()
    {
        UI.HideAllWindows();

        _winCharacterSelect.Show();
    }

    public void ShowCharacterSelect(List<CharacterSlotInfo> characterSlotInfos, int maxCharacters)
    {
        UI.HideAllWindows();

        _winCharacterSelect.Show(characterSlotInfos, maxCharacters);
    }

    public void ShowCharacterCreation()
    {
        // TODO: Implement...
    }

    public void Quit()
    {
        Environment.Exit(0);
    }

    public async void Login(string accountName, string password)
    {
        try
        {
            Network.Disconnect();

            UI.HideAllWindows();

            ShowStatus("Connecting to server...");

            if (!await Network.ConnectAsync())
            {
                ShowStatus("Failed to connect to server.", Color.Red);
                
                ShowLogin();

                return;
            }

            ShowStatus("Connected, sending login information...");

            Network.Send(new AuthRequest(1, accountName, password));
        }
        catch (Exception ex)
        {
            ShowStatus(ex.Message, Color.Red);

            ShowLogin();
        }
    }

    public async void CreateAccount(string accountName, string password)
    {
        try
        {
            Network.Disconnect();

            UI.HideAllWindows();

            ShowStatus("Connecting to server...");

            if (!await Network.ConnectAsync())
            {
                ShowStatus("Failed to connect to server.", Color.Red);

                ShowCreateAccount();

                return;
            }

            ShowStatus("Connected, sending new account information...");

            Network.Send(new CreateAccountRequest(accountName, password));
        }
        catch (Exception ex)
        {
            ShowStatus(ex.Message, Color.Red);

            ShowCreateAccount();
        }
    }

    public async void DeleteAccount(string accountName, string password)
    {
        try
        {
            Network.Disconnect();

            UI.HideAllWindows();

            ShowStatus("Connecting to server...");

            if (!await Network.ConnectAsync())
            {
                ShowStatus("Failed to connect to server.", Color.Red);

                ShowDeleteAccount();

                return;
            }

            ShowStatus("Connected, sending account deletion request...");

            Network.Send(new DeleteAccountRequest(accountName, password));
        }
        catch (Exception ex)
        {
            ShowStatus(ex.Message, Color.Red);

            ShowDeleteAccount();
        }
    }
}