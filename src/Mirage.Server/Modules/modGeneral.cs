using System.Runtime.InteropServices;

namespace Mirage.Modules;

public static class modGeneral
{
    public static int GetTickCount() => Environment.TickCount;

    // Version constants
    public const int CLIENT_MAJOR = 3;
    public const int CLIENT_MINOR = 0;
    public const int CLIENT_REVISION = 3;

    // Used for respawning items
    public static int SpawnSeconds;
    
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

    private static void CloseDoors(int mapNum, int tickCount)
    {
        if (tickCount <= modTypes.TempTile[mapNum].DoorTimer + 5000)
        {
            return;
        }

        for (var y = 0; y <= modTypes.MAX_MAPY; y++)
        {
            for (var x = 0; x <= modTypes.MAX_MAPX; x++)
            {
                if (modTypes.TempTile[mapNum].DoorOpen[x, y] == modTypes.NO)
                {
                    continue;
                }

                modTypes.TempTile[mapNum].DoorOpen[x, y] = modTypes.NO;

                modServerTCP.SendDataToMap(mapNum,
                    "MAPKEY" +
                    modTypes.SEP_CHAR + x +
                    modTypes.SEP_CHAR + y +
                    modTypes.SEP_CHAR + 0 +
                    modTypes.SEP_CHAR);
            }
        }
    }

