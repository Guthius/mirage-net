using System.Text.Json;
using MessagePack;

namespace Mirage.Modules;

public static class modDatabase
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        IncludeFields = true
    };

    public const int START_MAP = 1;
    public const int START_X = modTypes.MAX_MAPX / 2;
    public const int START_Y = modTypes.MAX_MAPY / 2;

    public const string ADMIN_LOG = "admin.txt";
    public const string PLAYER_LOG = "player.txt";

    private static string GetAccountPath(string name)
    {
        var filename = name.Trim().ToLower() + ".json";

        return Path.Combine("Accounts", filename);
    }

    private static string GetAccountPath(int index)
    {
        return GetAccountPath(modTypes.Player[index].Login);
    }


    public static void SavePlayer(int index)
    {
        var json = JsonSerializer.Serialize(modTypes.Player[index], JsonOptions);

        File.WriteAllText(GetAccountPath(index), json);
    }

    public static void DeleteAccount(string name)
    {
        var path = GetAccountPath(name);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
    
    public static void LoadPlayer(int index, string name)
    {
        var json = File.ReadAllText(GetAccountPath(name));

        modTypes.Player[index] = JsonSerializer.Deserialize<modTypes.AccountRec>(json, JsonOptions);
    }

    public static bool AccountExist(string name)
    {
        return File.Exists(GetAccountPath(name));
    }

    public static bool CharExist(int index, int charNum)
    {
        return !string.IsNullOrWhiteSpace(modTypes.Player[index].Char[charNum].Name);
    }

    public static bool PasswordOk(string name, string password)
    {
        if (!AccountExist(name))
        {
            return false;
        }

        var json = File.ReadAllText(GetAccountPath(name));

        var account = JsonSerializer.Deserialize<modTypes.AccountRec>(json, JsonOptions);

        return string.Equals(account.Password, password);
    }

    public static void AddAccount(int index, string name, string password)
    {
        modTypes.Player[index].Login = name;
        modTypes.Player[index].Password = password;

        for (var i = 1; i <= modTypes.MAX_CHARS; i++)
        {
            modTypes.ClearChar(index, i);
        }

        SavePlayer(index);
    }

    public static void AddChar(int index, string name, int sex, int classNum, int charNum)
    {
        if (string.IsNullOrWhiteSpace(modTypes.Player[index].Char[charNum].Name))
        {
            modTypes.Player[index].CharNum = charNum;

            modTypes.Player[index].Char[charNum].Name = name;
            modTypes.Player[index].Char[charNum].Sex = sex;
            modTypes.Player[index].Char[charNum].Class = classNum;
            modTypes.Player[index].Char[charNum].Sprite = modTypes.Class[classNum].Sprite;
            modTypes.Player[index].Char[charNum].Level = 1;

            modTypes.Player[index].Char[charNum].STR = modTypes.Class[classNum].STR;
            modTypes.Player[index].Char[charNum].DEF = modTypes.Class[classNum].DEF;
            modTypes.Player[index].Char[charNum].SPEED = modTypes.Class[classNum].SPEED;
            modTypes.Player[index].Char[charNum].MAGI = modTypes.Class[classNum].MAGI;

            modTypes.Player[index].Char[charNum].Map = START_MAP;
            modTypes.Player[index].Char[charNum].X = START_X;
            modTypes.Player[index].Char[charNum].Y = START_Y;

            modTypes.Player[index].Char[charNum].HP = modTypes.GetPlayerMaxHP(index);
            modTypes.Player[index].Char[charNum].MP = modTypes.GetPlayerMaxMP(index);
            modTypes.Player[index].Char[charNum].SP = modTypes.GetPlayerMaxSP(index);

            // Append name to file
            using (var writer = File.AppendText("Charlist.txt"))
            {
                writer.WriteLine(name);
            }

            SavePlayer(index);
        }
    }

    public static void DelChar(int index, int charNum)
    {
        DeleteName(modTypes.Player[index].Char[charNum].Name);
        modTypes.ClearChar(index, charNum);
        SavePlayer(index);
    }

    public static bool FindChar(string name)
    {
        using var streamReader = File.OpenText("Charlist.txt");

        while (!streamReader.EndOfStream)
        {
            var s = streamReader.ReadLine();
            if (s is null)
            {
                continue;
            }

            if (string.Equals(s, name, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    public static void SaveAllPlayersOnline()
    {
        for (var i = 1; i <= modTypes.MAX_PLAYERS; i++)
        {
            if (modServerTCP.IsPlaying(i))
            {
                SavePlayer(i);
            }
        }
    }

    public static void LoadClasses()
    {
        CheckClasses();

        var classesJson = File.ReadAllText("Classes.json");
        var classes = JsonSerializer.Deserialize<modTypes.ClassRec[]>(classesJson, JsonOptions);
        if (classes is null)
        {
            modTypes.Max_Classes = 0;
            return;
        }

        modTypes.Max_Classes = classes.Length;
        modTypes.Class = classes;
    }

    public static void SaveClasses()
    {
        var classesJson = JsonSerializer.Serialize(modTypes.Class, JsonOptions);

        File.WriteAllText("Classes.json", classesJson);
    }

    public static void CheckClasses()
    {
        if (!File.Exists("Classes.json"))
        {
            SaveClasses();
        }
    }

    public static void SaveItems()
    {
        for (var i = 1; i <= modTypes.MAX_ITEMS; i++)
        {
            SaveItem(i);
        }
    }

    public static void SaveItem(int itemNum)
    {
        var json = JsonSerializer.Serialize(modTypes.Item[itemNum]);
        var filename = $"Item{itemNum}.json";

        File.WriteAllText(Path.Combine("Data", "Items", filename), json);
    }

    public static void LoadItems()
    {
        var path = Path.Combine("Data", "Items");
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        for (var i = 1; i <= modTypes.MAX_ITEMS; i++)
        {
            var filename = $"Item{i}.json";
            var itemPath = Path.Combine(path, filename);
            if (!File.Exists(itemPath))
            {
                modTypes.Item[i] = new modTypes.ItemRec();
                continue;
            }

            var json = File.ReadAllText(itemPath);
            modTypes.Item[i] = JsonSerializer.Deserialize<modTypes.ItemRec>(json, JsonOptions);
        }
    }

    public static void SaveShops()
    {
        for (var i = 1; i <= modTypes.MAX_SHOPS; i++)
        {
            SaveShop(i);
        }
    }

    public static void SaveShop(int shopNum)
    {
        var json = JsonSerializer.Serialize(modTypes.Shop[shopNum]);
        var filename = $"Shop{shopNum}.json";

        File.WriteAllText(Path.Combine("Data", "Shops", filename), json);
    }

    public static void LoadShops()
    {
        var path = Path.Combine("Data", "Shops");
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        for (var i = 1; i <= modTypes.MAX_SHOPS; i++)
        {
            var filename = $"Shop{i}.json";
            var shopPath = Path.Combine(path, filename);
            if (!File.Exists(shopPath))
            {
                modTypes.Shop[i] = new modTypes.ShopRec();
                continue;
            }

            var json = File.ReadAllText(shopPath);
            modTypes.Shop[i] = JsonSerializer.Deserialize<modTypes.ShopRec>(json, JsonOptions);
        }
    }

    public static void SaveSpell(int spellNum)
    {
        var json = JsonSerializer.Serialize(modTypes.Spell[spellNum]);
        var filename = $"Spell{spellNum}.json";

        File.WriteAllText(Path.Combine("Data", "Spells", filename), json);
    }

    public static void SaveSpells()
    {
        for (var i = 1; i <= modTypes.MAX_SPELLS; i++)
        {
            SaveSpell(i);
        }
    }

    public static void LoadSpells()
    {
        var path = Path.Combine("Data", "Spells");
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        for (var i = 1; i <= modTypes.MAX_SPELLS; i++)
        {
            var filename = $"Spell{i}.json";
            var spellPath = Path.Combine(path, filename);
            if (!File.Exists(spellPath))
            {
                modTypes.Spell[i] = new modTypes.SpellRec();
                continue;
            }

            var json = File.ReadAllText(spellPath);
            modTypes.Spell[i] = JsonSerializer.Deserialize<modTypes.SpellRec>(json, JsonOptions);
        }
    }
    
    public static void SaveNpcs()
    {
        for (var i = 1; i <= modTypes.MAX_NPCS; i++)
        {
            SaveNpc(i);
        }
    }

    public static void SaveNpc(int npcNum)
    {
        var json = JsonSerializer.Serialize(modTypes.Npc[npcNum]);
        var filename = $"Npc{npcNum}.json";

        File.WriteAllText(Path.Combine("Data", "Npcs", filename), json);
    }

    public static void LoadNpcs()
    {
        var path = Path.Combine("Data", "Npcs");
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        for (var i = 1; i <= modTypes.MAX_NPCS; i++)
        {
            var filename = $"Npc{i}.json";
            var npcPath = Path.Combine(path, filename);
            if (!File.Exists(npcPath))
            {
                modTypes.Npc[i] = new modTypes.NpcRec();
                continue;
            }

            var json = File.ReadAllText(npcPath);
            modTypes.Npc[i] = JsonSerializer.Deserialize<modTypes.NpcRec>(json, JsonOptions);
        }
    }
    
    public static void SaveMap(int mapNum)
    {
        var path = Path.Combine("Data", "Maps", $"Map{mapNum}.map");
        var bytes = MessagePackSerializer.Serialize(modTypes.Map[mapNum]);

        File.WriteAllBytes(path, bytes);
    }

    public static void SaveMaps()
    {
        for (var i = 1; i <= modTypes.MAX_MAPS; i++)
        {
            SaveMap(i);
        }
    }

    public static void LoadMaps()
    {
        var path = Path.Combine("Data", "Maps");
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        for (var i = 1; i <= modTypes.MAX_MAPS; i++)
        {
            path = Path.Combine("Data", "Maps", $"Map{i}.map");
            if (!File.Exists(path))
            {
                modTypes.Map[i] = new modTypes.MapRec();
                continue;
            }

            var bytes = File.ReadAllBytes(path);

            modTypes.Map[i] = MessagePackSerializer.Deserialize<modTypes.MapRec>(bytes);
        }
    }

    public static void AddLog(string text, string fn)
    {
        if (!modGeneral.ServerLog)
        {
            return;
        }

        using var streamWriter = File.AppendText(fn);

        streamWriter.WriteLine(DateTime.Now.ToString("HH:mm:ss") + ": " + text);
        streamWriter.Close();
    }

    public static void BanIndex(int banPlayerIndex, int bannedByIndex)
    {
        // Cut off last portion of ip
        var ip = modTypes.GetPlayerIP(banPlayerIndex);
        
        var dot = ip.LastIndexOf('.');
        if (dot == -1)
        {
            ip = ip[..dot];
        }

        using (var streamWriter = File.AppendText("Banlist.txt"))
        {
            streamWriter.WriteLine($"{ip},{modTypes.GetPlayerName(bannedByIndex)}");
        }
        
        modServerTCP.GlobalMsg($"{modTypes.GetPlayerName(banPlayerIndex)} has been banned from {modTypes.GAME_NAME} by {modTypes.GetPlayerName(banPlayerIndex)}!", modText.White);
        AddLog($"{modTypes.GetPlayerName(bannedByIndex)} has banned {modTypes.GetPlayerName(banPlayerIndex)}.", ADMIN_LOG);
        modServerTCP.AlertMsg(banPlayerIndex, $"You have been banned by {modTypes.GetPlayerName(bannedByIndex)}!");
    }
 
    public static void DeleteName(string name)
    {
        if (!File.Exists("Charlist.txt"))
        {
            return;
        }

        File.Copy("Charlist.txt", "CharlistPrev.tmp");

        using var streamReader = File.OpenText("CharlistPrev.tmp");
        using var streamWriter = File.CreateText("Charlist.txt");

        while (streamReader.ReadLine() is { } line)
        {
            if (!string.Equals(line.Trim(), name.Trim(), StringComparison.CurrentCultureIgnoreCase))
            {
                streamWriter.WriteLine(line);
            }
        }

        File.Delete("CharlistPrev.tmp");
    }
}