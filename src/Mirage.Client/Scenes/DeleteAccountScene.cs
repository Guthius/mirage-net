using ImGuiNET;
using Microsoft.Xna.Framework;
using Mirage.Client.Net;
using Mirage.Net.Protocol.FromClient;
using ImGuiVec2 = System.Numerics.Vector2;

namespace Mirage.Client.Scenes;

public sealed class DeleteAccountScene(ISceneManager sceneManager, IGameState gameState) : Scene
{
    private string _accountName = string.Empty;
    private string _password = string.Empty;

    protected override void OnShow()
    {
        gameState.ClearStatus();
    }

    public override void DrawUI(GameTime gameTime)
    {
        var center = ImGui.GetMainViewport().GetCenter();

        ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new ImGuiVec2(0.5f, 0.5f));
        ImGui.Begin("Delete Account", ImGuiWindowFlags.AlwaysAutoResize);
        ImGui.TextWrapped("Enter a account name and password of the account you wish to delete.");
        ImGui.Separator();
        ImGui.Spacing();
        ImGui.InputText("Name", ref _accountName, 16);
        ImGui.InputText("Password", ref _password, 32, ImGuiInputTextFlags.Password);
        ImGui.Separator();

        if (ImGui.Button("Connect"))
        {
            Task.Run(DeleteAccount);
        }

        ImGui.SameLine();

        if (ImGui.Button("Cancel"))
        {
            sceneManager.SwitchTo<MainMenuScene>();
        }

        ImGui.End();
    }

    private async Task DeleteAccount()
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
}