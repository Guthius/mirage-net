using ImGuiNET;
using Microsoft.Xna.Framework;
using Mirage.Client.Net;
using Mirage.Game.Data;
using Mirage.Net.Protocol.FromClient;
using Mirage.Net.Protocol.FromClient.New;
using ImGuiVec2 = System.Numerics.Vector2;
using ImGuiVec4 = System.Numerics.Vector4;

namespace Mirage.Client.Scenes;

public sealed class CreateCharacterScene(ISceneManager sceneManager, IGameState gameState) : Scene
{
    private bool _disabled;
    private string _characterName = string.Empty;
    private int _selectedJob;
    private int _gender;

    protected override void OnShow()
    {
        _disabled = false;
    }

    public override void DrawUI(GameTime gameTime)
    {
        if (gameState.Jobs is not {Count: > 0})
        {
            sceneManager.SwitchTo<CharacterSelectScene>();
        }

        var center = ImGui.GetMainViewport().GetCenter();

        var jobNames = gameState.Jobs.Select(c => c.Name).ToArray();
        var jobInfo = gameState.Jobs[_selectedJob];

        ImGui.BeginDisabled(_disabled);
        ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new ImGuiVec2(0.5f, 0.5f));
        ImGui.Begin("Create Character", ImGuiWindowFlags.AlwaysAutoResize);

        ImGui.InputText("Name", ref _characterName, 16);
        ImGui.Spacing();
        ImGui.Spacing();
        
        ImGui.Combo("Class", ref _selectedJob, jobNames, jobNames.Length);
        ImGui.Spacing();
        ImGui.Spacing();
        
        ImGui.RadioButton("Male", ref _gender, 0);
        ImGui.SameLine();
        ImGui.RadioButton("Female", ref _gender, 1);
        ImGui.Spacing();

        ImGui.Separator();
        ImGui.Spacing();

        ImGui.BeginTable("stats", 4);
        ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, new ImGuiVec2(5, 10));
        ImGui.TableSetupColumn("col1", ImGuiTableColumnFlags.WidthFixed, 60.0f);
        ImGui.TableSetupColumn("col2", ImGuiTableColumnFlags.WidthFixed, 60.0f);
        ImGui.TableSetupColumn("col3", ImGuiTableColumnFlags.WidthFixed, 60.0f);
        ImGui.TableSetupColumn("col4", ImGuiTableColumnFlags.WidthFixed, 60.0f);

        ImGui.TableNextRow();
        ImGui.TableNextColumn();
        DrawStat("HP", jobInfo.MaxHP);

        ImGui.TableNextColumn();
        DrawStat("MP", jobInfo.MaxMP);

        ImGui.TableNextColumn();
        DrawStat("SP", jobInfo.MaxSP);

        ImGui.TableNextRow();
        ImGui.TableNextColumn();
        DrawStat("STR", jobInfo.Strength);

        ImGui.TableNextColumn();
        DrawStat("DEF", jobInfo.Defense);

        ImGui.TableNextColumn();
        DrawStat("SPD", jobInfo.Speed);

        ImGui.TableNextColumn();
        DrawStat("INT", jobInfo.Intelligence);

        ImGui.EndTable();
        ImGui.Spacing();
        ImGui.Spacing();

        if (ImGui.Button("Create", new ImGuiVec2(70, 26)))
        {
            gameState.SetStatus("Requesting character creation...");

            Network.Send(new CreateCharacterRequest(_characterName, (Gender) _gender, gameState.Jobs[_selectedJob].Id));

            _disabled = true;
        }

        ImGui.SameLine();

        if (ImGui.Button("Cancel", new ImGuiVec2(70, 26)))
        {
            sceneManager.SwitchTo<CharacterSelectScene>();
        }

        ImGui.End();
        ImGui.EndDisabled();
    }

    private static void DrawStat(string label, int value)
    {
        var x = ImGui.GetCursorPosX();
        
        ImGui.Text(label);
        ImGui.SameLine();
        ImGui.SetCursorPosX(x + 30);
        ImGui.PushStyleColor(ImGuiCol.Text, new ImGuiVec4(.7f, .7f, 1, 1));
        ImGui.Text(value.ToString());
        ImGui.PopStyleColor();
    }
}