using System.Buffers;
using Mirage.Net.Protocol.FromClient;
using Mirage.Net.Protocol.FromClient.New;
using Mirage.Net.Protocol.FromServer;
using Mirage.Net.Protocol.FromServer.New;
using Mirage.Server.Assets;
using Mirage.Server.Chat;
using Mirage.Server.Players;
using Mirage.Server.Repositories;
using Mirage.Shared.Constants;
using Mirage.Shared.Data;
using Serilog;
using static Mirage.Server.Net.Network;

namespace Mirage.Server.Net;

public static class NetworkHandlers
{
    public static void HandleAuth(NetworkSession session, AuthRequest request)
    {
        if (session.Account is not null)
        {
            return;
        }

        if (request.ProtocolVersion != ProtocolVersion)
        {
            session.Send(new AuthResponse(AuthResult.InvalidProtocolVersion));
            return;
        }

        var account = AccountRepository.Authenticate(request.AccountName, request.Password);
        if (account is null)
        {
            session.Send(new AuthResponse(AuthResult.InvalidAccountNameOrPassword));
            return;
        }

        if (IsAccountLoggedIn(request.AccountName))
        {
            session.Send(new AuthResponse(AuthResult.AlreadyLoggedIn));
            return;
        }

        session.Account = account;
        session.Send(new AuthResponse(AuthResult.Ok));
        session.Send(new UpdateJobListCommand(JobRepository.GetAll()));
        session.Send(new UpdateCharacterListCommand(Limits.MaxCharacters, CharacterRepository.GetCharacterList(account.Id)));

        Log.Information("Account {AccountName} has logged in from {RemoteIp}", account.Name, GetIP(session.Id));
    }

    public static void HandleCreateAccount(NetworkSession session, CreateAccountRequest request)
    {
        if (session.Account is not null)
        {
            return;
        }

        if (request.AccountName.Length < 3 || request.Password.Length < 3)
        {
            session.Send(new CreateAccountResponse(CreateAccountResult.AccountNameOrPasswordTooShort));
            return;
        }

        foreach (var ch in request.AccountName)
        {
            if (char.IsLetterOrDigit(ch) || ch == '_' || ch == ' ')
            {
                continue;
            }

            session.Send(new CreateAccountResponse(CreateAccountResult.AccountNameInvalid));
            return;
        }

        if (AccountRepository.Exists(request.AccountName))
        {
            session.Send(new CreateAccountResponse(CreateAccountResult.AccountNameTaken));
            return;
        }

        var account = AccountRepository.Create(request.AccountName, request.Password);

        Log.Information("Account '{AccountName}' has been created.", request.AccountName);

        session.Account = account;
        session.Send(new CreateAccountResponse(CreateAccountResult.Ok));
        session.Send(new UpdateJobListCommand(JobRepository.GetAll()));
        session.Send(new UpdateCharacterListCommand(Limits.MaxCharacters, CharacterRepository.GetCharacterList(account.Id)));
    }

    public static void HandleDeleteAccount(NetworkSession session, DeleteAccountRequest request)
    {
        if (session.Account is not null)
        {
            return;
        }

        if (request.AccountName.Length < 3 || request.Password.Length < 3)
        {
            session.Send(new DeleteAccountResponse(DeleteAccountResult.AccountNameOrPasswordTooShort));
            return;
        }

        var account = AccountRepository.Authenticate(request.AccountName, request.Password);
        if (account is null)
        {
            session.Send(new DeleteAccountResponse(DeleteAccountResult.InvalidAccountNameOrPassword));
            return;
        }

        AccountRepository.Delete(account.Id);

        session.Send(new DeleteAccountResponse(DeleteAccountResult.Ok));
    }

    public static void HandleCreateCharacter(NetworkSession session, AccountInfo account, CreateCharacterRequest request)
    {
        var result = CharacterRepository.Create(account.Id, request.CharacterName, request.Gender, request.JobId);

        session.Send(new CreateCharacterResponse(result));
        if (result != CreateCharacterResult.Ok)
        {
            return;
        }

        Log.Information("Character {CharacterName} created by account {AccountName}", request.CharacterName, account.Name);

        session.Send(new UpdateCharacterListCommand(Limits.MaxCharacters, CharacterRepository.GetCharacterList(account.Id)));
    }

    public static void HandleDeleteCharacter(NetworkSession session, AccountInfo account, DeleteCharacterRequest request)
    {
        CharacterRepository.Delete(request.CharacterId, account.Id);

        Log.Information("Character deleted on account {AccountName}", account.Name);

        session.Send(new UpdateCharacterListCommand(Limits.MaxCharacters, CharacterRepository.GetCharacterList(account.Id)));
    }

