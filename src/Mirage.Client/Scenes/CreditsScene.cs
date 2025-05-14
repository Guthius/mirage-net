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

        ImGui.SetNextWindowPos(center, ImGuiCond.Always, new ImGuiVec2(0.5f, 0.5f));
        ImGui.Begin("Credits", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.AlwaysAutoResize);

        DrawSection("Programming", "Chris Kremer (Torquel/Valient/Consty)");
        DrawSection("Tile/Sprite Art", "Copyright (c) Square Soft");
        DrawSection("GUI Art", "Jess Triska (Loken)");

        if (ImGui.Button("Back"))
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
        ImGui.Text(content);
        ImGui.Separator();
    }
}