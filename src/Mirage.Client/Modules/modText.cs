using System.Numerics;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace Mirage.Client.Modules;

public static class modText
{
    public const int Black = 0;
    public const int Blue = 1;
    public const int Green = 2;
    public const int Cyan = 3;
    public const int Red = 4;
    public const int Magenta = 5;
    public const int Brown = 6;
    public const int Grey = 7;
    public const int DarkGrey = 8;
    public const int BrightBlue = 9;
    public const int BrightGreen = 10;
    public const int BrightCyan = 11;
    public const int BrightRed = 12;
    public const int Pink = 13;
    public const int Yellow = 14;
    public const int White = 15;

    public const int HelpColor = Magenta;
    public const int AlertColor = Red;

    private static Color GetColorInternal(int colorCode)
    {
        return colorCode switch
        {
            Black => Color.Black,
            Blue => Color.Blue,
            Green => Color.Green,
            Cyan => Color.Cyan,
            Red => Color.Red,
            Magenta => Color.Magenta,
            Brown => Color.Brown,
            Grey => Color.Gray,
            DarkGrey => Color.DimGray,
            BrightBlue => Color.DodgerBlue,
            BrightGreen => Color.LawnGreen,
            BrightCyan => Color.LightCyan,
            BrightRed => Color.OrangeRed,
            Pink => Color.HotPink,
            Yellow => Color.Yellow,
            White => Color.White,
            _ => Color.Black,
        };
    }

    public static Vector4 GetColor(int colorCode)
    {
        var color = GetColorInternal(colorCode);

        return new Vector4(
            color.R / 255f,
            color.G / 255f,
            color.B / 255f,
            1.0f);
    }

    // public static void DrawText(int x, int y, string text, int color)
    // {
    //     var s = new Text(text, modDirectX.Font, 14);
    //     s.Position = new Vector2f(x, y);
    //     s.FillColor = GetSfmlColor(color);
    //     s.OutlineColor = SFML.Graphics.Color.Black;
    //     s.OutlineThickness = 1;
    //     modDirectX.Renderer.Draw(s);
    // }

    public static int MeasureText(string text)
    {
        return 0;
    }

    public static void AddText(string message, int colorCode)
    {
        var gameState = Ioc.Default.GetRequiredService<IGameState>();

        gameState.ChatHistory.Add(new Chat(message, colorCode));
        gameState.ChatHistoryUpdated = true;
    }
}