    public static void HandleSelectCharacter(NetworkSession session, AccountInfo account, SelectCharacterRequest request)
    {
        var character = CharacterRepository.Get(request.CharacterId, account.Id);
        if (character is null)
        {
            session.Send(new SelectCharacterResponse(SelectCharacterResult.InvalidCharacter, -1));
            return;
        }

        session.Send(new SelectCharacterResponse(SelectCharacterResult.Ok, session.Id));
        session.CreatePlayer(character);

        Log.Information("Player {CharacterName} started playing {GameName} [Account: {AccountName}]",
            character.Name, Options.GameName, account.Name);
    }

    public static void HandleMove(Player player, MoveRequest request)
    {
        if (player.CastedSpell)
        {
            if (Environment.TickCount > player.AttackTimer + 1000)
            {
                player.CastedSpell = false;
            }
            else
            {
                player.Send(new SetActorPositionCommand(
                    player.Id,
                    player.Character.Direction,
                    player.Character.X,
                    player.Character.Y));

                return;
            }
        }

        player.NewMap.Move(player, request.Direction, request.Movement);
    }

    public static void HandleAttack(Player player, AttackRequest request)
    {
        player.NewMap.Attack(player);
    }

    public static void HandleSetDirection(Player player, SetDirectionRequest request)
    {
        player.Character.Direction = request.Direction;
        player.NewMap.Send(new SetActorDirectionCommand(player.Id, player.Character.Direction),
            p => p.Id != player.Id);
    }

    public static void HandleSay(Player player, SayRequest request)
    {
        ChatProcessor.Handle(player, request.Message);
    }

    public static void HandleDownloadAsset(Player player, DownloadAssetRequest request)
    {
        const int chunkSize = 1024;

        var asset = AssetManager.Get(request.Hash);
        if (asset is null)
        {
            player.Send(new DownloadAssetResponse(request.Handle, DownloadAssetResult.NotFound));
            return;
        }

        _ = Task.Run(async () =>
        {
            var buffer = ArrayPool<byte>.Shared.Rent(chunkSize);
            try
            {
                int bytesRead;

                await using var stream = asset.OpenRead();
                while ((bytesRead = await stream.ReadAsync(buffer)) > 0)
                {
                    player.Send(new DownloadAssetChunkCommand(request.Handle, buffer.AsSpan(0, bytesRead).ToArray()));
                }

                player.Send(new DownloadAssetResponse(request.Handle, DownloadAssetResult.Ok));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error sending {Hash} to client [Hash: {Sha1Hash}]",
                    asset.Path, asset.Id);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        });
    }

    //----

    public static void HandleUseItem(Player player, UseItemRequest request)
    {
        player.UseItem(request.InventorySlot);
    }

    public static void HandleUseStatPoint(Player player, UseStatPointRequest request)
    {
        if (player.Character.StatPoints <= 0)
        {
            player.Tell("You have no skill points to train with!", ColorCode.BrightRed);
            return;
        }

        player.Character.StatPoints--;
        switch (request.PointType)
        {
            case StatType.Strength:
                player.Character.Strength++;
                player.Tell("You have gained more strength!", ColorCode.White);
                break;

            case StatType.Defense:
                player.Character.Defense++;
                player.Tell("You have gained more defense!", ColorCode.White);
                break;

            case StatType.Intelligence:
                player.Character.Intelligence++;
                player.Tell("You have gained more magic abilities!", ColorCode.White);
                break;

            case StatType.Speed:
                player.Character.Speed++;
                player.Tell("You have gained more speed!", ColorCode.White);
                break;
        }

        player.SendStats();
    }

    public static void HandlePickupItem(Player player, PickupItemRequest request)
    {
        player.PickupItem();
    }

    public static void HandleDropItem(Player player, DropItemRequest request)
    {
        if (request.InventorySlot is < 1 or > Limits.MaxInventory)
        {
            ReportHackAttempt(player.Id, "Invalid InvNum");
            return;
        }

        var slotInfo = player.Character.Inventory[request.InventorySlot];
        if (request.Quantity > slotInfo.Quantity)
        {
            ReportHackAttempt(player.Id, "Item amount modification");
            return;
        }

        player.DropItem(request.InventorySlot, request.Quantity);
    }

