using Mirage.Client.Net;
using Mirage.Client.UI;
using Mirage.Engine.UI.Controls;
using Mirage.Net.Protocol.FromClient;
using SFML.Graphics;
using SFML.System;

namespace Mirage.Client.Scenes;

public sealed class CreateAccountScene : Scene
{
    private readonly CreateAccountWindow _createAccountWindow = new();

    private readonly Label _statusLabel = new(TempStyle.Style)
    {
        Position = new Vector2f(10, 572),
        Size = new Vector2i(200, 25),
        TextColor = Color.White
    };

    public CreateAccountScene(ISceneManager sceneManager)
    {
        UI.Add(new PictureBox {Image = "Content/Title.png"});
        UI.Add(_statusLabel);
        UI.Add(_createAccountWindow);

        _createAccountWindow.CreateAccount += CreateAccount;
        _createAccountWindow.Cancel += sceneManager.SwitchTo<MainMenuScene>;
        _createAccountWindow.MoveToCenter();
    }

    protected override void OnAlert(string alertMessage)
    {
        _statusLabel.Text = alertMessage;
    }

    private async void CreateAccount(CreateAccountEventArgs e)
    {
        try
        {
            Network.Disconnect();

            _createAccountWindow.Enabled = false;

            _statusLabel.Text = "Connecting to server...";
            _statusLabel.TextColor = Color.White;

            if (!await Network.ConnectAsync())
            {
                _statusLabel.Text = "Failed to connect to server.";
                _statusLabel.TextColor = Color.Red;

                return;
            }

            _statusLabel.Text = "Connected, sending new account information...";

            Network.Send(new CreateAccountRequest(e.AccountName, e.Password));
        }
        catch (Exception ex)
        {
            _statusLabel.Text = ex.Message;
            _statusLabel.TextColor = Color.Red;
        }
        finally
        {
            _createAccountWindow.Enabled = true;
        }
    }
}