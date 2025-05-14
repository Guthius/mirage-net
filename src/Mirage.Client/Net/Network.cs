using System.Buffers;
using System.Net.Sockets;
using System.Threading.Channels;
using Mirage.Game.Constants;
using Mirage.Net;
using Mirage.Net.Protocol.FromServer;
using Mirage.Net.Protocol.FromServer.New;

namespace Mirage.Client.Net;

public static class Network
{
    private static readonly PacketParser Parser = new();

    private static TcpClient? _tcpClient;
    private static Channel<byte[]>? _writeChannel;

    static Network()
    {
        Parser.Register<AuthResponse>(NetworkHandlers.HandleAuth);
        Parser.Register<JobList>(NetworkHandlers.HandleJobList);
        Parser.Register<CharacterList>(NetworkHandlers.HandleCharacterList);
        Parser.Register<CreateCharacterResponse>(NetworkHandlers.HandleCreateCharacter);
        Parser.Register<SelectCharacterResponse>(NetworkHandlers.HandleSelectCharacter);

        Parser.Register<LoadMapCommand>(NetworkHandlers.HandleLoadMap);
        Parser.Register<CreatePlayerCommand>(NetworkHandlers.HandleCreatePlayer);

        //---
        Parser.Register<AlertMessage>(NetworkHandlers.HandleAlertMessage);
        Parser.Register<NewCharClasses>(NetworkHandlers.HandleNewCharClasses);
        Parser.Register<InGame>(NetworkHandlers.HandleInGame);
        Parser.Register<PlayerInventory>(NetworkHandlers.HandleInventory);
        Parser.Register<PlayerInventoryUpdate>(NetworkHandlers.HandlePlayerInventoryUpdate);
        Parser.Register<PlayerEquipment>(NetworkHandlers.HandlePlayerEquipment);
        Parser.Register<PlayerHp>(NetworkHandlers.HandlePlayerHP);
        Parser.Register<PlayerMp>(NetworkHandlers.HandlePlayerMP);
        Parser.Register<PlayerSp>(NetworkHandlers.HandlePlayerSP);
        Parser.Register<PlayerStats>(NetworkHandlers.HandlePlayerStats);
        Parser.Register<PlayerData>(NetworkHandlers.HandlePlayerData);
        Parser.Register<PlayerMove>(NetworkHandlers.HandlePlayerMove);
        Parser.Register<PlayerDir>(NetworkHandlers.HandlePlayerDir);
        Parser.Register<NpcMove>(NetworkHandlers.HandleNpcMove);
        Parser.Register<NpcDir>(NetworkHandlers.HandleNpcDir);
        Parser.Register<PlayerPosition>(NetworkHandlers.HandlePlayerPosition);
        Parser.Register<Attack>(NetworkHandlers.HandleAttack);
        Parser.Register<NpcAttack>(NetworkHandlers.HandleNpcAttack);
        Parser.Register<CheckForMap>(NetworkHandlers.HandleCheckForMap);
        Parser.Register<MapData>(NetworkHandlers.HandleMapData);
        Parser.Register<MapItemData>(NetworkHandlers.HandleMapItemData);
        Parser.Register<MapNpcData>(NetworkHandlers.HandleMapNpcData);
        Parser.Register<MapDone>(NetworkHandlers.HandleMapDone);
        Parser.Register<PlayerMessage>(NetworkHandlers.HandlePlayerMessage);
        Parser.Register<SpawnItem>(NetworkHandlers.HandleSpawnItem);
        Parser.Register<OpenItemEditor>(NetworkHandlers.HandleOpenItemEditor);
        Parser.Register<UpdateItem>(NetworkHandlers.HandleUpdateItem);
        Parser.Register<EditItem>(NetworkHandlers.HandleEditItem);
        Parser.Register<SpawnNpc>(NetworkHandlers.HandleSpawnNpc);
        Parser.Register<NpcDead>(NetworkHandlers.HandleNpcDead);
        Parser.Register<OpenNpcEditor>(NetworkHandlers.HandleOpenNpcEditor);
        Parser.Register<UpdateNpc>(NetworkHandlers.HandleUpdateNpc);
        Parser.Register<EditNpc>(NetworkHandlers.HandleEditNpc);
        Parser.Register<MapKey>(NetworkHandlers.HandleMapKey);
        Parser.Register<OpenMapEditor>(NetworkHandlers.HandleOpenMapEditor);
        Parser.Register<OpenShopEditor>(NetworkHandlers.HandleOpenShopEditor);
        Parser.Register<UpdateShop>(NetworkHandlers.HandleUpdateShop);
        Parser.Register<EditShop>(NetworkHandlers.HandleEditShop);
        Parser.Register<OpenSpellEditor>(NetworkHandlers.HandleOpenSpellEditor);
        Parser.Register<UpdateSpell>(NetworkHandlers.HandleUpdateSpell);
        Parser.Register<EditSpell>(NetworkHandlers.HandleEditSpell);
        Parser.Register<Trade>(NetworkHandlers.HandleTrade);
        Parser.Register<PlayerSpells>(NetworkHandlers.HandlePlayerSpells);
    }

    public static async Task<bool> ConnectAsync()
    {
        try
        {
            _tcpClient = new TcpClient();

            await _tcpClient.ConnectAsync("127.0.0.1", Options.GamePort);

            _writeChannel = Channel.CreateUnbounded<byte[]>(new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false
            });

            _ = DoSendAndReceive(_writeChannel, _tcpClient);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public static void Disconnect()
    {
        if (_tcpClient is null)
        {
            return;
        }

        _tcpClient.Close();
        _tcpClient = null;

        _writeChannel = null;
    }

    private static async Task DoSendAndReceive(Channel<byte[]> channel, TcpClient tcpClient)
    {
        try
        {
            await Task.WhenAny(DoSend(channel, tcpClient), DoReceive(tcpClient));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        finally
        {
            channel.Writer.Complete();

            tcpClient.Close();
        }
    }

    private static async Task DoSend(Channel<byte[]> channel, TcpClient tcpClient)
    {
        var stream = tcpClient.GetStream();

        await foreach (var bytes in channel.Reader.ReadAllAsync())
        {
            await stream.WriteAsync(bytes).ConfigureAwait(false);
        }
    }

    private static async Task DoReceive(TcpClient tcpClient)
    {
        const int bufferSize = 0xffff;

        var buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
        var bufferPos = 0;

        try
        {
            var stream = tcpClient.GetStream();
            while (true)
            {
                var bytesReceived = await stream.ReadAsync(buffer.AsMemory(bufferPos)).ConfigureAwait(false);
                if (bytesReceived == 0)
                {
                    return;
                }

                Console.WriteLine($"Received {bytesReceived} bytes");

                bufferPos += bytesReceived;

                var bytesHandled = Parser.Parse(buffer.AsMemory(0, bytesReceived));
                if (bytesHandled <= 0)
                {
                    return;
                }

                var bytesLeft = bufferPos - bytesHandled;
                if (bytesLeft > 0)
                {
                    buffer.AsSpan(bytesHandled, bytesLeft).CopyTo(buffer);
                }

                bufferPos = bytesLeft;
            }
        }
        catch (IOException)
        {
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    public static void Send<TPacket>(TPacket packet) where TPacket : IPacket<TPacket>
    {
        var bytes = PacketSerializer.GetBytes(packet);

        _writeChannel?.Writer.TryWrite(bytes);
    }

    public static void Send<TPacket>() where TPacket : IPacket<TPacket>, new()
    {
        Send(new TPacket());
    }
}