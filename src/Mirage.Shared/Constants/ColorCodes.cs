using Mirage.Shared.Data;

namespace Mirage.Shared.Constants;

public static class ColorCodes
{
    public static readonly ColorCode White = new(255, 255, 255);
    public static readonly ColorCode Red = new(235, 51, 36);
    public static readonly ColorCode Green = new(0, 255, 0);
    public static readonly ColorCode Blue = new(0, 35, 245);
    public static readonly ColorCode Cyan = new(115, 251, 253);
    public static readonly ColorCode Pink = new(238, 138, 248);
    public static readonly ColorCode Yellow = new(255, 253, 85);
    public static readonly ColorCode Brown = new(120, 67, 21);
    public static readonly ColorCode Grey = new(128, 128, 128);
    public static readonly ColorCode DarkGrey = new(64, 64, 64);

    public static readonly ColorCode SayColor = Grey;
    public static readonly ColorCode GlobalColor = Blue;
    public static readonly ColorCode BroadcastColor = Pink;
    public static readonly ColorCode TellColor = Green;
    public static readonly ColorCode EmoteColor = Cyan;
    public static readonly ColorCode AdminColor = Cyan;
    public static readonly ColorCode HelpColor = Pink;
    public static readonly ColorCode WhoColor = Pink;
    public static readonly ColorCode JoinLeftColor = DarkGrey;
    public static readonly ColorCode AlertColor = Red;
}