    public static void HandleShop(Player player, ShopRequest request)
    {
        var mapInfo = MapRepository.Get(player.Character.MapId);
        if (mapInfo is null)
        {
            return;
        }

        var shopInfo = ShopRepository.Get(mapInfo.ShopId);
        if (shopInfo is null)
        {
            player.Tell("There is no shop here.", ColorCode.BrightRed);
            return;
        }

        foreach (var tradeInfo in shopInfo.Trades)
        {
            var itemInfo = ItemRepository.Get(tradeInfo.GetItemId);
            if (itemInfo is null || itemInfo.Type != ItemType.Spell)
            {
                continue;
            }

            var spellInfo = SpellRepository.Get(itemInfo.Data1);
            if (spellInfo is null)
            {
                continue;
            }

            player.Tell(!string.IsNullOrEmpty(spellInfo.RequiredClassId)
                    ? $"{itemInfo.Name} can be used by all classes."
                    : $"{itemInfo.Name} can only be used by a {JobRepository.GetName(spellInfo.RequiredClassId)};",
                ColorCode.Yellow);
        }

        player.Send(new Trade(shopInfo.Id, shopInfo.FixesItems, shopInfo.Trades));
    }

    public static void HandleShopTrade(Player player, ShopTradeRequest request)
    {
        if (request.Slot is < 0 or > Limits.MaxShopTrades)
        {
            ReportHackAttempt(player.Id, "Invalid Trade Index");
            return;
        }

        var mapInfo = MapRepository.Get(player.Character.MapId);
        if (mapInfo is null)
        {
            return;
        }

        var shopInfo = ShopRepository.Get(mapInfo.ShopId);
        if (shopInfo is null)
        {
            return;
        }

        var tradeInfo = shopInfo.Trades[request.Slot];

        var getItemInfo = ItemRepository.Get(tradeInfo.GetItemId);
        if (getItemInfo is null)
        {
            return;
        }

        var inventorySlot = player.GetFreeInventorySlot(getItemInfo);
        if (inventorySlot == 0)
        {
            player.Tell("Trade unsuccessful, inventory full.", ColorCode.BrightRed);
            return;
        }

        if (player.GetItemQuantity(tradeInfo.GiveItemId) < tradeInfo.GiveItemQuantity)
        {
            player.Tell("Trade unsuccessful.", ColorCode.BrightRed);
            return;
        }

        player.TakeItem(tradeInfo.GiveItemId, tradeInfo.GiveItemQuantity);
        player.GiveItem(tradeInfo.GetItemId, tradeInfo.GetItemQuantity);

        player.Tell("The trade was successful!", ColorCode.Yellow);
    }

    public static void HandleFixItem(Player player, FixItemRequest request)
    {
        const int goldId = 1;

        var slotInfo = player.Character.Inventory[request.InventorySlot];

        var itemInfo = ItemRepository.Get(slotInfo.ItemId);
        if (itemInfo is null)
        {
            return;
        }

        if (itemInfo.Type is < ItemType.Weapon or > ItemType.Shield)
        {
            player.Tell("You can only fix weapons, armors, helmets, and shields.", ColorCode.BrightRed);
            return;
        }

        var pointsToRepair = itemInfo.Data1 - slotInfo.Durability;
        if (pointsToRepair <= 0)
        {
            player.Tell("This item is in perfect condition!", ColorCode.White);
            return;
        }

        var costPerPoint = Math.Min(1, itemInfo.Data2 / 5);
        var costTotal = Math.Min(1, pointsToRepair * costPerPoint);

        var availableGold = player.GetItemQuantity(goldId);
        if (availableGold < costPerPoint)
        {
            player.Tell("Insufficient gold to fix this item!", ColorCode.BrightRed);
            return;
        }

        if (availableGold >= costTotal)
        {
            player.TakeItem(goldId, costTotal);

            slotInfo.Durability = itemInfo.Data1;

            player.Tell($"Item has been totally restored for {costTotal} gold!", ColorCode.BrightBlue);
            return;
        }

        pointsToRepair = availableGold / costPerPoint;
        if (pointsToRepair <= 0)
        {
            return;
        }

        var cost = pointsToRepair * costPerPoint;

        player.TakeItem(goldId, cost);

        slotInfo.Durability += pointsToRepair;

        player.Tell($"Item has been partially fixed for {cost} gold!", ColorCode.BrightBlue);
    }

    public static void HandleSearch(Player player, SearchRequest request)
    {
        player.NewMap.LookAt(player, request.X, request.Y);
    }

    public static void HandleSpells(Player player, SpellsRequest request)
    {
        player.Send(new PlayerSpells(player.Character.SpellIds));
    }

    public static void HandleCast(Player player, CastRequest request)
    {
        player.Cast(request.SpellSlot);
    }
}