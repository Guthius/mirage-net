using System.Runtime.InteropServices;

namespace Mirage.Modules;

public static partial class modGeneral
{
    [LibraryImport("kernel32.dll", EntryPoint = "GetTickCount")]
    internal static partial int GetTickCount();

    // Version constants
    public const int CLIENT_MAJOR = 3;
    public const int CLIENT_MINOR = 0;
    public const int CLIENT_REVISION = 3;

    // Used for respawning items
    public static int SpawnSeconds;

    // Used for weather effects
    public static int GameWeather = modTypes.WEATHER_NONE;
    public static int WeatherSeconds;
    public static int GameTime = modTypes.TIME_DAY;
    public static int TimeSeconds;

    // Used for closing key doors again
    public static int KeyTimer;

    // Used for gradually giving back players and npcs hp
    public static int GiveHPTimer;
    public static int GiveNPCHPTimer;

    // Used for logging
    public static bool ServerLog;

    public static void InitServer()
    {
        // Check if the maps directory is there, if its not make it
        if (!Directory.Exists("Maps"))
        {
            Directory.CreateDirectory("Maps");
        }

        // Check if the accounts directory is there, if its not make it
        if (!Directory.Exists("Accounts"))
        {
            Directory.CreateDirectory("Accounts");
        }

        // Init all the player sockets
        SetStatus("Initializing player array...");
        for (var i = 1; i <= modTypes.MAX_PLAYERS; i++)
        {
            modTypes.ClearPlayer(i);
        }

        SetStatus("Clearing temp tile fields...");
        modTypes.ClearTempTile();
        SetStatus("Clearing maps...");
        modTypes.ClearMaps();
        SetStatus("Clearing map items...");
        modTypes.ClearMapItems();
        SetStatus("Clearing map npcs...");
        modTypes.ClearMapNpcs();
        SetStatus("Clearing npcs...");
        modTypes.ClearNpcs();
        SetStatus("Clearing items...");
        modTypes.ClearItems();
        SetStatus("Clearing shops...");
        modTypes.ClearShops();
        SetStatus("Clearing spells...");
        modTypes.ClearSpells();
        SetStatus("Loading classes...");
        modDatabase.LoadClasses();
        SetStatus("Loading maps...");
        modDatabase.LoadMaps();
        SetStatus("Loading items...");
        modDatabase.LoadItems();
        SetStatus("Loading npcs...");
        modDatabase.LoadNpcs();
        SetStatus("Loading shops...");
        modDatabase.LoadShops();
        SetStatus("Loading spells...");
        modDatabase.LoadSpells();
        SetStatus("Spawning map items...");
        modGameLogic.SpawnAllMapsItems();
        SetStatus("Spawning map npcs...");
        modGameLogic.SpawnAllMapNpcs();

        // Check if the master charlist file exists for checking duplicate names, and if it doesnt make it
        var charListPath = Path.Combine("Charlist.txt");
        if (!File.Exists(charListPath))
        {
            File.Create(charListPath);
        }

        // Start listening
        modNetwork.Listen();

        SpawnSeconds = 0;

        Console.WriteLine($"Server started on port {modTypes.GAME_PORT}");

        // TODO: Start game AI timers...
    }

    public static void DestroyServer()
    {
        SetStatus("Shutting down server...");
        SetStatus("Saving players online...");

        modDatabase.SaveAllPlayersOnline();

        Application.Exit();
    }

    public static void SetStatus(string status)
    {
        Console.WriteLine(status);
    }

    public static void ServerLogic()
    {
        CheckGiveHP();
        GameAI();
    }

    public static void CheckSpawnMapItems()
    {
        // Used for map item respawning
        SpawnSeconds += 1;

        ///////////////////////////////////////////
        // This is used for respawning map items //
        ///////////////////////////////////////////
        if (SpawnSeconds >= 120)
        {
            // 2 minutes have passed
            for (var y = 1; y <= modTypes.MAX_MAPS; y++)
            {
                // Make sure no one is on the map when it respawns
                if (modTypes.PlayersOnMap[y] == 0)
                {
                    // Clear out unnecessary junk
                    for (var x = 1; x <= modTypes.MAX_MAP_ITEMS; x++)
                    {
                        modTypes.ClearMapItem(x, y);
                    }

                    // Spawn the items 
                    modGameLogic.SpawnMapItems(y);
                    modServerTCP.SendMapItemsToAll(y);
                }
            }

            SpawnSeconds = 0;
        }
    }