    private static void UpdateNpc(int mapNum, int mapNpcNum, int tickCount)
    {
        var npcNum = modTypes.Map[mapNum].Npc[mapNpcNum];
        if (npcNum == 0)
        {
            return;
        }

        ref var npc = ref modTypes.Npc[npcNum];
        ref var mapNpc = ref modTypes.MapNpc[mapNum, mapNpcNum];

        //////////////////////////////////////
        // This is used for spawning an NPC //
        //////////////////////////////////////

        // Check if we are supposed to spawn an npc or not
        if (mapNpc.Num == 0)
        {
            if (tickCount > mapNpc.SpawnWait + npc.SpawnSecs * 1000)
            {
                modGameLogic.SpawnNpc(mapNpcNum, mapNum);
            }

            return;
        }

        /////////////////////////////////////////
        // This is used for ATTACKING ON SIGHT //
        /////////////////////////////////////////

        // If the npc is a attack on sight, search for a player on the map
        if (mapNpc.Target == 0 && npc.Behavior is modTypes.NPC_BEHAVIOR_ATTACKONSIGHT or modTypes.NPC_BEHAVIOR_GUARD)
        {
            for (var playerNum = 1; playerNum <= modTypes.MAX_PLAYERS; playerNum++)
            {
                if (!modServerTCP.IsPlaying(playerNum))
                {
                    continue;
                }

                if (modTypes.GetPlayerMap(playerNum) != mapNum ||
                    modTypes.GetPlayerAccess(playerNum) > modTypes.ADMIN_MONITER)
                {
                    continue;
                }

                var range = npc.Range;

                var distanceX = mapNpc.X - modTypes.GetPlayerX(playerNum);
                var distanceY = mapNpc.Y - modTypes.GetPlayerY(playerNum);

                // Make sure we get a positive value
                if (distanceX < 0) distanceX *= -1;
                if (distanceY < 0) distanceY *= -1;

                // Are they in range?  if so GET'M!
                if (distanceX > range || distanceY > range)
                {
                    continue;
                }

                if (npc.Behavior != modTypes.NPC_BEHAVIOR_ATTACKONSIGHT && modTypes.GetPlayerPK(playerNum) == modTypes.NO)
                {
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(npc.AttackSay))
                {
                    modServerTCP.PlayerMsg(playerNum, $"{npc.Name.Trim()} says, '{npc.AttackSay.Trim()}' to you.", modText.SayColor);
                }

                mapNpc.Target = playerNum;
            }
        }

        // /////////////////////////////////////////////
        // // This is used for NPC walking/targetting //
        // /////////////////////////////////////////////

        // Check to see if its time for the npc to walk
        if (npc.Behavior != modTypes.NPC_BEHAVIOR_SHOPKEEPER)
        {
            // Check to see if we are following a player or not
            if (mapNpc.Target == 0)
            {
                var i = Random.Shared.Next(4);
                if (i == 1)
                {
                    var dir = Random.Shared.Next(4);
                    if (modGameLogic.CanNpcMove(mapNum, mapNpcNum, dir))
                    {
                        modGameLogic.NpcMove(mapNum, mapNpcNum, dir, modTypes.MOVING_WALKING);
                    }
                }
            }
            else
            {
                var target = mapNpc.Target;
                
                // Check if the player is even playing, if so follow'm
                if (modServerTCP.IsPlaying(target) && modTypes.GetPlayerMap(target) == mapNum)
                {
                    var didWalk = false;
                    
                    var playerNum = Random.Shared.Next(5);
                    switch (playerNum)
                    {
                        case 0:
                            // Up
                            if (mapNpc.Y > modTypes.GetPlayerY(target) && !didWalk)
                            {
                                if (modGameLogic.CanNpcMove(mapNum, mapNpcNum, modTypes.DIR_UP))
                                {
                                    modGameLogic.NpcMove(mapNum, mapNpcNum, modTypes.DIR_UP, modTypes.MOVING_WALKING);
                                    didWalk = true;
                                }
                            }

                            // Down
                            if (mapNpc.Y < modTypes.GetPlayerY(target) && !didWalk)
                            {
                                if (modGameLogic.CanNpcMove(mapNum, mapNpcNum, modTypes.DIR_DOWN))
                                {
                                    modGameLogic.NpcMove(mapNum, mapNpcNum, modTypes.DIR_DOWN, modTypes.MOVING_WALKING);
                                    didWalk = true;
                                }
                            }

                            // Left
                            if (mapNpc.X > modTypes.GetPlayerX(target) && !didWalk)
                            {
                                if (modGameLogic.CanNpcMove(mapNum, mapNpcNum, modTypes.DIR_LEFT))
                                {
                                    modGameLogic.NpcMove(mapNum, mapNpcNum, modTypes.DIR_LEFT, modTypes.MOVING_WALKING);
                                    didWalk = true;
                                }
                            }

                            // Right
                            if (mapNpc.X < modTypes.GetPlayerX(target) && !didWalk)
                            {
                                if (modGameLogic.CanNpcMove(mapNum, mapNpcNum, modTypes.DIR_RIGHT))
                                {
                                    modGameLogic.NpcMove(mapNum, mapNpcNum, modTypes.DIR_RIGHT, modTypes.MOVING_WALKING);
                                    didWalk = true;
                                }
                            }

                            break;

                        case 1:
                            // Right
                            if (mapNpc.X < modTypes.GetPlayerX(target) && !didWalk)
                            {
                                if (modGameLogic.CanNpcMove(mapNum, mapNpcNum, modTypes.DIR_RIGHT))
                                {
                                    modGameLogic.NpcMove(mapNum, mapNpcNum, modTypes.DIR_RIGHT, modTypes.MOVING_WALKING);
                                    didWalk = true;
                                }
                            }

                            // Left
                            if (mapNpc.X > modTypes.GetPlayerX(target) && !didWalk)
                            {
                                if (modGameLogic.CanNpcMove(mapNum, mapNpcNum, modTypes.DIR_LEFT))
                                {
                                    modGameLogic.NpcMove(mapNum, mapNpcNum, modTypes.DIR_LEFT, modTypes.MOVING_WALKING);
                                    didWalk = true;
                                }
                            }

                            // Down
                            if (mapNpc.Y < modTypes.GetPlayerY(target) && !didWalk)
                            {
                                if (modGameLogic.CanNpcMove(mapNum, mapNpcNum, modTypes.DIR_DOWN))
                                {
                                    modGameLogic.NpcMove(mapNum, mapNpcNum, modTypes.DIR_DOWN, modTypes.MOVING_WALKING);
                                    didWalk = true;
                                }
                            }

                            // Up
                            if (mapNpc.Y > modTypes.GetPlayerY(target) && !didWalk)
                            {
                                if (modGameLogic.CanNpcMove(mapNum, mapNpcNum, modTypes.DIR_UP))
                                {
                                    modGameLogic.NpcMove(mapNum, mapNpcNum, modTypes.DIR_UP, modTypes.MOVING_WALKING);
                                    didWalk = true;
                                }
                            }

                            break;

                        case 2:
                            // Down
                            if (mapNpc.Y < modTypes.GetPlayerY(target) && !didWalk)
                            {
                                if (modGameLogic.CanNpcMove(mapNum, mapNpcNum, modTypes.DIR_DOWN))
                                {
                                    modGameLogic.NpcMove(mapNum, mapNpcNum, modTypes.DIR_DOWN, modTypes.MOVING_WALKING);
                                    didWalk = true;
                                }
                            }

                            // Up
                            if (mapNpc.Y > modTypes.GetPlayerY(target) && !didWalk)
                            {
                                if (modGameLogic.CanNpcMove(mapNum, mapNpcNum, modTypes.DIR_UP))
                                {
                                    modGameLogic.NpcMove(mapNum, mapNpcNum, modTypes.DIR_UP, modTypes.MOVING_WALKING);
                                    didWalk = true;
                                }
                            }

                            // Right
                            if (mapNpc.X < modTypes.GetPlayerX(target) && !didWalk)
                            {
                                if (modGameLogic.CanNpcMove(mapNum, mapNpcNum, modTypes.DIR_RIGHT))
                                {
                                    modGameLogic.NpcMove(mapNum, mapNpcNum, modTypes.DIR_RIGHT, modTypes.MOVING_WALKING);
                                    didWalk = true;
                                }
                            }

                            // Left
                            if (mapNpc.X > modTypes.GetPlayerX(target) && !didWalk)
                            {
                                if (modGameLogic.CanNpcMove(mapNum, mapNpcNum, modTypes.DIR_LEFT))
                                {
                                    modGameLogic.NpcMove(mapNum, mapNpcNum, modTypes.DIR_LEFT, modTypes.MOVING_WALKING);
                                    didWalk = true;
                                }
                            }

                            break;

                        case 3:
                            // Left
                            if (mapNpc.X > modTypes.GetPlayerX(target) && !didWalk)
                            {
                                if (modGameLogic.CanNpcMove(mapNum, mapNpcNum, modTypes.DIR_LEFT))
                                {
                                    modGameLogic.NpcMove(mapNum, mapNpcNum, modTypes.DIR_LEFT, modTypes.MOVING_WALKING);
                                    didWalk = true;
                                }
                            }

                            // Right
                            if (mapNpc.X < modTypes.GetPlayerX(target) && !didWalk)
                            {
                                if (modGameLogic.CanNpcMove(mapNum, mapNpcNum, modTypes.DIR_RIGHT))
                                {
                                    modGameLogic.NpcMove(mapNum, mapNpcNum, modTypes.DIR_RIGHT, modTypes.MOVING_WALKING);
                                    didWalk = true;
                                }
                            }

                            // Down
                            if (mapNpc.Y < modTypes.GetPlayerY(target) && !didWalk)
                            {
                                if (modGameLogic.CanNpcMove(mapNum, mapNpcNum, modTypes.DIR_DOWN))
                                {
                                    modGameLogic.NpcMove(mapNum, mapNpcNum, modTypes.DIR_DOWN, modTypes.MOVING_WALKING);
                                    didWalk = true;
                                }
                            }

                            // Up
                            if (mapNpc.Y > modTypes.GetPlayerY(target) && !didWalk)
                            {
                                if (modGameLogic.CanNpcMove(mapNum, mapNpcNum, modTypes.DIR_UP))
                                {
                                    modGameLogic.NpcMove(mapNum, mapNpcNum, modTypes.DIR_UP, modTypes.MOVING_WALKING);
                                    didWalk = true;
                                }
                            }

                            break;
                    }

                    // Check if we can't move and if player is behind something and if we can just switch dirs
                    if (!didWalk)
                    {
                        if (mapNpc.X - 1 == modTypes.GetPlayerX(target) && mapNpc.Y == modTypes.GetPlayerY(target))
                        {
                            if (mapNpc.Dir != modTypes.DIR_LEFT)
                            {
                                modGameLogic.NpcDir(mapNum, mapNpcNum, modTypes.DIR_LEFT);
                            }

                            didWalk = true;
                        }

                        if (mapNpc.X + 1 == modTypes.GetPlayerX(target) && mapNpc.Y == modTypes.GetPlayerY(target))
                        {
                            if (mapNpc.Dir != modTypes.DIR_RIGHT)
                            {
                                modGameLogic.NpcDir(mapNum, mapNpcNum, modTypes.DIR_RIGHT);
                            }

                            didWalk = true;
                        }

                        if (mapNpc.X == modTypes.GetPlayerX(target) && mapNpc.Y - 1 == modTypes.GetPlayerY(target))
                        {
                            if (mapNpc.Dir != modTypes.DIR_UP)
                            {
                                modGameLogic.NpcDir(mapNum, mapNpcNum, modTypes.DIR_UP);
                            }

                            didWalk = true;
                        }

                        if (mapNpc.X == modTypes.GetPlayerX(target) && mapNpc.Y + 1 == modTypes.GetPlayerY(target))
                        {
                            if (mapNpc.Dir != modTypes.DIR_DOWN)
                            {
                                modGameLogic.NpcDir(mapNum, mapNpcNum, modTypes.DIR_DOWN);
                            }

                            didWalk = true;
                        }

                        // We could not move so player must be behind something, walk randomly.
                        if (!didWalk)
                        {
                            var i = Random.Shared.Next(2);
                            if (i == 1)
                            {
                                var dir = Random.Shared.Next(4);
                                if (modGameLogic.CanNpcMove(mapNum, mapNpcNum, i))
                                {
                                    modGameLogic.NpcMove(mapNum, mapNpcNum, dir, modTypes.MOVING_WALKING);
                                }
                            }
                        }
                    }
                }
                else
                {
                    mapNpc.Target = 0;
                }
            }
        }

        // /////////////////////////////////////////////
        // // This is used for npcs to attack players //
        // /////////////////////////////////////////////
        
        // Check if the npc can attack the targeted player player
        if (mapNpc.Target > 0)
        {
            var target = mapNpc.Target;
            
            // Is the target playing and on the same map?
            if (modServerTCP.IsPlaying(target) && modTypes.GetPlayerMap(target) == mapNum)
            {
                // Can the npc attack the player?
                if (modGameLogic.CanNpcAttackPlayer(mapNpcNum, target))
                {
                    if (!modGameLogic.CanPlayerBlockHit(target))
                    {
                        var damage = npc.STR - modGameLogic.GetPlayerProtection(target);
                        if (damage > 0)
                        {
                            modGameLogic.NpcAttackPlayer(mapNpcNum, target, damage);
                        }
                        else
                        {
                            modServerTCP.PlayerMsg(target, $"The {npc.Name.Trim()}'s hit didn't even phase you!", modText.BrightBlue);
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
                mapNpc.Target = 0;
            }
        }

        ////////////////////////////////////////////
        // This is used for regenerating NPC's HP //
        ////////////////////////////////////////////
        if (modTypes.MapNpc[mapNpcNum, mapNum].Num > 0 && GetTickCount() > GiveNPCHPTimer + 10000)
        {
            if (mapNpc.HP > 0)
            {
                mapNpc.HP += modGameLogic.GetNpcHPRegen(npcNum);

                // Check if they have more then they should and if so just set it to max
                if (mapNpc.HP > modGameLogic.GetNpcMaxHP(npcNum))
                {
                    mapNpc.HP = modGameLogic.GetNpcMaxHP(npcNum);
                }
            }
        }
    }

    public static void GameAI()
    {
        var tickCount = GetTickCount();

        for (var mapNum = 1; mapNum <= modTypes.MAX_MAPS; mapNum++)
        {
            if (modTypes.PlayersOnMap[mapNum] == 0)
            {
                continue;
            }

            CloseDoors(mapNum, tickCount);

            for (var mapNpcNum = 1; mapNpcNum <= modTypes.MAX_MAP_NPCS; mapNpcNum++)
            {
                UpdateNpc(mapNum, mapNpcNum, tickCount);
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