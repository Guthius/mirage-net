using ImGuiNET;
using Microsoft.Xna.Framework;
using Mirage.Client.Net;
using Mirage.Net.Protocol.FromClient;
using ImGuiVec2 = System.Numerics.Vector2;

namespace Mirage.Client.Scenes;

public sealed class CharacterSelectScene(ISceneManager sceneManager, Game gameState) : Scene
{
    private bool _disabled;

    protected override void OnShow()
    {
        _disabled = false;

        gameState.ClearStatus();
    }

    public override void DrawUI(GameTime gameTime)
    {
        var center = ImGui.GetMainViewport().GetCenter();

        ImGui.BeginDisabled(_disabled);
        ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new ImGuiVec2(0.5f, 0.5f));
        ImGui.Begin("Character Select", ImGuiWindowFlags.AlwaysAutoResize);
        ImGui.BeginChild("##Characters", new ImGuiVec2(276, 150), ImGuiChildFlags.FrameStyle);

        foreach (var slotInfo in gameState.Characters)
        {
            if (ImGui.Button($"{slotInfo.Name} a level {slotInfo.Level} {slotInfo.JobId}", new ImGuiVec2(200, 40)))
            {
                _disabled = true;

                Network.Send(new SelectCharacterRequest(slotInfo.CharacterId));
            }

            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Click to select this character and start playing.");
            }

            ImGui.SameLine();
            if (ImGui.Button($"Delete##{slotInfo.CharacterId}", new ImGuiVec2(60, 40)))
            {
                ImGui.OpenPopup($"Confirm Delete##{slotInfo.CharacterId}");
            }

            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Click to delete this character.");
            }

            ImGui.Spacing();
        }

        foreach (var slotInfo in gameState.Characters)
        {
            ImGui.SetNextWindowSize(new ImGuiVec2(380, 120), ImGuiCond.Always);
            ImGui.SetNextWindowPos(new ImGuiVec2(center.X - 190, center.Y - 60), ImGuiCond.Appearing);

            if (!ImGui.BeginPopupModal($"Confirm Delete##{slotInfo.CharacterId}"))
            {
                continue;
            }

            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new ImGuiVec2(10, 10));
            ImGui.Spacing();
            ImGui.TextWrapped($"Are you sure you want to delete {slotInfo.Name}?");
            ImGui.SetCursorPosY(70);
            ImGui.Separator();

            var buttonsWidth = 70 * 2 + ImGui.GetStyle().ItemSpacing.X;

            ImGui.SetCursorPosX((ImGui.GetWindowSize().X - buttonsWidth) * 0.5f);

            if (ImGui.Button("Yes", new ImGuiVec2(70, 26)))
            {
                ImGui.CloseCurrentPopup();
                gameState.SetStatus("Deleting character...");
                Network.Send(new DeleteCharacterRequest(slotInfo.CharacterId));
            }

            ImGui.SameLine();
            ImGui.SetItemDefaultFocus();

            if (ImGui.Button("No", new ImGuiVec2(70, 26)))
            {
                ImGui.CloseCurrentPopup();
            }

            ImGui.PopStyleVar();
            ImGui.EndPopup();
        }

        ImGui.EndChild();
        ImGui.Spacing();
        ImGui.Spacing();
        ImGui.BeginDisabled(gameState.MaxCharacters - gameState.Characters.Count == 0);
        if (ImGui.Button("Create", new ImGuiVec2(276, 30)))
        {
            sceneManager.SwitchTo<CreateCharacterScene>();
        }

        ImGui.EndDisabled();
        ImGui.Spacing();
        ImGui.Spacing();

        if (ImGui.Button("Back", new ImGuiVec2(70, 26)))
        {
            sceneManager.SwitchTo<LoginScene>();
        }

        ImGui.End();
        ImGui.EndDisabled();
    }
}