using CommunityToolkit.Mvvm.DependencyInjection;
using Mirage.Client.Assets;
using Mirage.Client.Scenes;
using Mirage.Net.Protocol.FromServer;
using Mirage.Net.Protocol.FromServer.New;
using Mirage.Shared.Data;

namespace Mirage.Client.Net;

public static class NetworkHandlers
{
    private static readonly GameClient GameState = Ioc.Default.GetRequiredService<GameClient>();
    private static readonly ISceneManager SceneManager = Ioc.Default.GetRequiredService<ISceneManager>();

    public static void HandleCreateAccount(CreateAccountResponse response)
    {
        GameState.ClearStatus();

        switch (response.Result)
        {
            case CreateAccountResult.Ok:
                GameState.ShowAlert("Your account has been created!");
                SceneManager.SwitchTo<CharacterSelectScene>();
                break;

            case CreateAccountResult.AccountNameInvalid:
                GameState.ShowAlert("Invalid name, only letters, numbers, spaces, and _ allowed in names.");
                break;

            case CreateAccountResult.AccountNameOrPasswordTooShort:
                GameState.ShowAlert("Invalid account name, only letters, numbers, spaces, and _ allowed in names.");
                break;

            case CreateAccountResult.AccountNameTaken:
                GameState.ShowAlert("This account name is already taken. Please choose a different name.");
                break;

            default:
                GameState.ShowAlert("Unknown error.");
                break;
        }
    }

    public static void HandleDeleteAccount(DeleteAccountResponse response)
    {
        switch (response.Result)
        {
            case DeleteAccountResult.Ok:
                GameState.ShowAlert("Your account has been deleted!");
                break;

            case DeleteAccountResult.InvalidAccountNameOrPassword:
                GameState.ShowAlert("Invalid account name or password.");
                break;

            case DeleteAccountResult.AccountNameOrPasswordTooShort:
                GameState.ShowAlert("Account name and password must each contain at least 3 characters");
                break;

            default:
                GameState.ShowAlert("Unknown error.");
                break;
        }

        SceneManager.SwitchTo<MainMenuScene>();
    }

    public static void HandleAuth(AuthResponse response)
    {
        GameState.ClearStatus();

        switch (response.Result)
        {
            case AuthResult.Ok:
                break;

            case AuthResult.InvalidAccountNameOrPassword:
                GameState.ShowAlert("Incorrect account name or password.");
                break;

            case AuthResult.InvalidProtocolVersion:
                GameState.ShowAlert("Your client is out of date. Please update your client and try again.");
                break;

            case AuthResult.AlreadyLoggedIn:
                GameState.ShowAlert("Account is already logged in.");
                break;

            default:
                GameState.ShowAlert("Unknown error.");
                break;
        }
    }

    public static void HandleUpdateJobList(UpdateJobListCommand command)
    {
        GameState.Jobs = command.Jobs;
    }

    public static void HandleUpdateCharacterList(UpdateCharacterListCommand command)
    {
        GameState.MaxCharacters = command.MaxCharacters;
        GameState.Characters = command.Characters;

        SceneManager.SwitchTo<CharacterSelectScene>();
    }

    public static void HandleCreateCharacter(CreateCharacterResponse response)
    {
        GameState.ClearStatus();

        switch (response.Result)
        {
            case CreateCharacterResult.Ok:
                SceneManager.SwitchTo<CharacterSelectScene>();
                break;

            case CreateCharacterResult.CharacterNameInvalid:
                GameState.ShowAlert("Invalid name, only letters, numbers, spaces, and _ allowed in names.");
                break;

            case CreateCharacterResult.CharacterNameTooShort:
                GameState.ShowAlert("Character name must be at least three characters in length.");
                break;

            case CreateCharacterResult.CharacterNameInUse:
                GameState.ShowAlert("Sorry, but that name is in use!");
                break;

            case CreateCharacterResult.InvalidJob:
                GameState.ShowAlert("Invalid character job.");
                break;

            default:
                GameState.ShowAlert("Unknown error.");
                break;
        }
    }

    public static void HandleSelectCharacter(SelectCharacterResponse response)
    {
        GameState.ClearStatus();

        switch (response.Result)
        {
            case SelectCharacterResult.Ok:
                GameState.SetStatus("Entering game...");
                GameState.LocalPlayerId = response.PlayerId;
                SceneManager.SwitchTo<LoadingScene>();
                return;

            case SelectCharacterResult.InvalidCharacter:
                GameState.ShowAlert("Invalid character.");
                return;

            default:
                GameState.ShowAlert("Unknown error.");
                return;
        }
    }

    public static void HandleLoadMap(LoadMapCommand command)
    {
        GameState.Map.Load(command.MapId);
    }

    public static void HandleCreateActor(CreateActorCommand command)
    {
        var player = GameState.Map.CreateActor(
            command.ActorId,
            command.Name,
            command.Sprite,
            command.IsPlayerKiller,
            command.AccessLevel,
            command.X,
            command.Y,
            command.Direction,
            command.MaxHealth,
            command.Health,
            command.MaxMana,
            command.Mana,
            command.MaxStamina,
            command.Stamina);

        if (player.IsLocalPlayer)
        {
            GameState.LocalPlayer = player;
        }
    }

    public static void HandleDestroyActor(DestroyActorCommand command)
    {
        GameState.Map.DestroyActor(command.ActorId);
    }

