using Mirage.Client.Net;
using Mirage.Game.Constants;
using Mirage.Net.Protocol.FromClient;

namespace Mirage.Client.Modules;

public static class modGameLogic
{
    // Index of actual player
    public static int MyIndex;
    
    // Used to check if in editor or not and variables for use in editor
    public static bool InEditor;

    // Map for local use
    public static modTypes.MapRec SaveMap = new();
    public static readonly modTypes.MapItemRec[] SaveMapItem = new modTypes.MapItemRec[Limits.MaxMapItems + 1];
    public static readonly modTypes.MapNpcRec[] SaveMapNpc = new modTypes.MapNpcRec[Limits.MaxMapNpcs + 1];

    // Game fps
    public static int GameFPS;

    public static void PlayerSearch(int x, int y)
    {
        var x1 = x / modTypes.PIC_X;
        var y1 = y / modTypes.PIC_Y;

        if (x1 is > 0 and <= modTypes.MAX_MAPX && y1 is >= 0 and <= modTypes.MAX_MAPY)
        {
            Network.Send(new SearchRequest(x1, y1));
        }
    }
}