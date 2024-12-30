using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;

namespace Mirage.Modules;

public static class modNetwork
{
    private static readonly CancellationTokenSource CancellationTokenSource = new();
    private static readonly Connection?[] Connections = new Connection?[modTypes.MAX_PLAYERS + 1];
    private static readonly ConcurrentQueue<int> ConnectionIds = new();
    private static bool _listening;

    private sealed record Connection(int Id, TcpClient Client, string RemoteAddr, Channel<byte[]> SendQueue);

    static modNetwork()
    {
        for (var i = 1; i <= modTypes.MAX_PLAYERS; i++)
        {
            ConnectionIds.Enqueue(i);
        }
    }

    public static void Listen()
    {
        var listening = Interlocked.Exchange(ref _listening, true);
        if (listening)
        {
            throw new InvalidOperationException("Cannot call Listen() more than once");
        }

        var tcpListener = new TcpListener(IPAddress.Any, modTypes.GAME_PORT);

        tcpListener.Start();

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

                var sendQueue = Channel.CreateUnbounded<byte[]>();
                var connection = new Connection(id, tcpClient, remoteAddr, sendQueue);

                Connections[id] = connection;

                _ = Task.Run(() => ReceiveDataAsync(connection, CancellationTokenSource.Token), cancellationToken);
                _ = Task.Run(() => SendDataAsync(connection, CancellationTokenSource.Token), cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error accepting client: {ex.Message}");
            }
        }
    }

    private static async Task ReceiveDataAsync(Connection connection, CancellationToken cancellationToken)
    {
        var stream = connection.Client.GetStream();
        var buffer = new byte[1024];

        try
        {
            modServerTCP.SocketConnected(connection.Id);

            while (!cancellationToken.IsCancellationRequested && connection.Client.Connected)
            {
                var bytesRead = await stream.ReadAsync(buffer, cancellationToken);
                if (bytesRead == 0)
                {
                    break;
                }

                modServerTCP.IncomingData(connection.Id, buffer.AsSpan(0, bytesRead));
            }
        }
        catch (IOException)
        {
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling client: {ex.Message}");
        }
        finally
        {
            modServerTCP.CloseSocket(connection.Id);

            Connections[connection.Id] = null;
            ConnectionIds.Enqueue(connection.Id);

            connection.Client.Close();
        }
    }

    private static async Task SendDataAsync(Connection connection, CancellationToken cancellationToken)
    {
        var client = connection.Client;
        var sendQueue = connection.SendQueue;

        while (!cancellationToken.IsCancellationRequested)
        {
            while (await sendQueue.Reader.WaitToReadAsync(cancellationToken))
            {
                var ok = sendQueue.Reader.TryRead(out var bytes);
                if (!ok)
                {
                    continue;
                }

                try
                {
                    var stream = client.GetStream();

                    await stream.WriteAsync(bytes, cancellationToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending data to client: {ex.Message}");
                }
            }
        }
    }

    public static bool IsConnected(int index)
    {
        return Connections[index]?.Client.Connected ?? false;
    }

    public static void SendData(int id, byte[] bytes)
    {
        var connection = Connections[id];

        connection?.SendQueue.Writer.TryWrite(bytes);
    }

    public static void Close(int id)
    {
        Connections[id]?.Client.Close();
    }

    public static string GetIP(int id)
    {
        return Connections[id]?.RemoteAddr ?? "null";
    }
}