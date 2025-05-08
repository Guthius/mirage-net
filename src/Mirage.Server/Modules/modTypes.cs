using Mirage.Game.Constants;
using Mirage.Game.Data;
using Mirage.Server.Game;

namespace Mirage.Server.Modules;

public static class modTypes
{
    public static List<ClassInfo> Classes { get; set; } = [];
    
    public static readonly MapInfo[] Maps = new MapInfo[Limits.MaxMaps + 1];
    public static readonly ItemInfo[] Items = new ItemInfo[Limits.MaxItems + 1];
    public static readonly NpcInfo[] Npcs = new NpcInfo[Limits.MaxNpcs + 1];
    public static readonly ShopInfo[] Shops = new ShopInfo[Limits.MaxShops + 1];
    public static readonly SpellInfo[] Spells = new SpellInfo[Limits.MaxSpells + 1];
    
    public static string GetPlayerName(int index)
    {
        return GameState.Sessions[index].Player.Character.Name;
    }
}