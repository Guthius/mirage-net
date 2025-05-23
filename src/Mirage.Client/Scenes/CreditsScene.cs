using ImGuiNET;
using Microsoft.Xna.Framework;
using ImGuiVec2 = System.Numerics.Vector2;
using ImGuiVec4 = System.Numerics.Vector4;

namespace Mirage.Client.Scenes;

public sealed class CreditsScene(ISceneManager sceneManager) : Scene
{
    public override void DrawUI(GameTime gameTime)
    {
        var center = ImGui.GetMainViewport().GetCenter();

        ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new ImGuiVec2(0.5f, 0.5f));
        ImGui.Begin("Credits", ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoCollapse);
        ImGui.Spacing();

        DrawSection("Programming", "Chris Kremer (Torquel/Valient/Consty)");
        ImGui.Separator();

        DrawSection("Tile/Sprite Art", "Copyright (c) Square Soft");
        ImGui.Separator();

        DrawSection("GUI Art", "Jess Triska (Loken)");
        ImGui.Spacing();

        if (ImGui.Button("Back", new ImGuiVec2(70, 26)))
        {
            sceneManager.SwitchTo<MainMenuScene>();
        }

        ImGui.End();
    }

    private static void DrawSection(string title, string content)
    {
        ImGui.PushStyleColor(ImGuiCol.Text, new ImGuiVec4(.7f, .7f, 1, 1));
        ImGui.Text(title);
        ImGui.PopStyleColor();
        ImGui.Spacing();
        ImGui.Text(content);
        ImGui.Spacing();
        ImGui.Spacing();
    }
}