using ImGuiNET;
using Microsoft.Xna.Framework;
using ImGuiVec2 = System.Numerics.Vector2;

namespace Mirage.Client.Scenes;

public sealed class MainMenuScene(ISceneManager sceneManager, GameClient gameState) : Scene
{
    private static readonly ImGuiVec2 ButtonSize = new(160, 35);
    
    protected override void OnShow()
    {
        gameState.ClearStatus();
    }

    public override void DrawUI(GameTime gameTime)
    {
        var center = ImGui.GetMainViewport().GetCenter();

        ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new ImGuiVec2(0.5f, 0.15f));
        ImGui.Begin("Menu", ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoCollapse);
        ImGui.BeginGroup();

        if (ImGui.Button("New Account", ButtonSize))
        {
            sceneManager.SwitchTo<CreateAccountScene>();
        }

        if (ImGui.Button("Delete Account", ButtonSize))
        {
            sceneManager.SwitchTo<DeleteAccountScene>();
        }

        if (ImGui.Button("Login", ButtonSize))
        {
            sceneManager.SwitchTo<LoginScene>();
        }

        if (ImGui.Button("Credits", ButtonSize))
        {
            sceneManager.SwitchTo<CreditsScene>();
        }

        if (ImGui.Button("Quit", ButtonSize))
        {
            gameState.Exit();
        }

        ImGui.EndGroup();
        ImGui.End();
    }
}