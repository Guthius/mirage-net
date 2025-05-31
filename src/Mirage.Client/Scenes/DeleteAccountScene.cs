using Mirage.Client.Net;
using Mirage.Client.UI;
using Mirage.Engine.UI.Controls;
using Mirage.Net.Protocol.FromClient;
using SFML.Graphics;
using SFML.System;

namespace Mirage.Client.Scenes;

public sealed class DeleteAccountScene : Scene
{
    private readonly DeleteAccountWindow _deleteAccountWindow = new();

    private readonly Label _statusLabel = new(TempStyle.Style)
    {
        Position = new Vector2f(10, 572),
        Width = 200,
        Height = 25,
        TextColor = Color.White
    };

    public DeleteAccountScene(ISceneManager sceneManager)
    {
        UI.Add(new PictureBox {Image = "Content/Title.png"});
        UI.Add(_statusLabel);
        UI.Add(_deleteAccountWindow);

        _deleteAccountWindow.DeleteAccount += DeleteAccount;
        _deleteAccountWindow.Cancel += sceneManager.SwitchTo<MainMenuScene>;
        _deleteAccountWindow.MoveToCenter();
    }

    protected override void OnAlert(string alertMessage)
    {
        _statusLabel.Text = alertMessage;
    }

    private async void DeleteAccount(DeleteAccountEventArgs e)
    {
        try
        {
            Network.Disconnect();

            _deleteAccountWindow.Enabled = false;

            _statusLabel.Text = "Connecting to server...";
            _statusLabel.TextColor = Color.White;

            if (!await Network.ConnectAsync())
            {
                _statusLabel.Text = "Failed to connect to server.";
                _statusLabel.TextColor = Color.Red;

                return;
            }

            _statusLabel.Text = "Connected, sending account deletion request...";

            Network.Send(new DeleteAccountRequest(e.AccountName, e.Password));
        }
        catch (Exception ex)
        {
            _statusLabel.Text = ex.Message;
            _statusLabel.TextColor = Color.Red;
        }
        finally
        {
            _deleteAccountWindow.Enabled = true;
        }
    }
}