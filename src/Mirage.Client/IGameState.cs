using Mirage.Client.Game;
using Mirage.Game.Data;

namespace Mirage.Client;

public sealed record Chat(string Message, int Color);

public interface IGameState
{
    List<JobInfo> Jobs { get; set; }
    int MaxCharacters { get; set; }
    List<CharacterSlotInfo> Characters { get; set; }
    InventorySlotInfo[] Inventory { get; set; }
    List<Chat> ChatHistory { get; set; }
    bool ChatHistoryUpdated { get; set; }
    
    Map? Map { get; set; }

    void Exit();
    void SetStatus(string status);
    void ClearStatus();
    void ShowAlert(string alertMessage);
}