    public static void HandleUpdateActorVitals(UpdateActorVitalsCommand command)
    {
        var actor = GameState.Map.GetActor(command.ActorId);
        if (actor is null)
        {
            return;
        }

        actor.MaxHealth = command.MaxHealth;
        actor.Health = command.Health;
        actor.MaxMana = command.MaxMana;
        actor.Mana = command.Mana;
        actor.MaxStamina = command.MaxStamina;
        actor.Stamina = command.Stamina;
    }

    public static void HandleActorMove(ActorMoveCommand command)
    {
        var actor = GameState.Map.GetActor(command.ActorId);

        actor?.QueueMove(command.Direction, command.MovementType);
    }

    public static void HandleActorAttack(ActorAttackCommand command)
    {
        var actor = GameState.Map.GetActor(command.ActorId);

        actor?.QueueAttack();
    }

    public static void HandleChat(ChatCommand command)
    {
        GameState.ChatHistory.Add(new ChatInfo(command.Message, command.Color));
        GameState.ChatHistoryUpdated = true;
    }
    
    public static void HandleDownloadAssetChunk(DownloadAssetChunkCommand command)
    {
        AssetDownloader.WriteChunk(command.Handle, command.Data);
    }

    public static void HandleDownloadAssetResponse(DownloadAssetResponse response)
    {
        AssetDownloader.End(response.Handle);
    }
    
    //---
    

    public static void HandleAlertMessage(AlertMessage alertMessage)
    {
        GameState.ShowAlert(alertMessage.Message);
        GameState.ClearStatus();
    }

    public static void HandleInGame(InGame inGame)
    {
        SceneManager.SwitchTo<GameScene>();
    }

    public static void HandleInventory(PlayerInventory playerInventory)
    {
        GameState.Inventory = playerInventory.Slots;
    }

    public static void HandlePlayerInventoryUpdate(PlayerInventoryUpdate playerInventoryUpdate)
    {
        var slot = playerInventoryUpdate.InventorySlot;

        GameState.Inventory[slot - 1].ItemId = playerInventoryUpdate.ItemId;
        GameState.Inventory[slot - 1].Quantity = playerInventoryUpdate.Quantity;
        GameState.Inventory[slot - 1].Durability = playerInventoryUpdate.Durability;
    }

    public static void HandlePlayerEquipment(PlayerEquipment playerEquipment)
    {
    }

    public static void HandlePlayerDir(PlayerDir playerDir)
    {
    }

    public static void HandleNpcMove(NpcMove npcMove)
    {
    }

    public static void HandleNpcDir(NpcDir npcDir)
    {
    }

    public static void HandlePlayerPosition(PlayerPosition playerPosition)
    {
    }

    public static void HandleNpcAttack(NpcAttack npcAttack)
    {
    }

    public static void HandleCheckForMap(CheckForMap checkFormap)
    {
    }

    public static void HandleMapData(MapData mapData)
    {
    }

    public static void HandleMapItemData(MapItemData mapItemData)
    {
    }

    public static void HandleMapNpcData(MapNpcData mapNpcData)
    {
    }

    public static void HandleMapDone(MapDone mapDone)
    {
    }

    public static void HandleSpawnItem(SpawnItem spawnItem)
    {
    }

    public static void HandleOpenItemEditor()
    {
    }

    public static void HandleUpdateItem(UpdateItem updateItem)
    {
    }

    public static void HandleEditItem(EditItem editItem)
    {
    }

    public static void HandleSpawnNpc(SpawnNpc spawnNpc)
    {
    }

    public static void HandleNpcDead(NpcDead npcDead)
    {
    }

    public static void HandleOpenNpcEditor()
    {
    }

    public static void HandleUpdateNpc(UpdateNpc updateNpc)
    {
    }

    public static void HandleEditNpc(EditNpc editNpc)
    {
    }

    public static void HandleMapKey(MapKey mapKey)
    {
    }

    public static void HandleOpenMapEditor()
    {
    }

    public static void HandleOpenShopEditor()
    {
    }

    public static void HandleUpdateShop(UpdateShop updateShop)
    {
    }

    public static void HandleEditShop(EditShop editShop)
    {
    }

    public static void HandleOpenSpellEditor()
    {
    }

    public static void HandleUpdateSpell(UpdateSpell updateSpell)
    {
    }

    public static void HandleEditSpell(EditSpell editSpell)
    {
    }

    public static void HandleTrade(Trade trade)
    {
    }

    public static void HandlePlayerSpells(PlayerSpells playerSpells)
    {
        // My.Forms.frmMirage.picPlayerSpells.Visible = true;
        // My.Forms.frmMirage.lstSpells.Items.Clear();
        //
        // for (var slot = 1; slot <= Limits.MaxPlayerSpells; slot++)
        // {
        //     modTypes.Player[modGameLogic.MyIndex].Spell[slot] = playerSpells.SpellIds[slot];
        //
        //     My.Forms.frmMirage.lstSpells.Items.Add(modTypes.Player[modGameLogic.MyIndex].Spell[slot] != 0
        //         ? $"{slot}: {modTypes.Spell[modTypes.Player[modGameLogic.MyIndex].Spell[slot]].Name}"
        //         : "<free spells slot>");
        // }
        //
        // My.Forms.frmMirage.lstSpells.SelectedIndex = 0;
    }
}