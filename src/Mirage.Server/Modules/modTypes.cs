using System.Text.Json.Serialization;
using MessagePack;

namespace Mirage.Modules;

public static class modTypes
{
    // Winsock globals
    public const int GAME_PORT = 4000;

    // General constants
    public const string GAME_NAME = "Mirage Online";
    public const int MAX_PLAYERS = 70;
    public const int MAX_ITEMS = 255;
    public const int MAX_NPCS = 255;
    public const int MAX_INV = 50;
    public const int MAX_MAP_ITEMS = 20;
    public const int MAX_MAP_NPCS = 5;
    public const int MAX_SHOPS = 255;
    public const int MAX_PLAYER_SPELLS = 20;
    public const int MAX_SPELLS = 255;
    public const int MAX_TRADES = 8;
    public const int MAX_GUILDS = 20;
    public const int MAX_GUILD_MEMBERS = 10;

    public const int NO = 0;
    public const int YES = 1;

    // Account constants
    public const int NAME_LENGTH = 20;
    public const int MAX_CHARS = 3;

    // Sex constants
    public const int SEX_MALE = 0;
    public const int SEX_FEMALE = 1;

    // Map constants
    public const int MAX_MAPS = 1000;
    public const int MAX_MAPX = 15;
    public const int MAX_MAPY = 11;
    public const int MAP_MORAL_NONE = 0;
    public const int MAP_MORAL_SAFE = 1;

    // Image constants
    public const int PIC_X = 32;
    public const int PIC_Y = 32;

    // Tile consants
    public const int TILE_TYPE_WALKABLE = 0;
    public const int TILE_TYPE_BLOCKED = 1;
    public const int TILE_TYPE_WARP = 2;
    public const int TILE_TYPE_ITEM = 3;
    public const int TILE_TYPE_NPCAVOID = 4;
    public const int TILE_TYPE_KEY = 5;
    public const int TILE_TYPE_KEYOPEN = 6;

    // Item constants
    public const int ITEM_TYPE_NONE = 0;
    public const int ITEM_TYPE_WEAPON = 1;
    public const int ITEM_TYPE_ARMOR = 2;
    public const int ITEM_TYPE_HELMET = 3;
    public const int ITEM_TYPE_SHIELD = 4;
    public const int ITEM_TYPE_POTIONADDHP = 5;
    public const int ITEM_TYPE_POTIONADDMP = 6;
    public const int ITEM_TYPE_POTIONADDSP = 7;
    public const int ITEM_TYPE_POTIONSUBHP = 8;
    public const int ITEM_TYPE_POTIONSUBMP = 9;
    public const int ITEM_TYPE_POTIONSUBSP = 10;
    public const int ITEM_TYPE_KEY = 11;
    public const int ITEM_TYPE_CURRENCY = 12;
    public const int ITEM_TYPE_SPELL = 13;

    // Direction constants
    public const int DIR_UP = 0;
    public const int DIR_DOWN = 1;
    public const int DIR_LEFT = 2;
    public const int DIR_RIGHT = 3;

    // Constants for player movement
    public const int MOVING_WALKING = 1;
    public const int MOVING_RUNNING = 2;

    // Weather constants
    public const int WEATHER_NONE = 0;
    public const int WEATHER_RAINING = 1;
    public const int WEATHER_SNOWING = 2;

    // Time constants
    public const int TIME_DAY = 0;
    public const int TIME_NIGHT = 1;

    // Admin constants
    public const int ADMIN_MONITER = 1;
    public const int ADMIN_MAPPER = 2;
    public const int ADMIN_DEVELOPER = 3;
    public const int ADMIN_CREATOR = 4;

    // NPC constants
    public const int NPC_BEHAVIOR_ATTACKONSIGHT = 0;
    public const int NPC_BEHAVIOR_ATTACKWHENATTACKED = 1;
    public const int NPC_BEHAVIOR_FRIENDLY = 2;
    public const int NPC_BEHAVIOR_SHOPKEEPER = 3;
    public const int NPC_BEHAVIOR_GUARD = 4;

    // Spell constants
    public const int SPELL_TYPE_ADDHP = 0;
    public const int SPELL_TYPE_ADDMP = 1;
    public const int SPELL_TYPE_ADDSP = 2;
    public const int SPELL_TYPE_SUBHP = 3;
    public const int SPELL_TYPE_SUBMP = 4;
    public const int SPELL_TYPE_SUBSP = 5;
    public const int SPELL_TYPE_GIVEITEM = 6;

