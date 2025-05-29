using System.Buffers;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mirage.Net;
using Mirage.Net.Protocol.FromServer;
using Mirage.Server.Chat;
using Mirage.Server.Players;
using Mirage.Server.Repositories.Accounts;
using Mirage.Server.Repositories.Bans;
using Mirage.Server.Repositories.Characters;
using Mirage.Server.Repositories.Jobs;

namespace Mirage.Server.Net;

public sealed partial class NetworkService : BackgroundService
{
    private readonly ILogger<NetworkService> _logger;
    private readonly IChatService _chatService;
    private readonly IPlayerService _playerService;
    private readonly IAccountRepository _accountRepository;
    private readonly IBanRepository _banRepository;
    private readonly ICharacterRepository _characterRepository;
    private readonly IJobRepository _jobRepository;
    private readonly NetworkOptions _options;
    private readonly ConcurrentQueue<int> _connectionIds = [];
    private readonly ConcurrentDictionary<int, NetworkConnection> _connections = [];
    private readonly NetworkParser _parser;

    public NetworkService(
        ILogger<NetworkService> logger,
        IOptions<NetworkOptions> options,
        IChatService chatService,
        IPlayerService playerService,
        IAccountRepository accountRepository,
        IBanRepository banRepository,
        ICharacterRepository characterRepository,
        IJobRepository jobRepository)
    {
        _logger = logger;
        _chatService = chatService;
        _playerService = playerService;
        _accountRepository = accountRepository;
        _banRepository = banRepository;
        _characterRepository = characterRepository;
        _jobRepository = jobRepository;
        _options = options.Value;
        _parser = new NetworkParser(ReportBadPacket);

        foreach (var id in Enumerable.Range(1, options.Value.MaxConnections))
        {
            _connectionIds.Enqueue(id);
        }

        RegisterPackets();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var tcpListener = new TcpListener(IPAddress.Any, _options.Port);

        tcpListener.Start();

        _logger.LogInformation("Network service started on port {Port}", _options.Port);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var tcpClient = await tcpListener.AcceptTcpClientAsync(stoppingToken);

                _ = RunClient(tcpClient, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accepting client");
            }
        }

        _logger.LogInformation("Network service stopped");
    }

    private async Task RunClient(TcpClient tcpClient, CancellationToken stoppingToken)
    {
        var address = string.Empty;
        if (tcpClient.Client.RemoteEndPoint is IPEndPoint ipEndPoint)
        {
            address = ipEndPoint.Address.ToString();
        }

        _logger.LogInformation("Received connection from {Address}", address);

        if (!_connectionIds.TryDequeue(out var id))
        {
            _logger.LogWarning("Unable to handle client connection from {Address} (connection limit reached)", address);

            tcpClient.Close();

            return;
        }

        var channel = Channel.CreateUnbounded<byte[]>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });

        if (_banRepository.IsBanned(address))
        {
            var disconnectCommand = PacketSerializer.GetBytes(new DisconnectCommand(
                "You have been banned and can no longer play."));

            await channel.Writer.WriteAsync(disconnectCommand, stoppingToken);

            channel.Writer.TryComplete();
        }

        var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
        try
        {
            var connection = _connections[id] = new NetworkConnection(id, address, channel);

            await Task.WhenAny(
                RunClientReceive(connection, tcpClient,
                    linkedTokenSource.Token),
                RunClientSend(tcpClient, channel,
                    linkedTokenSource.Token));
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception while handling client connection from {Address}", address);
        }
        finally
        {
            channel.Writer.TryComplete();

            _logger.LogInformation("Connection with {Address} has been terminated", address);

            if (_connections.TryRemove(id, out var connection) && connection.Player is not null)
            {
                _playerService.Destroy(connection.Player);
            }

            linkedTokenSource.Dispose();
        }
    }

    private async Task RunClientSend(TcpClient tcpClient, Channel<byte[]> sendChannel, CancellationToken cancellationToken)
    {
        var stream = tcpClient.GetStream();

        await foreach (var bytes in sendChannel.Reader.ReadAllAsync(cancellationToken))
        {
            try
            {
                await stream.WriteAsync(bytes, cancellationToken);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception while sending data to client");
            }
        }
    }

    private async Task RunClientReceive(NetworkConnection connection, TcpClient tcpClient, CancellationToken cancellationToken)
    {
        const int bufferSize = 0xffff;

        var stream = tcpClient.GetStream();

        var buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
        var bufferPos = 0;

        try
        {
            while (!cancellationToken.IsCancellationRequested && tcpClient.Connected)
            {
                var bytesRead = await stream.ReadAsync(buffer, cancellationToken);
                if (bytesRead == 0)
                {
                    break;
                }

                bufferPos += bytesRead;

                var bytesHandled = _parser.Parse(connection, buffer.AsMemory(0, bufferPos));
                if (bytesHandled <= 0)
                {
                    continue;
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
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception while receiving data from client");
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    private bool IsAccountLoggedIn(string accountName)
    {
        foreach (var account in _connections.Values.Select(x => x.Account))
        {
            if (account is not null && account.Name.Equals(accountName, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private void ReportBadPacket(NetworkConnection connection, string packetId)
    {
        _logger.LogWarning("Received unsupported packet ({PacketId}) from {Address}", packetId, connection.Address);
    }
}