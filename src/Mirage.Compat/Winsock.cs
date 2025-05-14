using System.ComponentModel;
using System.Net.Sockets;
using System.Threading.Channels;

namespace Mirage.Compat;

public sealed class Winsock : Component
{
    private Socket? _socket;
    private readonly Lock _sendLock = new();
    private readonly Channel<byte[]> _sendQueue = Channel.CreateUnbounded<byte[]>();
    private bool _sending;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public string RemoteHost { get; set; } = string.Empty;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public int RemotePort { get; set; }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public WinsockState State { get; private set; } = WinsockState.Disconnected;

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