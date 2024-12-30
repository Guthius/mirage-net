using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;

namespace Mirage.Compat;

public sealed class Winsock : Component
{
    private const int RecvBufferSize = 102400;

    private readonly Lock _lock = new();
    private readonly byte[] _recvBuffer = new byte[RecvBufferSize];
    private readonly SynchronizationContext _synchronizationContext = SynchronizationContext.Current!;
    private Socket? _socket;
    private readonly Lock _sendLock = new();
    private readonly Channel<byte[]> _sendQueue = Channel.CreateUnbounded<byte[]>();
    private bool _sending;

    public event EventHandler<DataArrivalEventArgs>? DataArrival;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public string RemoteHost { get; set; } = string.Empty;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public int RemotePort { get; set; }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public WinsockState State { get; private set; } = WinsockState.Disconnected;
    
    public string LocalIP
    {
        get
        {
            if (_socket?.RemoteEndPoint is IPEndPoint ep)
            {
                return ep.Address.ToString();
            }

            return "unknown";
        }
    }

    public int LocalPort
    {
        get
        {
            if (_socket?.RemoteEndPoint is IPEndPoint ep)
            {
                return ep.Port;
            }

            return 0;
        }
    }
    
    public void Close()
    {
        lock (_lock)
        {
            if (State == WinsockState.Disconnected)
            {
                return;
            }

            _socket?.Close();
            State = WinsockState.Disconnected;
        }
    }

    public void Connect()
    {
        lock (_lock)
        {
            if (State != WinsockState.Disconnected)
            {
                return;
            }

            State = WinsockState.Connecting;
        }

        try
        {
            _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            _socket.BeginConnect(RemoteHost, RemotePort, EndConnect, null);
        }
        catch
        {
            Close();
        }
    }

    private void EndConnect(IAsyncResult ar)
    {
        if (_socket is null)
        {
            return;
        }

        try
        {
            _socket.EndConnect(ar);

            if (_socket.Connected)
            {
                State = WinsockState.Connected;

                BeginReceive();
            }
            else
            {
                Close();
            }
        }
        catch
        {
            Close();
        }
    }

    private void BeginReceive()
    {
        if (_socket is null)
        {
            return;
        }

        try
        {
            _socket.BeginReceive(
                _recvBuffer, 0, RecvBufferSize,
                SocketFlags.None,
                EndReceive,
                null);
        }
        catch
        {
            Close();
        }
    }

    private void EndReceive(IAsyncResult ar)
    {
        if (_socket is null)
        {
            return;
        }

        try
        {
            var bytesReceived = _socket.EndReceive(ar);
            if (bytesReceived == 0)
            {
                Close();
                return;
            }

            var bytes = _recvBuffer.AsSpan(0, bytesReceived).ToArray();

            _synchronizationContext.Post(_ => DataArrival?.Invoke(this, new DataArrivalEventArgs(bytes)), null);

            BeginReceive();
        }
        catch
        {
            Close();
        }
    }
    
    public void SendData(byte[] bytes)
    {
        lock (_sendLock)
        {
            _sendQueue.Writer.TryWrite(bytes);

            if (_sending)
            {
                return;
            }

            _sending = true;
        }

        BeginSend();
    }

    private void BeginSend()
    {
        var socket = _socket;
        if (socket is null)
        {
            return;
        }

        while (true)
        {
            byte[]? data;
            lock (_sendLock)
            {
                var dataToSend = _sendQueue.Reader.TryRead(out data);
                if (!dataToSend)
                {
                    _sending = false;
                    return;
                }

                if (data is null)
                {
                    continue;
                }
            }

            socket.BeginSend(data, 0, data.Length, SocketFlags.None, EndSend, null);
            break;
        }
    }

    private void EndSend(IAsyncResult ar)
    {
        if (_socket is null)
        {
            return;
        }

        var bytesSent = _socket.EndSend(ar);
        if (bytesSent == 0)
        {
            return;
        }

        BeginSend();
    }
}