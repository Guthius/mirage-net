using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;
using Mirage.Game.Constants;
using Mirage.Game.Data;
using Mirage.Net;
using Mirage.Net.Protocol.FromClient;
using Mirage.Net.Protocol.FromClient.New;
using Mirage.Net.Protocol.FromServer;
using Mirage.Server.Extensions;
using Mirage.Server.Game;
using Mirage.Server.Repositories;
using Serilog;

namespace Mirage.Server.Net;

public static class Network
{
    public const int ProtocolVersion = 1;

    private static readonly PacketParser Parser = new();
    private static readonly CancellationTokenSource CancellationTokenSource = new();
    private static readonly Connection?[] Connections = new Connection?[Limits.MaxPlayers + 1];
    private static readonly ConcurrentQueue<int> ConnectionIds = new();
    private static bool _listening;

    private sealed record Connection(int Id, TcpClient Client, string RemoteAddr, Channel<byte[]> SendQueue, CancellationTokenSource Cts);

    static Network()
    {
        // Authentication & Account Management
        Parser.Register<AuthRequest>(NetworkHandlers.HandleAuth);
        
        // Character Management
        Parser.Register<CreateCharacterRequest>(NetworkHandlers.HandleCreateCharacter);
        Parser.Register<DeleteCharacterRequest>(NetworkHandlers.HandleDeleteCharacter);
        Parser.Register<SelectCharacterRequest>(NetworkHandlers.HandleSelectCharacter);
        
        // Player Actions
        Parser.Register<MoveRequest>(NetworkHandlers.HandleMove);
        
        
        //---
        Parser.Register<CreateAccountRequest>(NetworkHandlers.HandleCreateAccount);
        Parser.Register<DeleteAccountRequest>(NetworkHandlers.HandleDeleteAccount);
        Parser.Register<SayRequest>(NetworkHandlers.HandleSay);
        Parser.Register<EmoteRequest>(NetworkHandlers.HandleEmote);
        Parser.Register<BroadcastRequest>(NetworkHandlers.HandleBroadcast);
        Parser.Register<GlobalMessageRequest>(NetworkHandlers.HandleGlobalMessage);
        Parser.Register<AdminMessageRequest>(NetworkHandlers.HandleAdminMessage);
        Parser.Register<PlayerMessageRequest>(NetworkHandlers.HandlePlayerMessage);

        Parser.Register<SetDirectionRequest>(NetworkHandlers.HandleSetDirection);
        Parser.Register<UseItemRequest>(NetworkHandlers.HandleUseItem);
        Parser.Register<AttackRequest>(NetworkHandlers.HandleAttack);
        Parser.Register<UseStatPointRequest>(NetworkHandlers.HandleUseStatPoint);
        Parser.Register<PlayerInfoRequest>(NetworkHandlers.HandlePlayerInfoRequest);
        Parser.Register<WarpMeToRequest>(NetworkHandlers.HandleWarpMeTo, AccessLevel.Mapper);
        Parser.Register<WarpToMeRequest>(NetworkHandlers.HandleWarpToMe, AccessLevel.Mapper);
        Parser.Register<WarpToRequest>(NetworkHandlers.HandleWarpTo, AccessLevel.Mapper);
        Parser.Register<SetSpriteRequest>(NetworkHandlers.HandleSetSprite, AccessLevel.Mapper);
        Parser.Register<GetStatsRequest>(NetworkHandlers.HandleGetStats);
        Parser.Register<NewMapRequest>(NetworkHandlers.HandleNewMap);
        Parser.Register<UpdateMapRequest>(NetworkHandlers.HandleUpdateMap, AccessLevel.Mapper);
        Parser.Register<NeedMapRequest>(NetworkHandlers.HandleNeedMap);
        Parser.Register<PickupItemRequest>(NetworkHandlers.HandlePickupItem);
        Parser.Register<DropItemRequest>(NetworkHandlers.HandleDropItem);
        Parser.Register<MapRespawnRequest>(NetworkHandlers.HandleMapRespawn, AccessLevel.Mapper);
        Parser.Register<MapReportRequest>(NetworkHandlers.HandleMapReport, AccessLevel.Mapper);
        Parser.Register<KickPlayerRequest>(NetworkHandlers.HandleKickPlayer, AccessLevel.Moderator);
        Parser.Register<BanListRequest>(NetworkHandlers.HandleBanList, AccessLevel.Mapper);
        Parser.Register<BanDestroyRequest>(NetworkHandlers.HandleBanDestroy, AccessLevel.Administrator);
        Parser.Register<BanPlayerRequest>(NetworkHandlers.HandleBanPlayer, AccessLevel.Mapper);
        Parser.Register<OpenMapEditorRequest>(NetworkHandlers.HandleOpenMapEditor, AccessLevel.Mapper);
        Parser.Register<OpenItemEditorRequest>(NetworkHandlers.HandleOpenItemEditor, AccessLevel.Developer);
        Parser.Register<EditItemRequest>(NetworkHandlers.HandleEditItem, AccessLevel.Developer);
        Parser.Register<UpdateItemRequest>(NetworkHandlers.HandleUpdateItem, AccessLevel.Developer);
        Parser.Register<OpenNpcEditorRequest>(NetworkHandlers.HandleOpenNpcEditor, AccessLevel.Developer);
        Parser.Register<EditNpcRequest>(NetworkHandlers.HandleEditNpc, AccessLevel.Developer);
        Parser.Register<UpdateNpcRequest>(NetworkHandlers.HandleUpdateNpc, AccessLevel.Developer);
        Parser.Register<SetAccessLevelRequest>(NetworkHandlers.HandleSetAccessLevel, AccessLevel.Administrator);
        Parser.Register<SetMotdRequest>(NetworkHandlers.HandleSetMotd, AccessLevel.Mapper);
        Parser.Register<OpenShopEditorRequest>(NetworkHandlers.HandleOpenShopEditor, AccessLevel.Developer);
        Parser.Register<EditShopRequest>(NetworkHandlers.HandleEditShop, AccessLevel.Developer);
        Parser.Register<UpdateShopRequest>(NetworkHandlers.HandleUpdateShop, AccessLevel.Developer);
        Parser.Register<OpenSpellEditorRequest>(NetworkHandlers.OpenSpellEditor, AccessLevel.Developer);
        Parser.Register<EditSpellRequest>(NetworkHandlers.HandleEditSpell, AccessLevel.Developer);
        Parser.Register<UpdateSpellRequest>(NetworkHandlers.HandleUpdateSpell, AccessLevel.Developer);
        Parser.Register<WhosOnlineRequest>(NetworkHandlers.HandleWhosOnline);
        Parser.Register<ShopRequest>(NetworkHandlers.HandleShop);
        Parser.Register<ShopTradeRequest>(NetworkHandlers.HandleShopTrade);
        Parser.Register<FixItemRequest>(NetworkHandlers.HandleFixItem);
        Parser.Register<SearchRequest>(NetworkHandlers.HandleSearch);
        Parser.Register<PartyRequest>(NetworkHandlers.HandleParty);
        Parser.Register<JoinPartyRequest>(NetworkHandlers.HandleJoinParty);
        Parser.Register<LeavePartyRequest>(NetworkHandlers.HandleLeaveParty);
        Parser.Register<SpellsRequest>(NetworkHandlers.HandleSpells);
        Parser.Register<CastRequest>(NetworkHandlers.HandleCast);
        Parser.Register<LocationRequest>(NetworkHandlers.HandleLocation, AccessLevel.Mapper);

        for (var playerId = 1; playerId <= Limits.MaxPlayers; playerId++)
        {
            ConnectionIds.Enqueue(playerId);
        }
    }

