using Mirage.Client.Net;
using Mirage.Client.UI;
using Mirage.Engine.UI.Controls;
using Mirage.Net.Protocol.FromClient;
using SFML.Graphics;
using SFML.System;

namespace Mirage.Client.Scenes;

public sealed class LoginScene : Scene
{
    private readonly LoginWindow _loginWindow = new(true);

    private readonly Label _statusLabel = new(TempStyle.Style)
    {
        Position = new Vector2f(10, 572),
        Size = new Vector2i(200, 25),
        TextColor = Color.White
    };

    public LoginScene(ISceneManager sceneManager)
    {
        UI.Add(new PictureBox {Image = "Content/Title.png"});
        UI.Add(_statusLabel);
        UI.Add(_loginWindow);

        var loader = new WindowLoader("Default");
        var window = loader.Load("WinLogin");

        UI.Add(window);
        
        _loginWindow.Login += Login;
        _loginWindow.Cancel += sceneManager.SwitchTo<MainMenuScene>;
        _loginWindow.MoveToCenter();
    }

    protected override void OnAlert(string alertMessage)
    {
        _statusLabel.Text = alertMessage;
    }

    private async void Login(LoginEventArgs e)
    {
        try
        {
            Network.Disconnect();

            _loginWindow.Enabled = false;

            _statusLabel.Text = "Connecting to server...";
            _statusLabel.TextColor = Color.White;

            if (!await Network.ConnectAsync())
            {
                _statusLabel.Text = "Failed to connect to server.";
                _statusLabel.TextColor = Color.Red;

                return;
            }

            _statusLabel.Text = "Connected, sending login information...";

            Network.Send(new AuthRequest(1, e.AccountName, e.Password));
        }
        catch (Exception ex)
        {
            _statusLabel.Text = ex.Message;
            _statusLabel.TextColor = Color.Red;
        }
        finally
        {
            _loginWindow.Enabled = true;
        }
    }
}