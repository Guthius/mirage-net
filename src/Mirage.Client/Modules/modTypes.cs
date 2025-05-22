using MessagePack;
using Mirage.Game.Constants;

namespace Mirage.Client.Modules;

public static class modTypes
{
    public const int NO = 0;

    // Map constants
    public const int MAX_MAPS = 1000;
    public const int MAX_MAPX = 15;
    public const int MAX_MAPY = 11;
    public const int MAP_MORAL_NONE = 0;

    // Image constants
    public const int PIC_X = 32;
    public const int PIC_Y = 32;
    
    // Item constants
    public const int ITEM_TYPE_CURRENCY = 12;

    // Direction constants
    public const int DIR_UP = 0;
    public const int DIR_DOWN = 1;
    public const int DIR_LEFT = 2;
    public const int DIR_RIGHT = 3;
    
    // Admin constants
    public const int ADMIN_MAPPER = 2;
    public const int ADMIN_DEVELOPER = 3;
    public const int ADMIN_CREATOR = 4;

    public struct PlayerInvRec
    {
        public int Num;
        public int Value;
        public int Dur;
    }

    public struct PlayerRec()
    {
        // General
        public string Name = string.Empty;
        public byte Class;
        public int Sprite;
        public byte Level;
        public int Exp;
        public byte Access;
        public byte PK;

        // Vitals
        public int HP;
        public int MP;
        public int SP;

        // Stats
        public byte STR;
        public byte DEF;
        public byte SPEED;
        public byte MAGI;
        public byte POINTS;

        // Worn equipment
        public int ArmorSlot;
        public int WeaponSlot;
        public int HelmetSlot;
        public int ShieldSlot;

        // Inventory
        public readonly PlayerInvRec[] Inv = new PlayerInvRec[Limits.MaxInventory + 1];
        public readonly int[] Spell = new int[Limits.MaxPlayerSpells + 1];

        // Position
        public int Map;
        public byte X;
        public byte Y;
        public byte Dir;

        // Client use only
        public int MaxHP;
        public int MaxMP;
        public int MaxSP;
        public int XOffset;
        public int YOffset;
    }

    [MessagePackObject]
    public struct TileRec
    {
        [Key(0)] public int Ground;
        [Key(1)] public int Mask;
        [Key(2)] public int Anim;
        [Key(3)] public int Fringe;
        [Key(4)] public int Type;
        [Key(5)] public int Data1;
        [Key(6)] public int Data2;
        [Key(7)] public int Data3;
    }

    [MessagePackObject]
    public struct MapRec()
    {
        [Key(0)] public string Name = string.Empty;
        [Key(1)] public int Revision;
        [Key(2)] public int Moral;
        [Key(3)] public int Up;
        [Key(4)] public int Down;
        [Key(5)] public int Left;
        [Key(6)] public int Right;
        [Key(7)] public int Music;
        [Key(8)] public int BootMap;
        [Key(9)] public int BootX;
        [Key(10)] public int BootY;
        [Key(11)] public int Shop;
        [Key(12)] public TileRec[,] Tile = new TileRec[Limits.MaxMapWidth + 1, Limits.MaxMapHeight + 1];
        [Key(13)] public int[] Npc = new int[Limits.MaxMapNpcs + 1];
    }

    public struct ItemRec()
    {
        public string Name = string.Empty;

        public int Pic;
        public int Type;
        public int Data1;
        public int Data2;
        public int Data3;
    }

    public struct MapItemRec
    {
        public int Num;
        public int X;
        public int Y;
    }

    public struct NpcRec()
    {
        public string Name = string.Empty;
        public string AttackSay = string.Empty;

        public int Sprite;
        public int SpawnSecs;
        public int Behavior;
        public int Range;

        public int DropChance;
        public int DropItem;
        public int DropItemValue;

        public int STR;
        public int DEF;
        public int SPEED;
        public int MAGI;
    }

    public struct MapNpcRec
    {
        public int Num;
        public int X;
        public int Y;
        public int Dir;