    public static void Start()
    {
        var listening = Interlocked.Exchange(ref _listening, true);
        if (listening)
        {
            throw new InvalidOperationException("Cannot call Listen() more than once");
        }

        var tcpListener = new TcpListener(IPAddress.Any, Options.GamePort);

        tcpListener.Start();

        Log.Information("Server started on port {Port}", Options.GamePort);

        _ = Task.Run(() => AcceptClientsAsync(tcpListener, CancellationTokenSource.Token), CancellationTokenSource.Token);
    }

    private static async Task AcceptClientsAsync(TcpListener tcpListener, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var tcpClient = await tcpListener.AcceptTcpClientAsync(cancellationToken);

                if (!ConnectionIds.TryDequeue(out var id))
                {
                    tcpClient.Close();

                    return;
                }

                var remoteAddr = "null";
                if (tcpClient.Client.RemoteEndPoint is IPEndPoint ipEndPoint)
                {
                    remoteAddr = ipEndPoint.Address.ToString();
                }

                var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

                var sendQueue = Channel.CreateUnbounded<byte[]>();
                var connection = new Connection(id, tcpClient, remoteAddr, sendQueue, cts);

                Connections[id] = connection;

                GameState.CreateSession(id);

                _ = RunAsync(connection);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error accepting client: {ex.Message}");
            }
        }
    }

    private static async Task RunAsync(Connection connection)
    {
        try
        {
            await Task.WhenAny(RunReceive(connection), RunSend(connection));

            await connection.Cts.CancelAsync();
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error handling client");
        }
        finally
        {
            connection.SendQueue.Writer.TryComplete();

            Log.Information("Connection with {IpAddress} has been terminated", connection.RemoteAddr);

            Connections[connection.Id] = null;
            ConnectionIds.Enqueue(connection.Id);

            GameState.DestroySession(connection.Id);

            connection.Client.Close();
            connection.Cts.Dispose();
        }
    }

    private static async Task RunReceive(Connection connection)
    {
        var stream = connection.Client.GetStream();
        var buffer = new byte[1024];

        try
        {
            SocketConnected(connection.Id);

            while (!connection.Cts.IsCancellationRequested && connection.Client.Connected)
            {
                var bytesRead = await stream.ReadAsync(buffer, connection.Cts.Token);
                if (bytesRead == 0)
                {
                    break;
                }

                IncomingData(connection.Id, buffer.AsSpan(0, bytesRead));
            }
        }
        catch (IOException)
        {
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error receiving data from client");
        }
    }

    private static async Task RunSend(Connection connection)
    {
        await foreach (var bytes in connection.SendQueue.Reader.ReadAllAsync(connection.Cts.Token))
        {
            try
            {
                var stream = connection.Client.GetStream();

                await stream.WriteAsync(bytes, connection.Cts.Token);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error sending data to client");
            }
        }
    }

    public static void SendData(int connectionId, byte[] bytes)
    {
        Connections[connectionId]?.SendQueue.Writer.TryWrite(bytes);
    }

    public static void Disconnect(int connectionId)
    {
        if (connectionId < 1)
        {
            return;
        }

        var connection = Connections[connectionId];

        connection?.SendQueue.Writer.Complete();
    }

    public static string GetIP(int connectionId)
    {
        return Connections[connectionId]?.RemoteAddr ?? "null";
    }

    public static void SendToAll<TPacket>(TPacket packet) where TPacket : IPacket<TPacket>
    {
        var bytes = PacketSerializer.GetBytes(packet);

        foreach (var player in GameState.OnlinePlayers())
        {
            Connections[player.Id]?.SendQueue.Writer.TryWrite(bytes);
        }
    }

    public static void SendToAllBut<TPacket>(int excludePlayerId, TPacket packet) where TPacket : IPacket<TPacket>
    {
        var bytes = PacketSerializer.GetBytes(packet);

        foreach (var player in GameState.OnlinePlayers())
        {
            if (excludePlayerId != player.Id)
            {
                Connections[player.Id]?.SendQueue.Writer.TryWrite(bytes);
            }
        }
    }

    public static void SendGlobalMessage(string message, int color)
    {
        SendToAll(new PlayerMessage(message, color));
    }

    public static void SendAlert(int connectionId, string message)
    {
        var bytes = PacketSerializer.GetBytes(new AlertMessage(message));

        Connections[connectionId]?.SendQueue.Writer.TryWrite(bytes);

        Disconnect(connectionId);
    }

    public static void ReportHackAttempt(int connectionId, string reason)
    {
        if (connectionId <= 0)
        {
            return;
        }

        SendAlert(connectionId, $"You have lost your connection with {Options.GameName}.");

        var player = GameState.Sessions[connectionId];
        if (player is {Account: not null, Player: not null})
        {
            SendToAll(new PlayerMessage($"{player.Account.Name}/{player.Player.Character.Name} has been booted for ({reason})", Color.White));
        }
    }

    public static void SocketConnected(int connectionId)
    {
        if (connectionId == 0)
        {
            return;
        }

        if (BanRepository.IsBanned(GetIP(connectionId)))
        {
            SendAlert(connectionId, $"You have been banned from {Options.GameName}, and can no longer play.");
            return;
        }

        Log.Information("Received connection from {IpAddress}", GetIP(connectionId));
    }

    public static void IncomingData(int connectionId, ReadOnlySpan<byte> bytes)
    {
        var session = GameState.Sessions[connectionId];
        if (session is null)
        {
            return;
        }

        bytes.CopyTo(session.Buffer.AsSpan(session.BufferOffset));

        session.BufferOffset += bytes.Length;

        var bytesHandled = Parser.Parse(connectionId, session.Buffer.AsMemory(0, session.BufferOffset));
        if (bytesHandled <= 0)
        {
            return;
        }

        var bytesLeft = session.BufferOffset - bytesHandled;
        if (bytesLeft > 0)
        {
            session.Buffer.AsSpan(bytesHandled, bytesLeft).CopyTo(session.Buffer);
        }

        session.BufferOffset = bytesLeft;
    }
}