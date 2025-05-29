using ImGuiNET;
using Microsoft.Xna.Framework;
using Mirage.Client.Net;
using Mirage.Net.Protocol.FromClient;
using ImGuiVec2 = System.Numerics.Vector2;

namespace Mirage.Client.Scenes;

public sealed class LoginScene(ISceneManager sceneManager, Game gameState) : Scene
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
        ImGui.Begin("Login", ImGuiWindowFlags.AlwaysAutoResize);
        ImGui.Spacing();
        
        ImGui.Text("Enter your account name and password.");
        ImGui.Spacing();
        ImGui.Spacing();
        ImGui.Spacing();
        
        ImGui.SetItemDefaultFocus();
        ImGui.InputText("Account Name", ref _accountName, 16);
        ImGui.Spacing();
        ImGui.Spacing();
        
        ImGui.InputText("Password", ref _password, 32, ImGuiInputTextFlags.Password);
        ImGui.Spacing();
        ImGui.Spacing();

        if (ImGui.Button("Connect", new ImGuiVec2(70, 26)))
        {
            Task.Run(Login);
        }

        ImGui.SameLine();

        if (ImGui.Button("Cancel", new ImGuiVec2(70, 26)))
        {
            sceneManager.SwitchTo<MainMenuScene>();
        }

        ImGui.End();
        ImGui.EndDisabled();
    }

    private async Task Login()
    {
        _disabled = true;
        try
        {
            gameState.SetStatus("Connecting to server...");
            
            if (!await Network.ConnectAsync())
            {
                gameState.ClearStatus();
                gameState.ShowAlert("Failed to connect to server.");

                return;
            }

            gameState.SetStatus("Connected, sending login information...");

            Network.Send(new AuthRequest(1, _accountName, _password));
        }
        finally
        {
            _disabled = false;
        }
    }
}