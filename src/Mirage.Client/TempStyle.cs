using Mirage.Engine.UI.Skins;
using Mirage.Engine.UI.Styles;

namespace Mirage.Client;

public static class TempStyle
{
    public static Skin Skin = new Skin("Content/UI");
    public static Style Style = (new Skin("Content/UI")).GetStyle("Blue");
}