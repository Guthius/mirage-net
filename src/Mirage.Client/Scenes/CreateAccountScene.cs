using ImGuiNET;
using Microsoft.Xna.Framework;
using Mirage.Client.Net;
using Mirage.Net.Protocol.FromClient.New;
using ImGuiVec2 = System.Numerics.Vector2;

namespace Mirage.Client.Scenes;

public sealed class CreateAccountScene(ISceneManager sceneManager, GameClient gameState) : Scene
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
        ImGui.Begin("Create Account", ImGuiWindowFlags.AlwaysAutoResize);
        ImGui.Spacing();

        ImGui.Text("Enter a account name and password.");
        ImGui.Text("You can name yourself whatever you want,");
        ImGui.Text("we have no restrictions on names.");
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

        if (ImGui.Button("Create", new ImGuiVec2(70, 26)))
        {
            Task.Run(CreateAccount);
        }

        ImGui.SameLine();

        if (ImGui.Button("Cancel", new ImGuiVec2(70, 26)))
        {
            sceneManager.SwitchTo<MainMenuScene>();
        }

        ImGui.End();
        ImGui.EndDisabled();
    }

    private async Task CreateAccount()
    {
        _disabled = true;
        try
        {
            if (!await Network.ConnectAsync())
            {
                gameState.ShowAlert("Failed to connect to server.");

                return;
            }

            sceneManager.SwitchTo<LoadingScene>();

            gameState.SetStatus("Connected, sending new account information...");

            Network.Send(new CreateAccountRequest(_accountName, _password));
        }
        finally
        {
            _disabled = false;
        }
    }
}