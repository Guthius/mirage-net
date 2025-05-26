using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;
using Mirage.Net;
using Mirage.Net.Protocol.FromClient;
using Mirage.Net.Protocol.FromClient.New;
using Mirage.Net.Protocol.FromServer.New;
using Mirage.Server.Extensions;
using Mirage.Server.Players;
using Mirage.Server.Repositories;
using Mirage.Shared.Constants;
using Serilog;

namespace Mirage.Server.Net;

public static class Network
{
    public const int ProtocolVersion = 1;

    private static readonly PacketParser Parser = new(HandleBadPacket);
    private static readonly CancellationTokenSource CancellationTokenSource = new();
    private static readonly Connection?[] Connections = new Connection?[Limits.MaxPlayers + 1];
    private static readonly ConcurrentQueue<int> ConnectionIds = new();
    private static bool _listening;

    private sealed record Connection(int Id, TcpClient Client, string RemoteAddr, Channel<byte[]> SendQueue, CancellationTokenSource Cts);

    static Network()
    {
        // Authentication & Account Management
        Parser.Register<AuthRequest>(NetworkHandlers.HandleAuth);
        Parser.Register<CreateAccountRequest>(NetworkHandlers.HandleCreateAccount);
        Parser.Register<DeleteAccountRequest>(NetworkHandlers.HandleDeleteAccount);

        // Character Management
        Parser.Register<CreateCharacterRequest>(NetworkHandlers.HandleCreateCharacter);
        Parser.Register<DeleteCharacterRequest>(NetworkHandlers.HandleDeleteCharacter);
        Parser.Register<SelectCharacterRequest>(NetworkHandlers.HandleSelectCharacter);

        // Player Actions
        Parser.Register<MoveRequest>(NetworkHandlers.HandleMove);
        Parser.Register<AttackRequest>(NetworkHandlers.HandleAttack);
        Parser.Register<SetDirectionRequest>(NetworkHandlers.HandleSetDirection);

        // Social
        Parser.Register<SayRequest>(NetworkHandlers.HandleSay);

        // Asset Management
        Parser.Register<DownloadAssetRequest>(NetworkHandlers.HandleDownloadAsset);
        
        //---
        
        Parser.Register<UseItemRequest>(NetworkHandlers.HandleUseItem);
        Parser.Register<UseStatPointRequest>(NetworkHandlers.HandleUseStatPoint);
        Parser.Register<PickupItemRequest>(NetworkHandlers.HandlePickupItem);
        Parser.Register<DropItemRequest>(NetworkHandlers.HandleDropItem);
        Parser.Register<ShopRequest>(NetworkHandlers.HandleShop);
        Parser.Register<ShopTradeRequest>(NetworkHandlers.HandleShopTrade);
        Parser.Register<FixItemRequest>(NetworkHandlers.HandleFixItem);
        Parser.Register<SearchRequest>(NetworkHandlers.HandleSearch);
        Parser.Register<SpellsRequest>(NetworkHandlers.HandleSpells);
        Parser.Register<CastRequest>(NetworkHandlers.HandleCast);

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

                CreateSession(id);

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

            DestroySession(connection.Id);

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
        var stream = connection.Client.GetStream();

        await foreach (var bytes in connection.SendQueue.Reader.ReadAllAsync(connection.Cts.Token))
        {
            try
            {
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

        foreach (var player in OnlinePlayers())
        {
            Connections[player.Id]?.SendQueue.Writer.TryWrite(bytes);
        }
    }

    public static void SendGlobalMessage(string message, int color)
    {
        SendToAll(new ChatCommand(message, color));
    }

    public static void SendAlert(int connectionId, string message)
    {
        var bytes = PacketSerializer.GetBytes(new DisconnectCommand(message));

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

        var player = Sessions[connectionId];
        if (player is {Account: not null, Player: not null})
        {
            SendToAll(new ChatCommand($"{player.Account.Name}/{player.Player.Character.Name} has been booted ({reason})", ColorCode.White));
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
        var session = Sessions[connectionId];
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

    private static void HandleBadPacket(int playerId, string packetId)
    {
        Log.Warning("Received unsupported packet ({PacketId}) from {IpAddr}", packetId, GetIP(playerId));
    }
    
    //--
    
    public static NetworkSession?[] Sessions { get; } = new NetworkSession[Limits.MaxPlayers + 1];

    public static NetworkSession? GetSession(int playerId)
    {
        if (playerId is <= 0 or > Limits.MaxPlayers)
        {
            return null;
        }

        return Sessions[playerId];
    }

    public static void CreateSession(int playerId)
    {
        Sessions[playerId] = new NetworkSession(playerId);
    }

    public static void DestroySession(int playerId)
    {
        Sessions[playerId]?.Destroy();
        Sessions[playerId] = null;
    }

    public static bool IsAccountLoggedIn(string accountName)
    {
        for (var playerId = 1; playerId <= Limits.MaxPlayers; playerId++)
        {
            var accountInfo = Sessions[playerId]?.Account;
            if (accountInfo is null)
            {
                continue;
            }

            if (accountInfo.Name.Equals(accountName, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    public static Player? FindPlayer(ReadOnlySpan<char> characterName)
    {
        characterName = characterName.Trim();

        for (var playerId = 1; playerId <= Limits.MaxPlayers; playerId++)
        {
            var player = Sessions[playerId]?.Player;
            if (player is null)
            {
                continue;
            }

            var character = player.Character;
            if (character.Name.Length < characterName.Length)
            {
                continue;
            }

            if (character.Name.AsSpan()[..characterName.Length].Equals(characterName, StringComparison.CurrentCultureIgnoreCase))
            {
                return player;
            }
        }

        return null;
    }

    public static IEnumerable<Player> OnlinePlayers()
    {
        return Sessions.Where(session => session?.Player is not null).Select(session => session!.Player!);
    }

    public static void SavePlayers()
    {
        Log.Information("Saving all online players...");

        for (var playerId = 1; playerId <= Limits.MaxPlayers; playerId++)
        {
            var session = Sessions[playerId];

            if (session?.Player != null)
            {
                CharacterRepository.Save(session.Player.Character);
            }
        }
    }
}