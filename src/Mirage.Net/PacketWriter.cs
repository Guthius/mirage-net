using System.Text;

namespace Mirage.Net;

public sealed class PacketWriter
{
    private const int InitialCapacity = 32;

    private readonly StringBuilder _stringBuilder = new(InitialCapacity);

    public void WriteBytes(ReadOnlySpan<byte> bytes)
    {
        WriteString(Convert.ToBase64String(bytes));
    }

    public void WriteString(string value)
    {
        _stringBuilder.Append(value);
        _stringBuilder.Append(PacketOptions.FieldDelimiter);
    }

    public void WriteInt32(int value)
    {
        _stringBuilder.Append(value);
        _stringBuilder.Append(PacketOptions.FieldDelimiter);
    }

    public void WriteBoolean(bool value)
    {
        WriteInt32(value ? 1 : 0);
    }

    public void WriteEnum<TEnum>(TEnum value) where TEnum : Enum
    {
        WriteInt32(Convert.ToInt32(value));
    }

    public void WriteList<T>(List<T> list, Action<T> writer)
    {
        WriteInt32(list.Count);

        foreach (var item in list)
        {
            writer(item);
        }
    }

    public override string ToString()
    {
        return _stringBuilder.ToString();
    }
}