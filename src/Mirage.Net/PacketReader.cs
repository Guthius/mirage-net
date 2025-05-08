using System.Text;

namespace Mirage.Net;

public sealed class PacketReader(ReadOnlyMemory<byte> data)
{
    private ReadOnlyMemory<byte> _data = data;

    private ReadOnlySpan<byte> GetNextField()
    {
        if (_data.Length == 0)
        {
            return ReadOnlySpan<byte>.Empty;
        }

        ReadOnlySpan<byte> value;

        var delim = _data.Span.IndexOf((byte) PacketOptions.FieldDelimiter);
        if (delim == -1)
        {
            value = _data.Span;

            _data = ReadOnlyMemory<byte>.Empty;

            return value;
        }

        value = _data.Span[..delim];

        _data = _data[(delim + 1)..];

        return value;
    }

    public string ReadString()
    {
        var bytes = GetNextField();

        return Encoding.UTF8.GetString(bytes);
    }

    public int ReadInt32()
    {
        return int.Parse(ReadString());
    }

    public bool ReadBoolean()
    {
        return ReadInt32() != 0;
    }

    public TEnum ReadEnum<TEnum>() where TEnum : Enum
    {
        return (TEnum) Enum.ToObject(typeof(TEnum), ReadInt32());
    }
}