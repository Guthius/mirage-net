using Microsoft.Xna.Framework;
using Mirage.Shared.Constants;
using Vector4 = System.Numerics.Vector4;

namespace Mirage.Client;

public static class ColorCodeTranslator
{
    private static Color GetColor(int colorCode)
    {
        return colorCode switch
        {
            ColorCode.Black => Color.Black,
            ColorCode.Blue => Color.Blue,
            ColorCode.Green => Color.Green,
            ColorCode.Cyan => Color.Cyan,
            ColorCode.Red => Color.Red,
            ColorCode.Magenta => Color.Magenta,
            ColorCode.Brown => Color.Brown,
            ColorCode.Grey => Color.Gray,
            ColorCode.DarkGrey => Color.DimGray,
            ColorCode.BrightBlue => Color.DodgerBlue,
            ColorCode.BrightGreen => Color.LawnGreen,
            ColorCode.BrightCyan => Color.LightCyan,
            ColorCode.BrightRed => Color.OrangeRed,
            ColorCode.Pink => Color.HotPink,
            ColorCode.Yellow => Color.Yellow,
            ColorCode.White => Color.White,
            _ => Color.Black
        };
    }

    public static Vector4 GetImGuiColor(int colorCode)
    {
        var color = GetColor(colorCode);

        return new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, 1.0f);
    }
}