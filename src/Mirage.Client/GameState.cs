using Mirage.Client.Entities;
using Mirage.Client.Game;
using Mirage.Game.Data;

namespace Mirage.Client;

public abstract class GameState : Microsoft.Xna.Framework.Game
{
    public List<JobInfo> Jobs { get; set; } = [];
    public int MaxCharacters { get; set; }
    public List<CharacterSlotInfo> Characters { get; set; } = [];
    public InventorySlotInfo[] Inventory { get; set; } = [];
    public List<ChatInfo> ChatHistory { get; set; } = [];
    public bool ChatHistoryUpdated { get; set; }
    public Map Map { get; protected set; }
    public bool GettingMap { get; set; }
    public int LocalPlayerId { get; set; }
    public Player? LocalPlayer { get; set; }
}