        // Client use only
        public int XOffset;
        public int YOffset;
        public int Moving;
        public byte Attacking;
        public int AttackTimer;
    }

    public struct TradeItemRec
    {
        public int GiveItem;
        public int GiveValue;
        public int GetItem;
        public int GetValue;
    }

    public struct ShopRec()
    {
        public string Name = string.Empty;
        public string JoinSay = string.Empty;
        public string LeaveSay = string.Empty;
        public int FixesItems;
        public readonly TradeItemRec[] TradeItem = new TradeItemRec[Limits.MaxShopTrades + 1];
    }

    public struct SpellRec()
    {
        public string Name = string.Empty;
        public int ClassReq;
        public int LevelReq;
        public int Type;
        public int Data1;
        public int Data2;
        public int Data3;
    }

    public struct TempTileRec
    {
        public int DoorOpen;
    }
    
    public static MapRec Map = new();
    public static readonly TempTileRec[,] TempTile = new TempTileRec[MAX_MAPX + 1, MAX_MAPY + 1];
    public static readonly PlayerRec[] Player = new PlayerRec[Limits.MaxPlayers + 1];
    public static readonly ItemRec[] Item = new ItemRec[Limits.MaxItems + 1];
    public static readonly NpcRec[] Npc = new NpcRec[Limits.MaxNpcs + 1];
    public static readonly MapItemRec[] MapItem = new MapItemRec[Limits.MaxMapItems + 1];
    public static readonly MapNpcRec[] MapNpc = new MapNpcRec[Limits.MaxMapNpcs + 1];
    public static readonly ShopRec[] Shop = new ShopRec[Limits.MaxShops + 1];
    public static readonly SpellRec[] Spell = new SpellRec[Limits.MaxSpells + 1];

    static modTypes()
    {
        for (var i = 0; i < Player.Length; i++)
        {
            Player[i] = new PlayerRec();
        }

        for (var i = 0; i < Item.Length; i++)
        {
            Item[i] = new ItemRec();
        }

        for (var i = 0; i < Npc.Length; i++)
        {
            Npc[i] = new NpcRec();
        }

        for (var i = 0; i < MapItem.Length; i++)
        {
            MapItem[i] = new MapItemRec();
        }

        for (var i = 0; i < MapNpc.Length; i++)
        {
            MapNpc[i] = new MapNpcRec();
        }

        for (var i = 0; i < Shop.Length; i++)
        {
            Shop[i] = new ShopRec();
        }

        for (var i = 0; i < Spell.Length; i++)
        {
            Spell[i] = new SpellRec();
        }

        ClearTempTile();
    }

    public static void ClearTempTile()
    {
        for (var x = 0; x <= MAX_MAPX; x++)
        {
            for (var y = 0; y <= MAX_MAPY; y++)
            {
                TempTile[x, y].DoorOpen = NO;
            }
        }
    }

    public static int GetPlayerAccess(int index)
    {
        return Player[index].Access;
    }

    public static void SetPlayerHP(int index, int hp)
    {
        Player[index].HP = hp;

        if (Player[index].HP > GetPlayerMaxHP(index))
        {
            Player[index].HP = GetPlayerMaxHP(index);
        }
    }

    public static void SetPlayerMP(int index, int mp)
    {
        Player[index].MP = mp;

        if (Player[index].MP > GetPlayerMaxMP(index))
        {
            Player[index].MP = GetPlayerMaxMP(index);
        }
    }

    public static void SetPlayerSP(int index, int sp)
    {
        Player[index].SP = sp;

        if (Player[index].SP > GetPlayerMaxSP(index))
        {
            Player[index].SP = GetPlayerMaxSP(index);
        }
    }

    public static int GetPlayerMaxHP(int index)
    {
        return Player[index].MaxHP;
    }

    public static int GetPlayerMaxMP(int index)
    {
        return Player[index].MaxMP;
    }

    public static int GetPlayerMaxSP(int index)
    {
        return Player[index].MaxSP;
    }

    public static int GetPlayerDir(int index)
    {
        return Player[index].Dir;
    }
}