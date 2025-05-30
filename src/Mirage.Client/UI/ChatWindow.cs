using System.Numerics;
using ImGuiNET;

namespace Mirage.Client.UI;

public static class ChatWindow
{
    private static string _chatMessage = string.Empty;

    public static void Show(Game game)
    {
        const float sendButtonWidth = 60f;
        
        var inputTextHeight = ImGui.GetTextLineHeight() + ImGui.GetStyle().FramePadding.Y * 2;
        var inputRowHeight = inputTextHeight + ImGui.GetStyle().FramePadding.Y * 2;

        if (!ImGui.Begin("Chat"))
        {
            ImGui.End();
            return;
        }

        var contentArea = ImGui.GetContentRegionAvail();

        ImGui.BeginChild("##ChatMessages", contentArea with {Y = contentArea.Y - inputRowHeight}, ImGuiChildFlags.Border);

        foreach (var chat in game.ChatHistory)
        {
            ImGui.PushStyleColor(ImGuiCol.Text, ColorCodeTranslator.GetImGuiColor(chat.ColorCode));
            ImGui.TextWrapped(chat.Message);
            ImGui.PopStyleColor();
        }

        if (game.ChatHistoryUpdated)
        {
            ImGui.SetScrollHereY(1.0f);
            game.ChatHistoryUpdated = false;
        }

        ImGui.EndChild();
        
        ImGui.SetNextItemWidth(contentArea.X - ImGui.GetStyle().ItemSpacing.X - sendButtonWidth);
        if (ImGui.InputText("##Message", ref _chatMessage, 256, ImGuiInputTextFlags.EnterReturnsTrue))
        {
            ChatProcessor.Handle(_chatMessage);
            _chatMessage = string.Empty;
        }

        ImGui.SameLine();
        if (ImGui.Button("Send", new Vector2(sendButtonWidth, 19)))
        {
            ChatProcessor.Handle(_chatMessage);
            _chatMessage = string.Empty;
        }

        ImGui.End();
    }
}