    // Target type constants
    public const int TARGET_TYPE_PLAYER = 0;
    public const int TARGET_TYPE_NPC = 1;


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
        public int Sex;
        public int Class;
        public int Sprite;
        public int Level;
        public int Exp;
        public int Access;
        public int PK;
        public int Guild;

        // Vitals
        public int HP;
        public int MP;
        public int SP;

        // Stats
        public int STR;
        public int DEF;
        public int SPEED;
        public int MAGI;
        public int POINTS;

        // Worn equipment
        public int ArmorSlot;
        public int WeaponSlot;
        public int HelmetSlot;
        public int ShieldSlot;

        // Inventory
        public readonly PlayerInvRec[] Inv = new PlayerInvRec[MAX_INV + 1];
        public readonly int[] Spell = new int[MAX_PLAYER_SPELLS + 1];

        // Position
        public int Map;
        public int X;
        public int Y;
        public int Dir;
    }

    public struct AccountRec()
    {
        // Account
        public string Login = string.Empty;
        public string Password = string.Empty;

        // Characters
        public PlayerRec[] Char = new PlayerRec[MAX_CHARS + 1];

        // None saved local vars
        [JsonIgnore] public byte[] Buffer = new byte[0xFFFF];
        [JsonIgnore] public int BufferPos;
        [JsonIgnore] public int CharNum;
        [JsonIgnore] public bool InGame;
        [JsonIgnore] public int AttackTimer;
        [JsonIgnore] public int DataTimer;
        [JsonIgnore] public int DataBytes;
        [JsonIgnore] public int DataPackets;
        [JsonIgnore] public int PartyPlayer;
        [JsonIgnore] public int InParty;
        [JsonIgnore] public int TargetType;
        [JsonIgnore] public int Target;
        [JsonIgnore] public int CastedSpell;
        [JsonIgnore] public int PartyStarter;
        [JsonIgnore] public int GettingMap;
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
        [Key(12)] public int Indoors;
        [Key(13)] public readonly TileRec[,] Tile = new TileRec[MAX_MAPX + 1, MAX_MAPY + 1];
        [Key(14)] public int[] Npc = new int[MAX_MAP_NPCS + 1];
    }

    public struct ClassRec()
    {
        public string Name = string.Empty;

        public int Sprite;

        public int STR;
        public int DEF;
        public int SPEED;
        public int MAGI;
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
        public int Value;
        public int Dur;

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

        public int Target;

        public int HP;
        public int MP;
        public int SP;

        public int X;
        public int Y;
        public int Dir;

        // For server use only
        public int SpawnWait;
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
        public readonly TradeItemRec[] TradeItem = new TradeItemRec[MAX_TRADES + 1];
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

    public struct TempTileRec()
    {
        public readonly int[,] DoorOpen = new int[MAX_MAPX + 1, MAX_MAPY + 1];
        public long DoorTimer;
    }

    public struct GuildRec()
    {
        public string Name = string.Empty;
        public string Founder = string.Empty;
        public string[] Member = new string[MAX_GUILD_MEMBERS + 1];
    }

    // Used for parsing
    public static char SEP_CHAR = (char) 0;
    public static char END_CHAR = (char) 237;

    // Maximum classes
    public static int Max_Classes;

    public static readonly MapRec[] Map = new MapRec[MAX_MAPS + 1];
    public static readonly TempTileRec[] TempTile = new TempTileRec[MAX_MAPS + 1];
    public static readonly int[] PlayersOnMap = new int[MAX_MAPS + 1];
    public static readonly AccountRec[] Player = new AccountRec[MAX_PLAYERS + 1];
    public static ClassRec[] Class = [];
    public static readonly ItemRec[] Item = new ItemRec[MAX_ITEMS + 1];
    public static readonly NpcRec[] Npc = new NpcRec[MAX_NPCS + 1];
    public static readonly MapItemRec[,] MapItem = new MapItemRec[MAX_MAPS + 1, MAX_MAP_ITEMS + 1];
    public static readonly MapNpcRec[,] MapNpc = new MapNpcRec[MAX_MAPS + 1, MAX_MAP_NPCS + 1];
    public static readonly ShopRec[] Shop = new ShopRec[MAX_SHOPS + 1];
    public static readonly SpellRec[] Spell = new SpellRec[MAX_SPELLS + 1];
    public static readonly GuildRec[] Guild = new GuildRec[MAX_GUILDS + 1];

    public static void ClearTempTile()
    {
        for (var i = 1; i <= MAX_MAPS; i++)
        {
            TempTile[i] = new TempTileRec();

            for (var y = 0; y <= MAX_MAPY; y++)
            {
                for (var x = 0; x <= MAX_MAPX; x++)
                {
                    TempTile[i].DoorOpen[x, y] = NO;
                }
            }
        }
    }

    public static void ClearClasses()
    {
        for (var i = 0; i < Max_Classes; i++)
        {
            Class[i] = new ClassRec();
        }
    }

    public static void ClearPlayer(int index)
    {
        Player[index] = new AccountRec();
        for (var i = 1; i <= MAX_CHARS; i++)
        {
            Player[index].Char[i] = new PlayerRec();
        }
    }

    public static void ClearChar(int index, int charNum)
    {
        Player[index].Char[charNum] = new PlayerRec();
    }

    public static void ClearItem(int index)
    {
        Item[index].Name = string.Empty;

        Item[index].Type = 0;
        Item[index].Data1 = 0;
        Item[index].Data2 = 0;
        Item[index].Data3 = 0;
    }

    public static void ClearItems()
    {
        for (var i = 1; i <= MAX_ITEMS; i++)
        {
            ClearItem(i);
        }
    }

    public static void ClearNpc(int index)
    {
        Npc[index].Name = string.Empty;
        Npc[index].AttackSay = string.Empty;
        Npc[index].Sprite = 0;
        Npc[index].SpawnSecs = 0;
        Npc[index].Behavior = 0;
        Npc[index].Range = 0;
        Npc[index].DropChance = 0;
        Npc[index].DropItem = 0;
        Npc[index].DropItemValue = 0;
        Npc[index].STR = 0;
        Npc[index].DEF = 0;
        Npc[index].SPEED = 0;
        Npc[index].MAGI = 0;
    }

    public static void ClearNpcs()
    {
        for (var i = 1; i <= MAX_NPCS; i++)
        {
            ClearNpc(i);
        }
    }

    public static void ClearMapItem(int index, int mapNum)
    {
        MapItem[mapNum, index].Num = 0;
        MapItem[mapNum, index].Value = 0;
        MapItem[mapNum, index].Dur = 0;
        MapItem[mapNum, index].X = 0;
        MapItem[mapNum, index].Y = 0;
    }

    public static void ClearMapItems()
    {
        for (var mapNum = 1; mapNum <= MAX_MAPS; mapNum++)
        {
            for (var i = 1; i <= MAX_MAP_ITEMS; i++)
            {
                ClearMapItem(i, mapNum);
            }
        }
    }

    public static void ClearMapNpc(int index, int mapNum)
    {
        MapNpc[mapNum, index].Num = 0;
        MapNpc[mapNum, index].Target = 0;
        MapNpc[mapNum, index].HP = 0;
        MapNpc[mapNum, index].MP = 0;
        MapNpc[mapNum, index].SP = 0;
        MapNpc[mapNum, index].X = 0;
        MapNpc[mapNum, index].Y = 0;
        MapNpc[mapNum, index].Dir = 0;

        // Server use only
        MapNpc[mapNum, index].SpawnWait = 0;
        MapNpc[mapNum, index].AttackTimer = 0;
    }

    public static void ClearMapNpcs()
    {
        for (var mapNum = 1; mapNum <= MAX_MAPS; mapNum++)
        {
            for (var i = 1; i <= MAX_MAP_NPCS; i++)
            {
                ClearMapNpc(i, mapNum);
            }
        }
    }

    public static void ClearMap(int mapNum)
    {
        Map[mapNum] = new MapRec();

        for (var y = 0; y <= MAX_MAPY; y++)
        {
            for (var x = 0; x <= MAX_MAPX; x++)
            {
                Map[mapNum].Tile[x, y].Ground = 0;
                Map[mapNum].Tile[x, y].Mask = 0;
                Map[mapNum].Tile[x, y].Anim = 0;
                Map[mapNum].Tile[x, y].Fringe = 0;
                Map[mapNum].Tile[x, y].Type = 0;
                Map[mapNum].Tile[x, y].Data1 = 0;
                Map[mapNum].Tile[x, y].Data2 = 0;
                Map[mapNum].Tile[x, y].Data3 = 0;
            }
        }

        // Reset the values for if a player is on the map or not
        PlayersOnMap[mapNum] = NO;
    }

    public static void ClearMaps()
    {
        for (var i = 1; i <= MAX_MAPS; i++)
        {
            ClearMap(i);
        }
    }

    public static void ClearShop(int index)
    {
        Shop[index] = new ShopRec();

        for (var i = 1; i <= MAX_TRADES; i++)
        {
            Shop[index].TradeItem[i].GiveItem = 0;
            Shop[index].TradeItem[i].GiveValue = 0;
            Shop[index].TradeItem[i].GetItem = 0;
            Shop[index].TradeItem[i].GetValue = 0;
        }
    }

    public static void ClearShops()
    {
        for (var i = 1; i <= MAX_SHOPS; i++)
        {
            ClearShop(i);
        }
    }

    public static void ClearSpell(int index)
    {
        Spell[index].Name = string.Empty;
        Spell[index].ClassReq = 0;
        Spell[index].LevelReq = 0;
        Spell[index].Type = 0;
        Spell[index].Data1 = 0;
        Spell[index].Data2 = 0;
        Spell[index].Data3 = 0;
    }

    public static void ClearSpells()
    {
        for (var i = 1; i <= MAX_SPELLS; i++)
        {
            ClearSpell(i);
        }
    }

    //////////////////////
    // PLAYER FUNCTIONS //
    //////////////////////


    public static string GetPlayerLogin(int index)
    {
        return Player[index].Login;
    }

    public static void SetPlayerLogin(int index, string login)
    {
        Player[index].Login = login;
    }

    public static string GetPlayerPassword(int index)
    {
        return Player[index].Password;
    }

    public static void SetPlayerPassword(int index, string password)
    {
        Player[index].Password = password;
    }

    public static string GetPlayerName(int index)
    {
        return Player[index].Char[Player[index].CharNum].Name;
    }

    public static void SetPlayerName(int index, string name)
    {
        Player[index].Char[Player[index].CharNum].Name = name;
    }

    public static int GetPlayerClass(int index)
    {
        return Player[index].Char[Player[index].CharNum].Class;
    }

    public static void SetPlayerClass(int index, int classNum)
    {
        Player[index].Char[Player[index].CharNum].Class = classNum;
    }

    public static int GetPlayerSprite(int index)
    {
        return Player[index].Char[Player[index].CharNum].Sprite;
    }

    public static void SetPlayerSprite(int index, int sprite)
    {
        Player[index].Char[Player[index].CharNum].Sprite = sprite;
    }

    public static int GetPlayerLevel(int index)
    {
        return Player[index].Char[Player[index].CharNum].Level;
    }

    public static void SetPlayerLevel(int index, int level)
    {
        Player[index].Char[Player[index].CharNum].Level = level;
    }

    public static int GetPlayerNextLevel(int index)
    {
        return (GetPlayerLevel(index) + 1) * (GetPlayerSTR(index) + GetPlayerDEF(index) + GetPlayerMAGI(index) + GetPlayerSPEED(index) + GetPlayerPOINTS(index)) * 25;
    }

    public static int GetPlayerExp(int index)
    {
        return Player[index].Char[Player[index].CharNum].Exp;
    }

    public static void SetPlayerExp(int index, int exp)
    {
        Player[index].Char[Player[index].CharNum].Exp = exp;
    }

    public static int GetPlayerAccess(int index)
    {
        return Player[index].Char[Player[index].CharNum].Access;
    }

    public static void SetPlayerAccess(int index, int access)
    {
        Player[index].Char[Player[index].CharNum].Access = access;
    }

    public static int GetPlayerPK(int index)
    {
        return Player[index].Char[Player[index].CharNum].PK;
    }

    public static void SetPlayerPK(int index, int pk)
    {
        Player[index].Char[Player[index].CharNum].PK = pk;
    }

    public static int GetPlayerHP(int index)
    {
        return Player[index].Char[Player[index].CharNum].HP;
    }

    public static void SetPlayerHP(int index, int hp)
    {
        Player[index].Char[Player[index].CharNum].HP = hp;

        if (GetPlayerHP(index) > GetPlayerMaxHP(index))
        {
            Player[index].Char[Player[index].CharNum].HP = GetPlayerMaxHP(index);
        }

        if (GetPlayerHP(index) < 0)
        {
            Player[index].Char[Player[index].CharNum].HP = 0;
        }
    }

    public static int GetPlayerMP(int index)
    {
        return Player[index].Char[Player[index].CharNum].MP;
    }

    public static void SetPlayerMP(int index, int mp)
    {
        Player[index].Char[Player[index].CharNum].MP = mp;

        if (GetPlayerMP(index) > GetPlayerMaxMP(index))
        {
            Player[index].Char[Player[index].CharNum].MP = GetPlayerMaxMP(index);
        }

        if (GetPlayerMP(index) < 0)
        {
            Player[index].Char[Player[index].CharNum].MP = 0;
        }
    }

    public static int GetPlayerSP(int index)
    {
        return Player[index].Char[Player[index].CharNum].SP;
    }

    public static void SetPlayerSP(int index, int sp)
    {
        Player[index].Char[Player[index].CharNum].SP = sp;

        if (GetPlayerSP(index) > GetPlayerMaxSP(index))
        {
            Player[index].Char[Player[index].CharNum].SP = GetPlayerMaxSP(index);
        }

        if (GetPlayerSP(index) < 0)
        {
            Player[index].Char[Player[index].CharNum].SP = 0;
        }
    }

    public static int GetPlayerMaxHP(int index)
    {
        var charNum = Player[index].CharNum;
        return (Player[index].Char[charNum].Level + GetPlayerSTR(index) / 2 + Class[Player[index].Char[charNum].Class].STR) * 2;
    }

    public static int GetPlayerMaxMP(int index)
    {
        var charNum = Player[index].CharNum;
        return (Player[index].Char[charNum].Level + GetPlayerMAGI(index) / 2 + Class[Player[index].Char[charNum].Class].MAGI) * 2;
    }

    public static int GetPlayerMaxSP(int index)
    {
        var charNum = Player[index].CharNum;
        return (Player[index].Char[charNum].Level + GetPlayerSPEED(index) / 2 + Class[Player[index].Char[charNum].Class].SPEED) * 2;
    }

    public static string GetClassName(int classNum)
    {
        return Class[classNum].Name;
    }

    public static int GetClassMaxHP(int classNum)
    {
        return (1 + Class[classNum].STR / 2 + Class[classNum].STR) * 2;
    }

    public static int GetClassMaxMP(int classNum)
    {
        return (1 + Class[classNum].MAGI / 2 + Class[classNum].MAGI) * 2;
    }

    public static int GetClassMaxSP(int classNum)
    {
        return (1 + Class[classNum].SPEED / 2 + Class[classNum].SPEED) * 2;
    }

    public static int GetClassSTR(int classNum)
    {
        return Class[classNum].STR;
    }

    public static int GetClassDEF(int classNum)
    {
        return Class[classNum].DEF;
    }

    public static int GetClassSPEED(int classNum)
    {
        return Class[classNum].SPEED;
    }

    public static int GetClassMAGI(int classNum)
    {
        return Class[classNum].MAGI;
    }

    public static int GetPlayerSTR(int index)
    {
        return Player[index].Char[Player[index].CharNum].STR;
    }

    public static void SetPlayerSTR(int index, int str)
    {
        Player[index].Char[Player[index].CharNum].STR = str;
    }

    public static int GetPlayerDEF(int index)
    {
        return Player[index].Char[Player[index].CharNum].DEF;
    }

    public static void SetPlayerDEF(int index, int def)
    {
        Player[index].Char[Player[index].CharNum].DEF = def;
    }

    public static int GetPlayerSPEED(int index)
    {
        return Player[index].Char[Player[index].CharNum].SPEED;
    }

    public static void SetPlayerSPEED(int index, int speed)
    {
        Player[index].Char[Player[index].CharNum].SPEED = speed;
    }

    public static int GetPlayerMAGI(int index)
    {
        return Player[index].Char[Player[index].CharNum].MAGI;
    }

    public static void SetPlayerMAGI(int index, int magi)
    {
        Player[index].Char[Player[index].CharNum].MAGI = magi;
    }

    public static int GetPlayerPOINTS(int index)
    {
        return Player[index].Char[Player[index].CharNum].POINTS;
    }

    public static void SetPlayerPOINTS(int index, int points)
    {
        Player[index].Char[Player[index].CharNum].POINTS = points;
    }

    public static int GetPlayerMap(int index)
    {
        return Player[index].Char[Player[index].CharNum].Map;
    }

    public static void SetPlayerMap(int index, int mapNum)
    {
        if (mapNum > 0 && mapNum <= MAX_MAPS)
        {
            Player[index].Char[Player[index].CharNum].Map = mapNum;
        }
    }

    public static int GetPlayerX(int index)
    {
        return Player[index].Char[Player[index].CharNum].X;
    }

    public static void SetPlayerX(int index, int x)
    {
        Player[index].Char[Player[index].CharNum].X = x;
    }

    public static int GetPlayerY(int index)
    {
        return Player[index].Char[Player[index].CharNum].Y;
    }

    public static void SetPlayerY(int index, int y)
    {
        Player[index].Char[Player[index].CharNum].Y = y;
    }

    public static int GetPlayerDir(int index)
    {
        return Player[index].Char[Player[index].CharNum].Dir;
    }

    public static void SetPlayerDir(int index, int dir)
    {
        Player[index].Char[Player[index].CharNum].Dir = dir;
    }

    public static string GetPlayerIP(int index)
    {
        return modNetwork.GetIP(index);
    }

    public static int GetPlayerInvItemNum(int index, int invSlot)
    {
        return Player[index].Char[Player[index].CharNum].Inv[invSlot].Num;
    }

    public static void SetPlayerInvItemNum(int index, int invSlot, int itemNum)
    {
        Player[index].Char[Player[index].CharNum].Inv[invSlot].Num = itemNum;
    }

    public static int GetPlayerInvItemValue(int index, int invSlot)
    {
        return Player[index].Char[Player[index].CharNum].Inv[invSlot].Value;
    }

    public static void SetPlayerInvItemValue(int index, int invSlot, int itemValue)
    {
        Player[index].Char[Player[index].CharNum].Inv[invSlot].Value = itemValue;
    }

    public static int GetPlayerInvItemDur(int index, int invSlot)
    {
        return Player[index].Char[Player[index].CharNum].Inv[invSlot].Dur;
    }

    public static void SetPlayerInvItemDur(int index, int invSlot, int itemDur)
    {
        Player[index].Char[Player[index].CharNum].Inv[invSlot].Dur = itemDur;
    }

    public static int GetPlayerSpell(int index, int spellSlot)
    {
        return Player[index].Char[Player[index].CharNum].Spell[spellSlot];
    }

    public static void SetPlayerSpell(int index, int spellSlot, int spellNum)
    {
        Player[index].Char[Player[index].CharNum].Spell[spellSlot] = spellNum;
    }

    public static int GetPlayerArmorSlot(int index)
    {
        return Player[index].Char[Player[index].CharNum].ArmorSlot;
    }

    public static void SetPlayerArmorSlot(int index, int invNum)
    {
        Player[index].Char[Player[index].CharNum].ArmorSlot = invNum;
    }

    public static int GetPlayerWeaponSlot(int index)
    {
        return Player[index].Char[Player[index].CharNum].WeaponSlot;
    }

    public static void SetPlayerWeaponSlot(int index, int invNum)
    {
        Player[index].Char[Player[index].CharNum].WeaponSlot = invNum;
    }

    public static int GetPlayerHelmetSlot(int index)
    {
        return Player[index].Char[Player[index].CharNum].HelmetSlot;
    }

    public static void SetPlayerHelmetSlot(int index, int invNum)
    {
        Player[index].Char[Player[index].CharNum].HelmetSlot = invNum;
    }

    public static int GetPlayerShieldSlot(int index)
    {
        return Player[index].Char[Player[index].CharNum].ShieldSlot;
    }

    public static void SetPlayerShieldSlot(int index, int invNum)
    {
        Player[index].Char[Player[index].CharNum].ShieldSlot = invNum;
    }
}