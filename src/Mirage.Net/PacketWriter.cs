using System.Text;

namespace Mirage.Net;

public sealed class PacketWriter
{
    private const int InitialCapacity = 32;
    
    private readonly StringBuilder _stringBuilder = new(InitialCapacity);
    
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

    public override string ToString()
    {
        return _stringBuilder.ToString();
    }
}