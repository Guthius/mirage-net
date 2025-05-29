using ImGuiNET;
using Microsoft.Xna.Framework;
using Mirage.Client.Net;
using Mirage.Net.Protocol.FromClient;
using ImGuiVec2 = System.Numerics.Vector2;

namespace Mirage.Client.Scenes;

public sealed class DeleteAccountScene(ISceneManager sceneManager, Game gameState) : Scene
{
    private bool _disabled;
    private string _accountName = string.Empty;
    private string _password = string.Empty;

    protected override void OnShow()
    {
        Network.Disconnect();

        _accountName = string.Empty;
        _password = string.Empty;

        gameState.ClearStatus();
    }

    public override void DrawUI(GameTime gameTime)
    {
        var center = ImGui.GetMainViewport().GetCenter();

        ImGui.BeginDisabled(_disabled);
        ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new ImGuiVec2(0.5f, 0.5f));
        ImGui.Begin("Delete Account", ImGuiWindowFlags.AlwaysAutoResize);
        ImGui.Spacing();

        ImGui.Text("Enter a account name and password");
        ImGui.Text("of the account you wish to delete.");
        ImGui.Spacing();
        ImGui.Spacing();
        ImGui.Spacing();

        ImGui.SetItemDefaultFocus();
        ImGui.InputText("Account Name", ref _accountName, 16);
        ImGui.Spacing();
        ImGui.Spacing();

        ImGui.InputText("Password", ref _password, 32, ImGuiInputTextFlags.Password);
        ImGui.Separator();
        ImGui.Spacing();

        if (ImGui.Button("Delete", new ImGuiVec2(70, 26)))
        {
            Task.Run(DeleteAccount);
        }

        ImGui.SameLine();

        if (ImGui.Button("Cancel", new ImGuiVec2(70, 26)))
        {
            sceneManager.SwitchTo<MainMenuScene>();
        }

        ImGui.End();
        ImGui.EndDisabled();
    }

    private async Task DeleteAccount()
    {
        _disabled = true;
        try
        {
            if (!await Network.ConnectAsync())
            {
                gameState.ShowAlert("Failed to connect to server.");

                return;
            }

            gameState.SetStatus("Connected, sending account deletion request...");

            sceneManager.SwitchTo<LoadingScene>();

            Network.Send(new DeleteAccountRequest(_accountName, _password));
        }
        finally
        {
            _disabled = false;
        }
    }
}