    public static void GameAI()
    {
        int i;
        
        //WeatherSeconds = WeatherSeconds + 1
        //TimeSeconds = TimeSeconds + 1

        // Lets change the weather if its time to
        if (WeatherSeconds >= 60)
        {
            i = Random.Shared.Next(3);
            if (i != GameWeather)
            {
                GameWeather = i;
                modServerTCP.SendWeatherToAll();
            }

            WeatherSeconds = 0;
        }

        // Check if we need to switch from day to night or night to day
        if (TimeSeconds >= 60)
        {
            if (GameTime == modTypes.TIME_DAY)
            {
                GameTime = modTypes.TIME_NIGHT;
            }
            else
            {
                GameTime = modTypes.TIME_DAY;
            }

            modServerTCP.SendTimeToAll();
            TimeSeconds = 0;
        }

        for (var y = 1; y <= modTypes.MAX_MAPS; y++)
        {
            if (modTypes.PlayersOnMap[y] != 0)
            {
                var tickCount = GetTickCount();

                ////////////////////////////////////
                // This is used for closing doors //
                ////////////////////////////////////
                if (tickCount > modTypes.TempTile[y].DoorTimer + 5000)
                {
                    for (var y1 = 0; y1 <= modTypes.MAX_MAPY; y1++)
                    {
                        for (var x1 = 0; x1 <= modTypes.MAX_MAPX; x1++)
                        {
                            if (modTypes.TempTile[y].DoorOpen[x1, y1] == modTypes.YES)
                            {
                                modTypes.TempTile[y].DoorOpen[x1, y1] = modTypes.NO;
                                modServerTCP.SendDataToMap(y,
                                    "MAPKEY" +
                                    modTypes.SEP_CHAR + x1 +
                                    modTypes.SEP_CHAR + y1 +
                                    modTypes.SEP_CHAR + 0 +
                                    modTypes.SEP_CHAR);
                            }
                        }
                    }
                }

                for (var x = 1; x <= modTypes.MAX_MAP_NPCS; x++)
                {
                    var npcNum = modTypes.MapNpc[x, y].Num;

                    /////////////////////////////////////////
                    // This is used for ATTACKING ON SIGHT //
                    /////////////////////////////////////////
                    // Make sure theres a npc with the map
                    if (modTypes.Map[y].Npc[x] > 0 && modTypes.MapNpc[y, x].Num > 0)
                    {
                        // If the npc is a attack on sight, search for a player on the map
                        if (modTypes.Npc[npcNum].Behavior == modTypes.NPC_BEHAVIOR_ATTACKONSIGHT || modTypes.Npc[npcNum].Behavior == modTypes.NPC_BEHAVIOR_GUARD)
                        {
                            for (i = 1; i <= modTypes.MAX_PLAYERS; i++)
                            {
                                if (!modServerTCP.IsPlaying(i))
                                {
                                    continue;
                                }

                                if (modTypes.GetPlayerMap(i) != y || modTypes.MapNpc[y, x].Target != 0 || modTypes.GetPlayerAccess(i) > modTypes.ADMIN_MONITER)
                                {
                                    continue;
                                }

                                var n = modTypes.Npc[npcNum].Range;

                                var distanceX = modTypes.MapNpc[y, x].X - modTypes.GetPlayerX(i);
                                var distanceY = modTypes.MapNpc[y, x].Y - modTypes.GetPlayerY(i);

                                // Make sure we get a positive value
                                if (distanceX < 0) distanceX *= -1;
                                if (distanceY < 0) distanceY *= -1;

                                // Are they in range?  if so GET'M!
                                if (distanceX > n || distanceY > n)
                                {
                                    continue;
                                }

                                if (modTypes.Npc[npcNum].Behavior == modTypes.NPC_BEHAVIOR_ATTACKONSIGHT || modTypes.GetPlayerPK(i) == modTypes.YES)
                                {
                                    if (!string.IsNullOrWhiteSpace(modTypes.Npc[npcNum].AttackSay))
                                    {
                                        modServerTCP.PlayerMsg(i, $"{modTypes.Npc[npcNum].Name.Trim()} says, '{modTypes.Npc[npcNum].AttackSay.Trim()}' to you.", modText.SayColor);
                                    }

                                    modTypes.MapNpc[y, x].Target = i;
                                }
                            }
                        }
                    }

                    // /////////////////////////////////////////////
                    // // This is used for NPC walking/targetting //
                    // /////////////////////////////////////////////
                    // Make sure theres a npc with the map
                    if (modTypes.Map[y].Npc[x] > 0 && modTypes.MapNpc[y, x].Num > 0)
                    {
                        var target = modTypes.MapNpc[y, x].Target;

                        // Check to see if its time for the npc to walk
                        if (modTypes.Npc[npcNum].Behavior != modTypes.NPC_BEHAVIOR_SHOPKEEPER)
                        {
                            // Check to see if we are following a player or not
                            if (target > 0)
                            {
                                // Check if the player is even playing, if so follow'm
                                if (modServerTCP.IsPlaying(target) && modTypes.GetPlayerMap(target) == y)
                                {
                                    var didWalk = false;

                                    i = Random.Shared.Next(5);
                                    switch (i)
                                    {
                                        case 0:
                                            // Up
                                            if (modTypes.MapNpc[y, x].Y > modTypes.GetPlayerY(target) && !didWalk)
                                            {
                                                if (modGameLogic.CanNpcMove(y, x, modTypes.DIR_UP))
                                                {
                                                    modGameLogic.NpcMove(y, x, modTypes.DIR_UP, modTypes.MOVING_WALKING);
                                                    didWalk = true;
                                                }
                                            }

                                            // Down
                                            if (modTypes.MapNpc[y, x].Y < modTypes.GetPlayerY(target) && !didWalk)
                                            {
                                                if (modGameLogic.CanNpcMove(y, x, modTypes.DIR_DOWN))
                                                {
                                                    modGameLogic.NpcMove(y, x, modTypes.DIR_DOWN, modTypes.MOVING_WALKING);
                                                    didWalk = true;
                                                }
                                            }

                                            // Left
                                            if (modTypes.MapNpc[y, x].X > modTypes.GetPlayerX(target) && !didWalk)
                                            {
                                                if (modGameLogic.CanNpcMove(y, x, modTypes.DIR_LEFT))
                                                {
                                                    modGameLogic.NpcMove(y, x, modTypes.DIR_LEFT, modTypes.MOVING_WALKING);
                                                    didWalk = true;
                                                }
                                            }

                                            // Right
                                            if (modTypes.MapNpc[y, x].X < modTypes.GetPlayerX(target) && !didWalk)
                                            {
                                                if (modGameLogic.CanNpcMove(y, x, modTypes.DIR_RIGHT))
                                                {
                                                    modGameLogic.NpcMove(y, x, modTypes.DIR_RIGHT, modTypes.MOVING_WALKING);
                                                    didWalk = true;
                                                }
                                            }

                                            break;

                                        case 1:
                                            // Right
                                            if (modTypes.MapNpc[y, x].X < modTypes.GetPlayerX(target) && !didWalk)
                                            {
                                                if (modGameLogic.CanNpcMove(y, x, modTypes.DIR_RIGHT))
                                                {
                                                    modGameLogic.NpcMove(y, x, modTypes.DIR_RIGHT, modTypes.MOVING_WALKING);
                                                    didWalk = true;
                                                }
                                            }

                                            // Left
                                            if (modTypes.MapNpc[y, x].X > modTypes.GetPlayerX(target) && !didWalk)
                                            {
                                                if (modGameLogic.CanNpcMove(y, x, modTypes.DIR_LEFT))
                                                {
                                                    modGameLogic.NpcMove(y, x, modTypes.DIR_LEFT, modTypes.MOVING_WALKING);
                                                    didWalk = true;
                                                }
                                            }

                                            // Down
                                            if (modTypes.MapNpc[y, x].Y < modTypes.GetPlayerY(target) && !didWalk)
                                            {
                                                if (modGameLogic.CanNpcMove(y, x, modTypes.DIR_DOWN))
                                                {
                                                    modGameLogic.NpcMove(y, x, modTypes.DIR_DOWN, modTypes.MOVING_WALKING);
                                                    didWalk = true;
                                                }
                                            }

                                            // Up
                                            if (modTypes.MapNpc[y, x].Y > modTypes.GetPlayerY(target) && !didWalk)
                                            {
                                                if (modGameLogic.CanNpcMove(y, x, modTypes.DIR_UP))
                                                {
                                                    modGameLogic.NpcMove(y, x, modTypes.DIR_UP, modTypes.MOVING_WALKING);
                                                    didWalk = true;
                                                }
                                            }

                                            break;

                                        case 2:
                                            // Down
                                            if (modTypes.MapNpc[y, x].Y < modTypes.GetPlayerY(target) && !didWalk)
                                            {
                                                if (modGameLogic.CanNpcMove(y, x, modTypes.DIR_DOWN))
                                                {
                                                    modGameLogic.NpcMove(y, x, modTypes.DIR_DOWN, modTypes.MOVING_WALKING);
                                                    didWalk = true;
                                                }
                                            }

                                            // Up
                                            if (modTypes.MapNpc[y, x].Y > modTypes.GetPlayerY(target) && !didWalk)
                                            {
                                                if (modGameLogic.CanNpcMove(y, x, modTypes.DIR_UP))
                                                {
                                                    modGameLogic.NpcMove(y, x, modTypes.DIR_UP, modTypes.MOVING_WALKING);
                                                    didWalk = true;
                                                }
                                            }

                                            // Right
                                            if (modTypes.MapNpc[y, x].X < modTypes.GetPlayerX(target) && !didWalk)
                                            {
                                                if (modGameLogic.CanNpcMove(y, x, modTypes.DIR_RIGHT))
                                                {
                                                    modGameLogic.NpcMove(y, x, modTypes.DIR_RIGHT, modTypes.MOVING_WALKING);
                                                    didWalk = true;
                                                }
                                            }

                                            // Left
                                            if (modTypes.MapNpc[y, x].X > modTypes.GetPlayerX(target) && !didWalk)
                                            {
                                                if (modGameLogic.CanNpcMove(y, x, modTypes.DIR_LEFT))
                                                {
                                                    modGameLogic.NpcMove(y, x, modTypes.DIR_LEFT, modTypes.MOVING_WALKING);
                                                    didWalk = true;
                                                }
                                            }

                                            break;

                                        case 3:
                                            // Left
                                            if (modTypes.MapNpc[y, x].X > modTypes.GetPlayerX(target) && !didWalk)
                                            {
                                                if (modGameLogic.CanNpcMove(y, x, modTypes.DIR_LEFT))
                                                {
                                                    modGameLogic.NpcMove(y, x, modTypes.DIR_LEFT, modTypes.MOVING_WALKING);
                                                    didWalk = true;
                                                }
                                            }

                                            // Right
                                            if (modTypes.MapNpc[y, x].X < modTypes.GetPlayerX(target) && !didWalk)
                                            {
                                                if (modGameLogic.CanNpcMove(y, x, modTypes.DIR_RIGHT))
                                                {
                                                    modGameLogic.NpcMove(y, x, modTypes.DIR_RIGHT, modTypes.MOVING_WALKING);
                                                    didWalk = true;
                                                }
                                            }

                                            // Down
                                            if (modTypes.MapNpc[y, x].Y < modTypes.GetPlayerY(target) && !didWalk)
                                            {
                                                if (modGameLogic.CanNpcMove(y, x, modTypes.DIR_DOWN))
                                                {
                                                    modGameLogic.NpcMove(y, x, modTypes.DIR_DOWN, modTypes.MOVING_WALKING);
                                                    didWalk = true;
                                                }
                                            }

                                            // Up
                                            if (modTypes.MapNpc[y, x].Y > modTypes.GetPlayerY(target) && !didWalk)
                                            {
                                                if (modGameLogic.CanNpcMove(y, x, modTypes.DIR_UP))
                                                {
                                                    modGameLogic.NpcMove(y, x, modTypes.DIR_UP, modTypes.MOVING_WALKING);
                                                    didWalk = true;
                                                }
                                            }

                                            break;
                                    }

                                    // Check if we can't move and if player is behind something and if we can just switch dirs
                                    if (!didWalk)
                                    {
                                        if (modTypes.MapNpc[y, x].X - 1 == modTypes.GetPlayerX(target) && modTypes.MapNpc[y, x].Y == modTypes.GetPlayerY(target))
                                        {
                                            if (modTypes.MapNpc[y, x].Dir != modTypes.DIR_LEFT)
                                            {
                                                modGameLogic.NpcDir(y, x, modTypes.DIR_LEFT);
                                            }

                                            didWalk = true;
                                        }

                                        if (modTypes.MapNpc[y, x].X + 1 == modTypes.GetPlayerX(target) && modTypes.MapNpc[y, x].Y == modTypes.GetPlayerY(target))
                                        {
                                            if (modTypes.MapNpc[y, x].Dir != modTypes.DIR_RIGHT)
                                            {
                                                modGameLogic.NpcDir(y, x, modTypes.DIR_RIGHT);
                                            }

                                            didWalk = true;
                                        }

                                        if (modTypes.MapNpc[y, x].X == modTypes.GetPlayerX(target) && modTypes.MapNpc[y, x].Y - 1 == modTypes.GetPlayerY(target))
                                        {
                                            if (modTypes.MapNpc[y, x].Dir != modTypes.DIR_UP)
                                            {
                                                modGameLogic.NpcDir(y, x, modTypes.DIR_UP);
                                            }

                                            didWalk = true;
                                        }

                                        if (modTypes.MapNpc[y, x].X == modTypes.GetPlayerX(target) && modTypes.MapNpc[y, x].Y + 1 == modTypes.GetPlayerY(target))
                                        {
                                            if (modTypes.MapNpc[y, x].Dir != modTypes.DIR_DOWN)
                                            {
                                                modGameLogic.NpcDir(y, x, modTypes.DIR_DOWN);
                                            }

                                            didWalk = true;
                                        }

                                        // We could not move so player must be behind something, walk randomly.
                                        if (!didWalk)
                                        {
                                            i = Random.Shared.Next(2);
                                            if (i == 1)
                                            {
                                                i = Random.Shared.Next(4);
                                                if (modGameLogic.CanNpcMove(y, x, i))
                                                {
                                                    modGameLogic.NpcMove(y, x, i, modTypes.MOVING_WALKING);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    modTypes.MapNpc[y, x].Target = 0;
                                }
                            }
                            else
                            {
                                i = Random.Shared.Next(4);
                                if (i == 1)
                                {
                                    i = Random.Shared.Next(4);
                                    if (modGameLogic.CanNpcMove(y, x, i))
                                    {
                                        modGameLogic.NpcMove(y, x, i, modTypes.MOVING_WALKING);
                                    }
                                }
                            }
                        }
                    }

                    // /////////////////////////////////////////////
                    // // This is used for npcs to attack players //
                    // /////////////////////////////////////////////
                    // Make sure theres a npc with the map
                    if (modTypes.Map[y].Npc[x] > 0 && modTypes.MapNpc[y, x].Num > 0)
                    {
                        var target = modTypes.MapNpc[y, x].Target;

                        // Check if the npc can attack the targeted player player
                        if (target > 0)
                        {
                            // Is the target playing and on the same map?
                            if (modServerTCP.IsPlaying(target) && modTypes.GetPlayerMap(target) == y)
                            {
                                // Can the npc attack the player?
                                if (modGameLogic.CanNpcAttackPlayer(x, target))
                                {
                                    if (!modGameLogic.CanPlayerBlockHit(target))
                                    {
                                        var damage = modTypes.Npc[npcNum].STR - modGameLogic.GetPlayerProtection(target);
                                        if (damage > 0)
                                        {
                                            modGameLogic.NpcAttackPlayer(x, target, damage);
                                        }
                                        else
                                        {
                                            modServerTCP.PlayerMsg(target, $"The {modTypes.Npc[npcNum].Name.Trim()}'s hit didn't even phase you!", modText.BrightBlue);
                                        }
                                    }
                                    else
                                    {
                                        modServerTCP.PlayerMsg(target, $"Your {modTypes.Item[modTypes.GetPlayerInvItemNum(target, modTypes.GetPlayerShieldSlot(target))].Name.Trim()} blocks the {modTypes.Npc[npcNum].Name.Trim()}'s hit!", modText.BrightCyan);
                                    }
                                }
                            }
                            else
                            {
                                // Player left map or game, set target to 0
                                modTypes.MapNpc[y, x].Target = 0;
                            }
                        }
                    }

                    ////////////////////////////////////////////
                    // This is used for regenerating NPC's HP //
                    ////////////////////////////////////////////
                    if (modTypes.MapNpc[y, x].Num > 0 && GetTickCount() > GiveNPCHPTimer + 10000)
                    {
                        if (modTypes.MapNpc[y, x].HP > 0)
                        {
                            modTypes.MapNpc[y, x].HP += modGameLogic.GetNpcHPRegen(npcNum);

                            // Check if they have more then they should and if so just set it to max
                            if (modTypes.MapNpc[y, x].HP > modGameLogic.GetNpcMaxHP(npcNum))
                            {
                                modTypes.MapNpc[y, x].HP = modGameLogic.GetNpcMaxHP(npcNum);
                            }
                        }
                    }

                    //////////////////////////////////////
                    // This is used for spawning an NPC //
                    //////////////////////////////////////
                    // Check if we are supposed to spawn an npc or not
                    if (modTypes.MapNpc[y, x].Num == 0 && modTypes.Map[y].Npc[x] > 0)
                    {
                        if (tickCount > modTypes.MapNpc[y, x].SpawnWait + (modTypes.Npc[modTypes.Map[y].Npc[x]].SpawnSecs * 1000))
                        {
                            modGameLogic.SpawnNpc(x, y);
                        }
                    }
                }
            }
        }

        // Make sure we reset the timer for npc hp regeneration
        if (GetTickCount() > GiveNPCHPTimer + 10000)
        {
            GiveNPCHPTimer = GetTickCount();
        }

        // Make sure we reset the timer for door closing
        if (GetTickCount() > KeyTimer + 15000)
        {
            KeyTimer = GetTickCount();
        }
    }

    public static void CheckGiveHP()
    {
        if (GetTickCount() <= GiveHPTimer + 10000)
        {
            return;
        }

        for (var i = 1; i <= modTypes.MAX_PLAYERS; i++)
        {
            if (modServerTCP.IsPlaying(i))
            {
                modTypes.SetPlayerHP(i, modTypes.GetPlayerHP(i) + modGameLogic.GetPlayerHPRegen(i));
                modServerTCP.SendHP(i);
                modTypes.SetPlayerMP(i, modTypes.GetPlayerMP(i) + modGameLogic.GetPlayerMPRegen(i));
                modServerTCP.SendMP(i);
                modTypes.SetPlayerSP(i, modTypes.GetPlayerSP(i) + modGameLogic.GetPlayerSPRegen(i));
                modServerTCP.SendSP(i);
            }
        }
    }

    private static int _minPassed;

    public static void PlayerSaveTimer()
    {
        _minPassed += 1;

        if (_minPassed >= 10)
        {
            if (modGameLogic.TotalOnlinePlayers() > 0)
            {
                Console.WriteLine("Saving all online players...");
                modServerTCP.GlobalMsg("Saving all online players...", modText.Pink);
                for (var i = 1; i <= modTypes.MAX_PLAYERS; i++)
                {
                    if (modServerTCP.IsPlaying(i))
                    {
                        modDatabase.SavePlayer(i);
                    }
                }
            }

            _minPassed = 0;
        }
    }
}