namespace Mirage.Modules;

public static class modHandleData
{
    public static void HandleData(int index, string data)
    {
        // Handle Data
        var parse = data.Split(modTypes.SEP_CHAR);
        if (parse.Length == 0)
        {
            return;
        }

        // :::::::::::::::::::::::::::::::::::::::::::::::
        // :: Requesting classes for making a character ::
        // :::::::::::::::::::::::::::::::::::::::::::::::
        if (parse[0].Equals("getclasses", StringComparison.CurrentCultureIgnoreCase))
        {
            if (!modServerTCP.IsPlaying(index))
            {
                modServerTCP.SendNewCharClasses(index);
            }

            return;
        }

        // ::::::::::::::::::::::::
        // :: New account packet ::
        // ::::::::::::::::::::::::
        if (parse[0].Equals("newaccount", StringComparison.CurrentCultureIgnoreCase))
        {
            if (!modServerTCP.IsPlaying(index) && !modServerTCP.IsLoggedIn(index))
            {
                // Get the data
                var name = parse[1].Trim();
                var password = parse[2].Trim();

                // Prevent hacking
                if (name.Length < 3 || password.Length < 3)
                {
                    modServerTCP.AlertMsg(index, "Your name and password must be at least three characters in length");
                    return;
                }

                // Prevent hacking
                foreach (var n in name)
                {
                    if ((n >= 65 && n <= 90) || (n >= 97 && n <= 122) || n == 95 || n == 32 || (n >= 48 && n <= 57))
                    {
                    }
                    else
                    {
                        modServerTCP.AlertMsg(index, "Invalid name, only letters, numbers, spaces, and _ allowed in names.");
                        return;
                    }
                }

                // Check to see if account already exists
                if (!modDatabase.AccountExist(name))
                {
                    modDatabase.AddAccount(index, name, password);
                    Console.WriteLine($"Account {name} has been created.");
                    modServerTCP.AlertMsg(index, "Your account has been created!");
                }
                else
                {
                    modServerTCP.AlertMsg(index, "Sorry, that account name is already taken!");
                }
            }

            return;
        }

        // :::::::::::::::::::::::::::
        // :: Delete account packet ::
        // :::::::::::::::::::::::::::
        if (parse[0].Equals("delaccount", StringComparison.CurrentCultureIgnoreCase))
        {
            if (!modServerTCP.IsPlaying(index) && !modServerTCP.IsLoggedIn(index))
            {
                // Get the data
                var name = parse[1].Trim();
                var password = parse[2].Trim();

                // Prevent hacking
                if (name.Length < 3 || password.Length < 3)
                {
                    modServerTCP.AlertMsg(index, "The name and password must be at least three characters in length");
                    return;
                }

                if (!modDatabase.AccountExist(name))
                {
                    modServerTCP.AlertMsg(index, "That account name does not exist.");
                    return;
                }

                if (!modDatabase.PasswordOk(name, password))
                {
                    modServerTCP.AlertMsg(index, "Incorrect password.");
                    return;
                }

                // Delete names from master name file
                modDatabase.LoadPlayer(index, name);
                for (var i = 1; i <= modTypes.MAX_CHARS; i++)
                {
                    if (!string.IsNullOrWhiteSpace(modTypes.Player[index].Char[i].Name))
                    {
                        modDatabase.DeleteName(modTypes.Player[index].Char[i].Name);
                    }
                }

                modTypes.ClearPlayer(index);

                // Everything went ok
                modDatabase.DeleteAccount(name);
                Console.WriteLine($"Account {name} has been deleted.");
                modServerTCP.AlertMsg(index, "Your account has been deleted!");
            }

            return;
        }

        // ::::::::::::::::::
        // :: Login packet ::
        // ::::::::::::::::::
        if (parse[0].Equals("login", StringComparison.CurrentCultureIgnoreCase))
        {
            if (!modServerTCP.IsPlaying(index) && !modServerTCP.IsLoggedIn(index))
            {
                // Get the data
                var name = parse[1].Trim();
                var password = parse[2].Trim();

                // Check versions
                if (int.Parse(parse[3]) != modGeneral.CLIENT_MAJOR || int.Parse(parse[4]) != modGeneral.CLIENT_MINOR || int.Parse(parse[5]) != modGeneral.CLIENT_REVISION)
                {
                    modServerTCP.AlertMsg(index, "Version outdated, please visit https://github.com/Guthius/mirage-net/");
                    return;
                }

                if (name.Length < 3 || password.Length < 3)
                {
                    modServerTCP.AlertMsg(index, "Your name and password must be at least three characters in length");
                    return;
                }

                if (!modDatabase.AccountExist(name))
                {
                    modServerTCP.AlertMsg(index, "That account name does not exist.");
                    return;
                }

                if (!modDatabase.PasswordOk(name, password))
                {
                    modServerTCP.AlertMsg(index, "Incorrect password.");
                    return;
                }

                if (modServerTCP.IsMultiAccounts(name))
                {
                    modServerTCP.AlertMsg(index, "Multiple account logins is not authorized.");
                    return;
                }

                // Everything went ok

                // Load the player
                modDatabase.LoadPlayer(index, name);
                modServerTCP.SendChars(index);

                // Show the player up on the socket status
                Console.WriteLine($"{modTypes.GetPlayerLogin(index)} has logged in from {modTypes.GetPlayerIP(index)}.");
            }

            return;
        }

        // ::::::::::::::::::::::::::
        // :: Add character packet ::
        // ::::::::::::::::::::::::::
        if (parse[0].Equals("addchar", StringComparison.CurrentCultureIgnoreCase))
        {
            if (!modServerTCP.IsPlaying(index))
            {
                var name = parse[1].Trim();
                var sex = int.Parse(parse[2]);
                var classNum = int.Parse(parse[3]);
                var charNum = int.Parse(parse[4]);

                // Prevent hacking
                if (name.Length < 3)
                {
                    modServerTCP.AlertMsg(index, "Character name must be at least three characters in length.");
                    return;
                }

                // Prevent hacking
                foreach (var n in name)
                {
                    if ((n >= 65 && n <= 90) || (n >= 97 && n <= 122) || n == 95 || n == 32 || (n >= 48 && n <= 57))
                    {
                    }
                    else
                    {
                        modServerTCP.AlertMsg(index, "Invalid name, only letters, numbers, spaces, and _ allowed in names.");
                        return;
                    }
                }

                // Prevent hacking
                if (charNum is < 1 or > modTypes.MAX_CHARS)
                {
                    modServerTCP.HackingAttempt(index, "Invalid CharNum");
                    return;
                }

                // Prevent hacking
                if (sex is < modTypes.SEX_MALE or > modTypes.SEX_FEMALE)
                {
                    modServerTCP.HackingAttempt(index, "Invalid Sex (dont laugh)");
                    return;
                }

                // Prevent hacking
                if (classNum < 0 || classNum > modTypes.Max_Classes)
                {
                    modServerTCP.HackingAttempt(index, "Invalid Class");
                    return;
                }

                // Check if char already exists in slot
                if (modDatabase.CharExist(index, charNum))
                {
                    modServerTCP.AlertMsg(index, "This character already exists.");
                    return;
                }

                // Check if name is already in use
                if (modDatabase.FindChar(name))
                {
                    modServerTCP.AlertMsg(index, "Sorry, but that name is in use!");
                    return;
                }

                // Everything went ok, add the character
                modDatabase.AddChar(index, name, sex, classNum, charNum);
                modDatabase.SavePlayer(index);
                Console.WriteLine($"Character {name} added to {modTypes.GetPlayerLogin(index)}'s account.");
                modServerTCP.AlertMsg(index, "Character has been created!");
            }

            return;
        }

        // :::::::::::::::::::::::::::::::
        // :: Deleting character packet ::
        // :::::::::::::::::::::::::::::::
        if (parse[0].Equals("delchar", StringComparison.CurrentCultureIgnoreCase))
        {
            if (!modServerTCP.IsPlaying(index))
            {
                var charNum = int.Parse(parse[1]);

                // Prevent hacking
                if (charNum is < 1 or > modTypes.MAX_CHARS)
                {
                    modServerTCP.HackingAttempt(index, "Invalid CharNum");
                    return;
                }

                modDatabase.DelChar(index, charNum);
                Console.WriteLine($"Character deleted on {modTypes.GetPlayerLogin(index)}'s account.");
                modServerTCP.AlertMsg(index, "Character has been deleted!");
            }

            return;
        }

        // ::::::::::::::::::::::::::::
        // :: Using character packet ::
        // ::::::::::::::::::::::::::::
        if (parse[0].Equals("usechar", StringComparison.CurrentCultureIgnoreCase))
        {
            if (!modServerTCP.IsPlaying(index))
            {
                var charNum = int.Parse(parse[1]);

                // Prevent hacking
                if (charNum is < 1 or > modTypes.MAX_CHARS)
                {
                    modServerTCP.HackingAttempt(index, "Invalid CharNum");
                    return;
                }

                // Check to make sure the character exists and if so, set it as its current char
                if (modDatabase.CharExist(index, charNum))
                {
                    modTypes.Player[index].CharNum = charNum;
                    modGameLogic.JoinGame(index);

                    Console.WriteLine($"{modTypes.GetPlayerLogin(index)}/{modTypes.GetPlayerName(index)} has began playing  {modTypes.GAME_NAME}.");
                }
                else
                {
                    modServerTCP.AlertMsg(index, "Character does not exist!");
                }
            }

            return;
        }

        // ::::::::::::::::::::
        // :: Social packets ::
        // ::::::::::::::::::::
        if (parse[0].Equals("saymsg", StringComparison.CurrentCultureIgnoreCase))
        {
            var msg = parse[1];

            // Prevent hacking
            foreach (var ch in msg)
            {
                if (ch < 32 || ch > 126)
                {
                    modServerTCP.HackingAttempt(index, "Say Text Modification");
                    return;
                }
            }

            Console.WriteLine($"Map #{modTypes.GetPlayerMap(index)}: {modTypes.GetPlayerName(index)} says '{msg}'");
            modServerTCP.MapMsg(modTypes.GetPlayerMap(index), $"{modTypes.GetPlayerName(index)} says '{msg}'", modText.SayColor);
            return;
        }

        if (parse[0].Equals("emotemsg", StringComparison.CurrentCultureIgnoreCase))
        {
            var msg = parse[1];

            // Prevent hacking
            foreach (var ch in msg)
            {
                if (ch < 32 || ch > 126)
                {
                    modServerTCP.HackingAttempt(index, "Emote Text Modification");
                    return;
                }
            }

            Console.WriteLine($"Map #{modTypes.GetPlayerMap(index)}: {modTypes.GetPlayerName(index)} {msg}");
            modServerTCP.MapMsg(modTypes.GetPlayerMap(index), $"{modTypes.GetPlayerName(index)} {msg}", modText.EmoteColor);
            return;
        }

        if (parse[0].Equals("broadcastmsg", StringComparison.CurrentCultureIgnoreCase))
        {
            var msg = parse[1];

            // Prevent hacking
            foreach (var ch in msg)
            {
                if (ch < 32 || ch > 126)
                {
                    modServerTCP.HackingAttempt(index, "Broadcast Text Modification");
                    return;
                }
            }

            var s = $"{modTypes.GetPlayerName(index)}: {msg}";
            modServerTCP.GlobalMsg(s, modText.BroadcastColor);
            Console.WriteLine(s);
            return;
        }

        if (parse[0].Equals("globalmsg", StringComparison.CurrentCultureIgnoreCase))
        {
            var msg = parse[1];

            // Prevent hacking
            foreach (var ch in msg)
            {
                if (ch < 32 || ch > 126)
                {
                    modServerTCP.HackingAttempt(index, "Global Text Modification");
                    return;
                }
            }

            if (modTypes.GetPlayerAccess(index) > 0)
            {
                var s = $"(global) {modTypes.GetPlayerName(index)}: {msg}";
                modServerTCP.GlobalMsg(s, modText.GlobalColor);
                Console.WriteLine(s);
            }

            return;
        }

        if (parse[0].Equals("adminmsg", StringComparison.CurrentCultureIgnoreCase))
        {
            var msg = parse[1];

            // Prevent hacking
            foreach (var ch in msg)
            {
                if (ch < 32 || ch > 126)
                {
                    modServerTCP.HackingAttempt(index, "Admin Text Modification");
                    return;
                }
            }

            if (modTypes.GetPlayerAccess(index) > 0)
            {
                var s = $"(admin {modTypes.GetPlayerName(index)}) {msg}";
                modServerTCP.AdminMsg(s, modText.AdminColor);
                Console.WriteLine(s);
            }

            return;
        }

        if (parse[0].Equals("playermsg", StringComparison.CurrentCultureIgnoreCase))
        {
            var msgTo = modGameLogic.FindPlayer(parse[1]);
            var msg = parse[2];

            // Prevent hacking
            foreach (var ch in msg)
            {
                if (ch < 32 || ch > 126)
                {
                    modServerTCP.HackingAttempt(index, "Player Msg Text Modification");
                    return;
                }
            }

            // Check if they are trying to talk to themselves
            if (msgTo != index)
            {
                if (msgTo > 0)
                {
                    Console.WriteLine($"{modTypes.GetPlayerName(index)} tells {modTypes.GetPlayerName(msgTo)}, '{msg}'");
                    modServerTCP.PlayerMsg(msgTo, $"{modTypes.GetPlayerName(index)} tells you, '{msg}'", modText.TellColor);
                    modServerTCP.PlayerMsg(index, $"You tell {modTypes.GetPlayerName(msgTo)}, '{msg}'", modText.TellColor);
                }
                else
                {
                    modServerTCP.PlayerMsg(index, "Player is not online.", modText.White);
                }
            }
            else
            {
                Console.WriteLine($"Map #{modTypes.GetPlayerMap(index)}: {modTypes.GetPlayerName(index)} begins to mumble to himself, what a wierdo...");
                modServerTCP.MapMsg(modTypes.GetPlayerMap(index), $"{modTypes.GetPlayerName(index)} begins to mumble to himself, what a wierdo...", modText.Green);
            }

            return;
        }

        // :::::::::::::::::::::::::::::
        // :: Moving character packet ::
        // :::::::::::::::::::::::::::::
        if (parse[0].Equals("playermove", StringComparison.CurrentCultureIgnoreCase) && modTypes.Player[index].GettingMap == modTypes.NO)
        {
            var dir = int.Parse(parse[1]);
            var movement = int.Parse(parse[2]);

            // Prevent hacking
            if (dir is < modTypes.DIR_UP or > modTypes.DIR_RIGHT)
            {
                modServerTCP.HackingAttempt(index, "Invalid Direction");
                return;
            }

            // Prevent hacking
            if (movement is < 1 or > 2)
            {
                modServerTCP.HackingAttempt(index, "Invalid Movement");
                return;
            }

            // Prevent player from moving if they have casted a spell
            if (modTypes.Player[index].CastedSpell == modTypes.YES)
            {
                // Check if they have already casted a spell, and if so we can't let them move
                if (modGeneral.GetTickCount() > modTypes.Player[index].AttackTimer + 1000)
                {
                    modTypes.Player[index].CastedSpell = modTypes.NO;
                }
                else
                {
                    modServerTCP.SendPlayerXY(index);
                    return;
                }
            }

            modGameLogic.PlayerMove(index, dir, movement);
            return;
        }

        // :::::::::::::::::::::::::::::
        // :: Moving character packet ::
        // :::::::::::::::::::::::::::::
        if (parse[0].Equals("playerdir", StringComparison.CurrentCultureIgnoreCase) && modTypes.Player[index].GettingMap == modTypes.NO)
        {
            var dir = int.Parse(parse[1]);

            // Prevent hacking
            if (dir is < modTypes.DIR_UP or > modTypes.DIR_RIGHT)
            {
                modServerTCP.HackingAttempt(index, "Invalid Direction");
                return;
            }

            modTypes.SetPlayerDir(index, dir);
            modServerTCP.SendDataToMapBut(index, modTypes.GetPlayerMap(index),
                "PLAYERDIR" +
                modTypes.SEP_CHAR + index +
                modTypes.SEP_CHAR + modTypes.GetPlayerDir(index) +
                modTypes.SEP_CHAR);

            return;
        }

        // :::::::::::::::::::::
        // :: Use item packet ::
        // :::::::::::::::::::::
        if (parse[0].Equals("useitem", StringComparison.CurrentCultureIgnoreCase))
        {
            var invNum = int.Parse(parse[1]);
            var charNum = modTypes.Player[index].CharNum;

            // Prevent hacking
            if (invNum is < 1 or > modTypes.MAX_INV)
            {
                modServerTCP.HackingAttempt(index, "Invalid InvNum");
                return;
            }

            // Prevent hacking
            if (charNum is < 1 or > modTypes.MAX_CHARS)
            {
                modServerTCP.HackingAttempt(index, "Invalid CharNum");
                return;
            }

            var itemNum = modTypes.GetPlayerInvItemNum(index, invNum);
            var n = modTypes.Item[itemNum].Data2;

            if (itemNum is > 0 and <= modTypes.MAX_ITEMS)
            {
                // Find out what kind of item it is
                switch (modTypes.Item[itemNum].Type)
                {
                    case modTypes.ITEM_TYPE_ARMOR:
                        if (invNum != modTypes.GetPlayerArmorSlot(index))
                        {
                            if (modTypes.GetPlayerDEF(index) < n)
                            {
                                modServerTCP.PlayerMsg(index, $"Your defense is to low to wear this armor!  Required DEF ({n * 2})", modText.BrightRed);
                                return;
                            }

                            modTypes.SetPlayerArmorSlot(index, invNum);
                        }
                        else
                        {
                            modTypes.SetPlayerArmorSlot(index, 0);
                        }

                        modServerTCP.SendWornEquipment(index);
                        break;

                    case modTypes.ITEM_TYPE_WEAPON:
                        if (invNum != modTypes.GetPlayerWeaponSlot(index))
                        {
                            if (modTypes.GetPlayerSTR(index) < n)
                            {
                                modServerTCP.PlayerMsg(index, $"Your strength is to low to wear this armor!  Required STR ({n * 2})", modText.BrightRed);
                                return;
                            }

                            modTypes.SetPlayerWeaponSlot(index, invNum);
                        }
                        else
                        {
                            modTypes.SetPlayerWeaponSlot(index, 0);
                        }

                        modServerTCP.SendWornEquipment(index);
                        break;

                    case modTypes.ITEM_TYPE_HELMET:
                        if (invNum != modTypes.GetPlayerHelmetSlot(index))
                        {
                            if (modTypes.GetPlayerSPEED(index) < n)
                            {
                                modServerTCP.PlayerMsg(index, $"Your speed coordination is to low to wear this helmet!  Required SPEED ({n * 2})", modText.BrightRed);
                                return;
                            }

                            modTypes.SetPlayerHelmetSlot(index, invNum);
                        }
                        else
                        {
                            modTypes.SetPlayerHelmetSlot(index, 0);
                        }

                        modServerTCP.SendWornEquipment(index);
                        break;

                    case modTypes.ITEM_TYPE_SHIELD:
                        if (invNum != modTypes.GetPlayerShieldSlot(index))
                        {
                            modTypes.SetPlayerShieldSlot(index, invNum);
                        }
                        else
                        {
                            modTypes.SetPlayerShieldSlot(index, 0);
                        }

                        modServerTCP.SendWornEquipment(index);
                        break;

                    case modTypes.ITEM_TYPE_POTIONADDHP:
                        modTypes.SetPlayerHP(index, modTypes.GetPlayerHP(index) + modTypes.Item[modTypes.Player[index].Char[charNum].Inv[invNum].Num].Data1);
                        modGameLogic.TakeItem(index, modTypes.Player[index].Char[charNum].Inv[invNum].Num, 0);
                        modServerTCP.SendHP(index);
                        break;

                    case modTypes.ITEM_TYPE_POTIONADDMP:
                        modTypes.SetPlayerMP(index, modTypes.GetPlayerMP(index) + modTypes.Item[modTypes.Player[index].Char[charNum].Inv[invNum].Num].Data1);
                        modGameLogic.TakeItem(index, modTypes.Player[index].Char[charNum].Inv[invNum].Num, 0);
                        modServerTCP.SendMP(index);
                        break;

                    case modTypes.ITEM_TYPE_POTIONADDSP:
                        modTypes.SetPlayerSP(index, modTypes.GetPlayerSP(index) + modTypes.Item[modTypes.Player[index].Char[charNum].Inv[invNum].Num].Data1);
                        modGameLogic.TakeItem(index, modTypes.Player[index].Char[charNum].Inv[invNum].Num, 0);
                        modServerTCP.SendSP(index);
                        break;

                    case modTypes.ITEM_TYPE_POTIONSUBHP:
                        modTypes.SetPlayerHP(index, modTypes.GetPlayerHP(index) - modTypes.Item[modTypes.Player[index].Char[charNum].Inv[invNum].Num].Data1);
                        modGameLogic.TakeItem(index, modTypes.Player[index].Char[charNum].Inv[invNum].Num, 0);
                        modServerTCP.SendHP(index);
                        break;

                    case modTypes.ITEM_TYPE_POTIONSUBMP:
                        modTypes.SetPlayerMP(index, modTypes.GetPlayerMP(index) - modTypes.Item[modTypes.Player[index].Char[charNum].Inv[invNum].Num].Data1);
                        modGameLogic.TakeItem(index, modTypes.Player[index].Char[charNum].Inv[invNum].Num, 0);
                        modServerTCP.SendMP(index);
                        break;

                    case modTypes.ITEM_TYPE_POTIONSUBSP:
                        modTypes.SetPlayerSP(index, modTypes.GetPlayerSP(index) - modTypes.Item[modTypes.Player[index].Char[charNum].Inv[invNum].Num].Data1);
                        modGameLogic.TakeItem(index, modTypes.Player[index].Char[charNum].Inv[invNum].Num, 0);
                        modServerTCP.SendSP(index);
                        break;

                    case modTypes.ITEM_TYPE_KEY:
                        int x = 0, y = 0;

                        switch (modTypes.GetPlayerDir(index))
                        {
                            case modTypes.DIR_UP:
                                if (modTypes.GetPlayerY(index) > 0)
                                {
                                    x = modTypes.GetPlayerX(index);
                                    y = modTypes.GetPlayerY(index) - 1;
                                }
                                else return;

                                break;

                            case modTypes.DIR_DOWN:
                                if (modTypes.GetPlayerY(index) < modTypes.MAX_MAPY)
                                {
                                    x = modTypes.GetPlayerX(index);
                                    y = modTypes.GetPlayerY(index) + 1;
                                }
                                else return;

                                break;

                            case modTypes.DIR_LEFT:
                                if (modTypes.GetPlayerX(index) > 0)
                                {
                                    x = modTypes.GetPlayerX(index) - 1;
                                    y = modTypes.GetPlayerY(index);
                                }
                                else return;

                                break;

                            case modTypes.DIR_RIGHT:
                                if (modTypes.GetPlayerX(index) < modTypes.MAX_MAPX)
                                {
                                    x = modTypes.GetPlayerX(index) + 1;
                                    y = modTypes.GetPlayerY(index);
                                }
                                else return;

                                break;
                        }

                        var mapNum = modTypes.GetPlayerMap(index);

                        // Check if a key exists
                        if (modTypes.Map[mapNum].Tile[x, y].Type == modTypes.TILE_TYPE_KEY)
                        {
                            // Check if the key they are using matches the map key
                            if (modTypes.GetPlayerInvItemNum(index, invNum) == modTypes.Map[mapNum].Tile[x, y].Data1)
                            {
                                modTypes.TempTile[mapNum].DoorOpen[x, y] = modTypes.YES;
                                modTypes.TempTile[mapNum].DoorTimer = modGeneral.GetTickCount();

                                modServerTCP.SendDataToMap(mapNum,
                                    "MAPKEY" +
                                    modTypes.SEP_CHAR + x +
                                    modTypes.SEP_CHAR + y +
                                    modTypes.SEP_CHAR + 1 +
                                    modTypes.SEP_CHAR);

                                modServerTCP.MapMsg(mapNum, "A door has been unlocked.", modText.White);

                                // Check if we are supposed to take away the item
                                if (modTypes.Map[mapNum].Tile[x, y].Data2 == 1)
                                {
                                    modGameLogic.TakeItem(index, modTypes.GetPlayerInvItemNum(index, invNum), 0);
                                    modServerTCP.PlayerMsg(index, "The key disolves.", modText.Yellow);
                                }
                            }
                        }

                        break;

                    case modTypes.ITEM_TYPE_SPELL:
                        // Get the spell num
                        var spellNum = modTypes.Item[modTypes.GetPlayerInvItemNum(index, invNum)].Data1;

                        if (spellNum > 0)
                        {
                            // Make sure they are the right class
                            if (modTypes.Spell[spellNum].ClassReq - 1 == modTypes.GetPlayerClass(index) || modTypes.Spell[spellNum].ClassReq == 0)
                            {
                                // Make sure they are the right level
                                var i = modGameLogic.GetSpellReqLevel(index, n);
                                if (i <= modTypes.GetPlayerLevel(index))
                                {
                                    i = modGameLogic.FindOpenSpellSlot(index);

                                    // Make sure they have an open spell slot
                                    if (i > 0)
                                    {
                                        // Make sure they dont already have the spell
                                        if (!modGameLogic.HasSpell(index, n))
                                        {
                                            modTypes.SetPlayerSpell(index, i, n);
                                            modGameLogic.TakeItem(index, modTypes.GetPlayerInvItemNum(index, invNum), 0);
                                            modServerTCP.PlayerMsg(index, "You study the spell carefully...", modText.Yellow);
                                            modServerTCP.PlayerMsg(index, "You have learned a new spell!", modText.White);
                                        }
                                        else
                                        {
                                            modGameLogic.TakeItem(index, modTypes.GetPlayerInvItemNum(index, invNum), 0);
                                            modServerTCP.PlayerMsg(index, "You have already learned this spell! The spells crumbles into dust.", modText.BrightRed);
                                        }
                                    }
                                    else
                                    {
                                        modServerTCP.PlayerMsg(index, "You have learned all that you can learn!", modText.BrightRed);
                                    }
                                }
                                else
                                {
                                    modServerTCP.PlayerMsg(index, $"You must be level {i} to learn this spell.", modText.White);
                                }
                            }
                            else
                            {
                                modServerTCP.PlayerMsg(index, $"This spell can only be learned by a {modTypes.GetClassName(modTypes.Spell[spellNum].ClassReq - 1)}.", modText.White);
                            }
                        }
                        else
                        {
                            modServerTCP.PlayerMsg(index, "This scroll is not connected to a spell, please inform an admin!", modText.White);
                        }

                        break;
                }
            }

            return;
        }

        // ::::::::::::::::::::::::::
        // :: Player attack packet ::
        // ::::::::::::::::::::::::::
        if (parse[0].Equals("attack", StringComparison.CurrentCultureIgnoreCase))
        {
            // Try to attack a player
            for (var i = 1; i <= modTypes.MAX_PLAYERS; i++)
            {
                // Make sure we dont try to attack ourselves
                if (i != index)
                {
                    // Can we attack the player?
                    if (modGameLogic.CanAttackPlayer(index, i))
                    {
                        if (!modGameLogic.CanPlayerBlockHit(i))
                        {
                            // Get the damage we can do
                            int damage;
                            if (!modGameLogic.CanPlayerCriticalHit(index))
                            {
                                damage = modGameLogic.GetPlayerDamage(index) - modGameLogic.GetPlayerProtection(i);
                            }
                            else
                            {
                                var n = modGameLogic.GetPlayerDamage(index);
                                damage = n + Random.Shared.Next(n / 2) + 1 - modGameLogic.GetPlayerProtection(i);
                                modServerTCP.PlayerMsg(index, "You feel a surge of energy upon swinging!", modText.BrightCyan);
                                modServerTCP.PlayerMsg(i, $"{modTypes.GetPlayerName(index)} swings with enormous might!", modText.BrightCyan);
                            }

                            if (damage > 0)
                            {
                                modGameLogic.AttackPlayer(index, i, damage);
                            }
                            else
                            {
                                modServerTCP.PlayerMsg(index, "Your attack does nothing.", modText.BrightRed);
                            }
                        }
                        else
                        {
                            modServerTCP.PlayerMsg(index, $"{modTypes.GetPlayerName(i)}'s {modTypes.Item[modTypes.GetPlayerInvItemNum(i, modTypes.GetPlayerShieldSlot(i))].Name.Trim()} has blocked your hit!", modText.BrightCyan);
                            modServerTCP.PlayerMsg(i, $"Your {modTypes.Item[modTypes.GetPlayerInvItemNum(i, modTypes.GetPlayerShieldSlot(i))].Name.Trim()} has blocked {modTypes.GetPlayerName(index)}'s hit!", modText.BrightRed);
                        }

                        return;
                    }
                }
            }

            // Try to attack a npc
            for (var i = 0; i <= modTypes.MAX_MAP_NPCS; i++)
            {
                // Can we attack the npc?
                if (modGameLogic.CanAttackNpc(index, i))
                {
                    // Get the damage we can do
                    int damage;
                    if (!modGameLogic.CanPlayerCriticalHit(index))
                    {
                        damage = modGameLogic.GetPlayerDamage(index) - modTypes.Npc[modTypes.MapNpc[modTypes.GetPlayerMap(index), i].Num].DEF / 2;
                    }
                    else
                    {
                        var n = modGameLogic.GetPlayerDamage(index);
                        damage = n + Random.Shared.Next(n / 2) + 1 - modTypes.Npc[modTypes.MapNpc[modTypes.GetPlayerMap(index), i].Num].DEF / 2;
                        modServerTCP.PlayerMsg(index, "You feel a surge of energy upon swinging!", modText.BrightCyan);
                    }

                    if (damage > 0)
                    {
                        modGameLogic.AttackNpc(index, i, damage);
                    }
                    else
                    {
                        modServerTCP.PlayerMsg(index, "Your attack does nothing.", modText.BrightRed);
                    }
                }
            }

            return;
        }

        // ::::::::::::::::::::::
        // :: Use stats packet ::
        // ::::::::::::::::::::::
        if (parse[0].Equals("usestatpoint", StringComparison.CurrentCultureIgnoreCase))
        {
            var pointType = int.Parse(parse[1]);

            // Prevent hacking
            if (pointType is < 0 or > 3)
            {
                modServerTCP.HackingAttempt(index, "Invalid Point Type");
                return;
            }

            // Make sure they have points
            if (modTypes.GetPlayerPOINTS(index) > 0)
            {
                // Take away a stat point
                modTypes.SetPlayerPOINTS(index, modTypes.GetPlayerPOINTS(index) - 1);

                // Everything is ok
                switch (pointType)
                {
                    case 0:
                        modTypes.SetPlayerSTR(index, modTypes.GetPlayerSTR(index) + 1);
                        modServerTCP.PlayerMsg(index, "You have gained more strength!", modText.White);
                        break;

                    case 1:
                        modTypes.SetPlayerSTR(index, modTypes.GetPlayerDEF(index) + 1);
                        modServerTCP.PlayerMsg(index, "You have gained more defense!", modText.White);
                        break;

                    case 2:
                        modTypes.SetPlayerSTR(index, modTypes.GetPlayerMAGI(index) + 1);
                        modServerTCP.PlayerMsg(index, "You have gained more magic abilities!", modText.White);
                        break;

                    case 3:
                        modTypes.SetPlayerSTR(index, modTypes.GetPlayerSPEED(index) + 1);
                        modServerTCP.PlayerMsg(index, "You have gained more speed!", modText.White);
                        break;
                }
            }
            else
            {
                modServerTCP.PlayerMsg(index, "You have no skill points to train with!", modText.BrightRed);
            }

            // Send the update
            modServerTCP.SendStats(index);
            return;
        }

        // ::::::::::::::::::::::::::::::::
        // :: Player info request packet ::
        // ::::::::::::::::::::::::::::::::
        if (parse[0].Equals("playerinforequest", StringComparison.CurrentCultureIgnoreCase))
        {
            var name = parse[1];

            var j = modGameLogic.FindPlayer(name);
            if (j > 0)
            {
                modServerTCP.PlayerMsg(index, $"Account: {modTypes.Player[j].Login.Trim()}, Name: {modTypes.GetPlayerName(j)}", modText.BrightGreen);
                if (modTypes.GetPlayerAccess(index) > modTypes.ADMIN_MONITER)
                {
                    modServerTCP.PlayerMsg(index, $"-=- Stats for {modTypes.GetPlayerName(j)} -=-", modText.BrightGreen);
                    modServerTCP.PlayerMsg(index, $"Level: {modTypes.GetPlayerLevel(j)}  Exp: {modTypes.GetPlayerExp(j)}/{modTypes.GetPlayerNextLevel(j)}", modText.BrightGreen);
                    modServerTCP.PlayerMsg(index, $"HP: {modTypes.GetPlayerHP(j)}/{modTypes.GetPlayerMaxHP(j)}  MP: {modTypes.GetPlayerMP(j)}/{modTypes.GetPlayerMaxMP(j)}  SP: {modTypes.GetPlayerSP(j)}/{modTypes.GetPlayerMaxSP(j)}", modText.BrightGreen);
                    modServerTCP.PlayerMsg(index, $"STR: {modTypes.GetPlayerSTR(j)}  DEF: {modTypes.GetPlayerDEF(j)}  MAGI: {modTypes.GetPlayerMAGI(j)}  SPEED: {modTypes.GetPlayerSPEED(j)}", modText.BrightGreen);
                    var n = modTypes.GetPlayerSTR(j) / 2 + modTypes.GetPlayerLevel(j) / 2;
                    var i = modTypes.GetPlayerDEF(j) / 2 + modTypes.GetPlayerLevel(j) / 2;
                    if (n > 100) n = 100;
                    if (i > 100) i = 100;
                    modServerTCP.PlayerMsg(index, $"Critical Hit Chance: {n}%, Block Chance: {i}%", modText.BrightGreen);
                }
            }
            else
            {
                modServerTCP.PlayerMsg(index, "Player is not online.", modText.White);
            }

            return;
        }

        // :::::::::::::::::::::::
        // :: Warp me to packet ::
        // :::::::::::::::::::::::
        if (parse[0].Equals("warpmeto", StringComparison.CurrentCultureIgnoreCase))
        {
            //  Prevent hacking
            if (modTypes.GetPlayerAccess(index) < modTypes.ADMIN_MAPPER)
            {
                modServerTCP.HackingAttempt(index, "Admin Cloning");
                return;
            }

            // The player
            var n = modGameLogic.FindPlayer(parse[1]);

            if (n != index)
            {
                if (n > 0)
                {
                    modGameLogic.PlayerWarp(index, modTypes.GetPlayerMap(n), modTypes.GetPlayerX(n), modTypes.GetPlayerY(n));
                    modServerTCP.PlayerMsg(n, $"{modTypes.GetPlayerName(index)} has warped to you.", modText.BrightBlue);
                    modServerTCP.PlayerMsg(index, $"You have been warped to {modTypes.GetPlayerName(n)}.", modText.BrightBlue);
                    Console.WriteLine($"{modTypes.GetPlayerName(index)} has warped to {modTypes.GetPlayerName(n)}, map #{modTypes.GetPlayerMap(n)}.");
                }
                else
                {
                    modServerTCP.PlayerMsg(index, "Player is not online.", modText.White);
                }
            }
            else
            {
                modServerTCP.PlayerMsg(index, "You cannot warp to yourself!", modText.White);
            }

            return;
        }

        // :::::::::::::::::::::::
        // :: Warp to me packet ::
        // :::::::::::::::::::::::
        if (parse[0].Equals("warptome", StringComparison.CurrentCultureIgnoreCase))
        {
            //  Prevent hacking
            if (modTypes.GetPlayerAccess(index) < modTypes.ADMIN_MAPPER)
            {
                modServerTCP.HackingAttempt(index, "Admin Cloning");
                return;
            }

            // The player
            var n = modGameLogic.FindPlayer(parse[1]);

            if (n != index)
            {
                if (n > 0)
                {
                    modGameLogic.PlayerWarp(n, modTypes.GetPlayerMap(index), modTypes.GetPlayerX(index), modTypes.GetPlayerY(index));
                    modServerTCP.PlayerMsg(n, $"You have been summoned by {modTypes.GetPlayerName(index)}.", modText.BrightBlue);
                    modServerTCP.PlayerMsg(index, $"{modTypes.GetPlayerName(n)} has been summoned.", modText.BrightBlue);
                    Console.WriteLine($"{modTypes.GetPlayerName(index)} has warped {modTypes.GetPlayerName(n)} to self, map #{modTypes.GetPlayerMap(index)}.");
                }
                else
                {
                    modServerTCP.PlayerMsg(index, "Player is not online.", modText.White);
                }
            }
            else
            {
                modServerTCP.PlayerMsg(index, "You cannot warp yourself to yourself!", modText.White);
            }

            return;
        }

        // ::::::::::::::::::::::::
        // :: Warp to map packet ::
        // ::::::::::::::::::::::::
        if (parse[0].Equals("warpto", StringComparison.CurrentCultureIgnoreCase))
        {
            //  Prevent hacking
            if (modTypes.GetPlayerAccess(index) < modTypes.ADMIN_MAPPER)
            {
                modServerTCP.HackingAttempt(index, "Admin Cloning");
                return;
            }

            // The map
            var n = int.Parse(parse[1]);

            // Prevent hacking
            if (n is < 0 or >= modTypes.MAX_MAPS)
            {
                modServerTCP.HackingAttempt(index, "Invalid map");
                return;
            }

            modGameLogic.PlayerWarp(index, n, modTypes.GetPlayerX(index), modTypes.GetPlayerY(index));
            modServerTCP.PlayerMsg(index, $"You have been warped to map #{n}", modText.BrightBlue);
            Console.WriteLine($"{modTypes.GetPlayerName(index)} warped to map #{n}");
            return;
        }

        // :::::::::::::::::::::::
        // :: Set sprite packet ::
        // :::::::::::::::::::::::
        if (parse[0].Equals("setsprite", StringComparison.CurrentCultureIgnoreCase))
        {
            // Prevent hacking
            if (modTypes.GetPlayerAccess(index) < modTypes.ADMIN_MAPPER)
            {
                modServerTCP.HackingAttempt(index, "Admin Cloning");
                return;
            }

            // The sprite
            var n = int.Parse(parse[1]);

            modTypes.SetPlayerSprite(index, n);
            modServerTCP.SendPlayerData(index);
            return;
        }

        // ::::::::::::::::::::::::::
        // :: Stats request packet ::
        // ::::::::::::::::::::::::::
        if (parse[0].Equals("getstats", StringComparison.CurrentCultureIgnoreCase))
        {
            modServerTCP.PlayerMsg(index, $"-=- Stats for {modTypes.GetPlayerName(index)} -=-", modText.White);
            modServerTCP.PlayerMsg(index, $"Level: {modTypes.GetPlayerLevel(index)}  Exp: {modTypes.GetPlayerExp(index)}/{modTypes.GetPlayerNextLevel(index)}", modText.White);
            modServerTCP.PlayerMsg(index, $"HP: {modTypes.GetPlayerHP(index)}/{modTypes.GetPlayerMaxHP(index)}  MP: {modTypes.GetPlayerMP(index)}/{modTypes.GetPlayerMaxMP(index)}  SP: {modTypes.GetPlayerSP(index)}/{modTypes.GetPlayerMaxSP(index)}", modText.White);
            modServerTCP.PlayerMsg(index, $"STR: {modTypes.GetPlayerSTR(index)}  DEF: {modTypes.GetPlayerDEF(index)}  MAGI: {modTypes.GetPlayerMAGI(index)}  SPEED: {modTypes.GetPlayerSPEED(index)}", modText.White);
            var n = modTypes.GetPlayerSTR(index) / 2 + modTypes.GetPlayerLevel(index) / 2;
            var i = modTypes.GetPlayerDEF(index) / 2 + modTypes.GetPlayerLevel(index) / 2;
            if (n > 100) n = 100;
            if (i > 100) i = 100;
            modServerTCP.PlayerMsg(index, $"Critical Hit Chance: {n}%, Block Chance: {i}%", modText.White);
            return;
        }

        // ::::::::::::::::::::::::::::::::::
        // :: Player request for a new map ::
        // ::::::::::::::::::::::::::::::::::
        if (parse[0].Equals("requestnewmap", StringComparison.CurrentCultureIgnoreCase))
        {
            var dir = int.Parse(parse[1]);

            // Prevent hacking
            if (dir is < modTypes.DIR_UP or > modTypes.DIR_RIGHT)
            {
                modServerTCP.HackingAttempt(index, "Invalid Direction");
                return;
            }

            modGameLogic.PlayerMove(index, dir, modTypes.MOVING_WALKING);
            return;
        }

        // :::::::::::::::::::::
        // :: Map data packet ::
        // :::::::::::::::::::::
        if (parse[0].Equals("mapdata", StringComparison.CurrentCultureIgnoreCase))
        {
            //  Prevent hacking
            if (modTypes.GetPlayerAccess(index) < modTypes.ADMIN_MAPPER)
            {
                modServerTCP.HackingAttempt(index, "Admin Cloning");
                return;
            }

            var n = 1;

            var mapNum = modTypes.GetPlayerMap(index);
            modTypes.Map[mapNum].Name = parse[n + 1];
            modTypes.Map[mapNum].Revision += 1;
            modTypes.Map[mapNum].Moral = int.Parse(parse[n + 3]);
            modTypes.Map[mapNum].Up = int.Parse(parse[n + 4]);
            modTypes.Map[mapNum].Down = int.Parse(parse[n + 5]);
            modTypes.Map[mapNum].Left = int.Parse(parse[n + 6]);
            modTypes.Map[mapNum].Right = int.Parse(parse[n + 7]);
            modTypes.Map[mapNum].Music = int.Parse(parse[n + 8]);
            modTypes.Map[mapNum].BootMap = int.Parse(parse[n + 9]);
            modTypes.Map[mapNum].BootX = int.Parse(parse[n + 10]);
            modTypes.Map[mapNum].BootY = int.Parse(parse[n + 11]);
            modTypes.Map[mapNum].Shop = int.Parse(parse[n + 12]);

            n += 13;
            for (var y = 0; y <= modTypes.MAX_MAPY; y++)
            {
                for (var x = 0; x <= modTypes.MAX_MAPX; x++)
                {
                    modTypes.Map[mapNum].Tile[x, y].Ground = int.Parse(parse[n]);
                    modTypes.Map[mapNum].Tile[x, y].Mask = int.Parse(parse[n + 1]);
                    modTypes.Map[mapNum].Tile[x, y].Anim = int.Parse(parse[n + 2]);
                    modTypes.Map[mapNum].Tile[x, y].Fringe = int.Parse(parse[n + 3]);
                    modTypes.Map[mapNum].Tile[x, y].Type = int.Parse(parse[n + 4]);
                    modTypes.Map[mapNum].Tile[x, y].Data1 = int.Parse(parse[n + 5]);
                    modTypes.Map[mapNum].Tile[x, y].Data2 = int.Parse(parse[n + 6]);
                    modTypes.Map[mapNum].Tile[x, y].Data3 = int.Parse(parse[n + 7]);

                    n += 8;
                }
            }

            for (var x = 1; x <= modTypes.MAX_MAP_NPCS; x++)
            {
                modTypes.Map[mapNum].Npc[x] = int.Parse(parse[n]);
                n += 1;
                modTypes.ClearMapNpc(x, mapNum);
            }

            modServerTCP.SendMapNpcsToMap(mapNum);
            modGameLogic.SpawnMapNpcs(mapNum);

            // Save the map
            modDatabase.SaveMap(mapNum);

            // Refresh map for everyone online
            for (var i = 1; i <= modTypes.MAX_PLAYERS; i++)
            {
                if (modServerTCP.IsPlaying(i) && modTypes.GetPlayerMap(i) == mapNum)
                {
                    modGameLogic.PlayerWarp(i, mapNum, modTypes.GetPlayerX(i), modTypes.GetPlayerY(i));
                }
            }

            return;
        }

        // ::::::::::::::::::::::::::::
        // :: Need map yes/no packet ::
        // ::::::::::::::::::::::::::::
        if (parse[0].Equals("needmap", StringComparison.CurrentCultureIgnoreCase))
        {
            // Get yes/no value
            var s = parse[1].ToLower();
            if (s == "yes")
            {
                modServerTCP.SendMap(index, modTypes.GetPlayerMap(index));
            }

            modServerTCP.SendMapItemsTo(index, modTypes.GetPlayerMap(index));
            modServerTCP.SendMapNpcsTo(index, modTypes.GetPlayerMap(index));
            modServerTCP.SendJoinMap(index);
            modTypes.Player[index].GettingMap = modTypes.NO;
            modServerTCP.SendDataTo(index, "MAPDONE" + modTypes.SEP_CHAR);

            return;
        }

        // :::::::::::::::::::::::::::::::::::::::::::::::
        // :: Player trying to pick up something packet ::
        // :::::::::::::::::::::::::::::::::::::::::::::::
        if (parse[0].Equals("mapgetitem", StringComparison.CurrentCultureIgnoreCase))
        {
            modGameLogic.PlayerMapGetItem(index);
            return;
        }

        // ::::::::::::::::::::::::::::::::::::::::::::
        // :: Player trying to drop something packet ::
        // ::::::::::::::::::::::::::::::::::::::::::::
        if (parse[0].Equals("mapdropitem", StringComparison.CurrentCultureIgnoreCase))
        {
            var invNum = int.Parse(parse[1]);
            var amount = int.Parse(parse[2]);

            // Prevent hacking
            if (invNum is < 1 or > modTypes.MAX_INV)
            {
                modServerTCP.HackingAttempt(index, "Invalid InvNum");
                return;
            }

            // Prevent hacking
            if (amount > modTypes.GetPlayerInvItemValue(index, invNum))
            {
                modServerTCP.HackingAttempt(index, "Item amount modification");
                return;
            }

            // Prevent hacking
            if (modTypes.Item[modTypes.GetPlayerInvItemNum(index, invNum)].Type == modTypes.ITEM_TYPE_CURRENCY)
            {
                // Check if money and if it is we want to make sure that they aren't trying to drop 0 value
                if (amount <= 0)
                {
                    modServerTCP.HackingAttempt(index, "Trying to drop 0 ammount of currency");
                    return;
                }
            }

            modGameLogic.PlayerMapDropItem(index, invNum, amount);
            return;
        }

        // ::::::::::::::::::::::::
        // :: Respawn map packet ::
        // ::::::::::::::::::::::::
        if (parse[0].Equals("maprespawn", StringComparison.CurrentCultureIgnoreCase))
        {
            //  Prevent hacking
            if (modTypes.GetPlayerAccess(index) < modTypes.ADMIN_MAPPER)
            {
                modServerTCP.HackingAttempt(index, "Admin Cloning");
                return;
            }

            // Clear out it all
            var mapNum = modTypes.GetPlayerMap(index);
            for (var i = 1; i <= modTypes.MAX_MAP_ITEMS; i++)
            {
                modGameLogic.SpawnItemSlot(i, 0, 0, 0, mapNum, modTypes.MapItem[mapNum, i].X, modTypes.MapItem[mapNum, i].Y);
                modTypes.ClearMapItem(i, mapNum);
            }

            // Respawn
            modGameLogic.SpawnMapItems(mapNum);

            // Repsawn NPCS
            for (var i = 0; i <= modTypes.MAX_MAP_NPCS; i++)
            {
                modGameLogic.SpawnNpc(i, mapNum);
            }

            modServerTCP.PlayerMsg(index, "Map respawned.", modText.Blue);
            Console.WriteLine($"{modTypes.GetPlayerName(index)} has respawned map #{mapNum}");
            return;
        }

        // :::::::::::::::::::::::
        // :: Map report packet ::
        // :::::::::::::::::::::::
        if (parse[0].Equals("mapreport", StringComparison.CurrentCultureIgnoreCase))
        {
            //  Prevent hacking
            if (modTypes.GetPlayerAccess(index) < modTypes.ADMIN_MAPPER)
            {
                modServerTCP.HackingAttempt(index, "Admin Cloning");
                return;
            }

            var mapStart = 1;
            var mapEnd = 1;

            var ranges = new List<string>();
            for (var i = 1; i <= modTypes.MAX_MAPS; i++)
            {
                if (string.IsNullOrWhiteSpace(modTypes.Map[i].Name))
                {
                    mapEnd++;
                }
                else
                {
                    if (mapEnd - mapStart > 0)
                    {
                        ranges.Add($"{mapStart}-{mapEnd - 1}");
                    }

                    mapStart = i + 1;
                    mapEnd = i + 1;
                }
            }

            ranges.Add($"{mapStart}-{mapEnd - 1}");
            modServerTCP.PlayerMsg(index, $"Free Maps: {string.Join(", ", ranges)}.", modText.Brown);
            return;
        }

        // ::::::::::::::::::::::::
        // :: Kick player packet ::
        // ::::::::::::::::::::::::
        if (parse[0].Equals("kickplayer", StringComparison.CurrentCultureIgnoreCase))
        {
            //  Prevent hacking
            if (modTypes.GetPlayerAccess(index) <= 0)
            {
                modServerTCP.HackingAttempt(index, "Admin Cloning");
                return;
            }

            // The player index
            var n = modGameLogic.FindPlayer(parse[1]);

            if (n != index)
            {
                if (n > 0)
                {
                    if (modTypes.GetPlayerAccess(n) <= modTypes.GetPlayerAccess(index))
                    {
                        modServerTCP.GlobalMsg($"{modTypes.GetPlayerName(n)} has been kicked from {modTypes.GAME_NAME} by {modTypes.GetPlayerName(index)}!", modText.White);
                        Console.WriteLine($"{modTypes.GetPlayerName(index)} has kicked {modTypes.GetPlayerName(n)}.");
                        modServerTCP.AlertMsg(n, $"You have been kicked by {modTypes.GetPlayerName(index)}!");
                    }
                    else
                    {
                        modServerTCP.PlayerMsg(index, "That is a higher access admin then you!", modText.White);
                    }
                }
                else
                {
                    modServerTCP.PlayerMsg(index, "Player is not online.", modText.White);
                }
            }
            else
            {
                modServerTCP.PlayerMsg(index, "You cannot kick yourself!", modText.White);
            }

            return;
        }

        // :::::::::::::::::::::
        // :: Ban list packet ::
        // :::::::::::::::::::::
        if (parse[0].Equals("banlist", StringComparison.CurrentCultureIgnoreCase))
        {
            //  Prevent hacking
            if (modTypes.GetPlayerAccess(index) < modTypes.ADMIN_MAPPER)
            {
                modServerTCP.HackingAttempt(index, "Admin Cloning");
                return;
            }

            if (!File.Exists("Banlist.txt"))
            {
                return;
            }

            var n = 1;

            using var reader = File.OpenText("Banlist.txt");
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line is null)
                {
                    continue;
                }

                var comma = line.IndexOf(',');
                if (comma == -1)
                {
                    continue;
                }

                var ip = line[..comma];
                var name = line[(comma + 1)..];

                modServerTCP.PlayerMsg(index, $"{n}: Banned IP {ip} by {name}", modText.White);
                n++;
            }

            return;
        }

        // ::::::::::::::::::::::::
        // :: Ban destroy packet ::
        // ::::::::::::::::::::::::
        if (parse[0].Equals("bandestroy", StringComparison.CurrentCultureIgnoreCase))
        {
            // Prevent hacking
            if (modTypes.GetPlayerAccess(index) < modTypes.ADMIN_CREATOR)
            {
                modServerTCP.HackingAttempt(index, "Admin Cloning");
                return;
            }

            if (File.Exists("Banlist.txt"))
            {
                File.Delete("Banlist.txt");
            }

            modServerTCP.PlayerMsg(index, "Ban list destroyed.", modText.White);
            return;
        }

        // :::::::::::::::::::::::
        // :: Ban player packet ::
        // :::::::::::::::::::::::
        if (parse[0].Equals("banplayer", StringComparison.CurrentCultureIgnoreCase))
        {
            // Prevent hacking
            if (modTypes.GetPlayerAccess(index) < modTypes.ADMIN_MAPPER)
            {
                modServerTCP.HackingAttempt(index, "Admin Cloning");
                return;
            }

            // The player index
            var n = modGameLogic.FindPlayer(parse[1]);

            if (n != index)
            {
                if (n > 0)
                {
                    if (modTypes.GetPlayerAccess(n) <= modTypes.GetPlayerAccess(index))
                    {
                        modDatabase.BanIndex(n, index);
                    }
                    else
                    {
                        modServerTCP.PlayerMsg(index, "That is a higher access admin then you!", modText.White);
                    }
                }
                else
                {
                    modServerTCP.PlayerMsg(index, "Player is not online.", modText.White);
                }
            }
            else
            {
                modServerTCP.PlayerMsg(index, "You cannot ban yourself!", modText.White);
            }

            return;
        }

        // :::::::::::::::::::::::::::::
        // :: Request edit map packet ::
        // :::::::::::::::::::::::::::::
        if (parse[0].Equals("requesteditmap", StringComparison.CurrentCultureIgnoreCase))
        {
            // Prevent hacking
            if (modTypes.GetPlayerAccess(index) < modTypes.ADMIN_MAPPER)
            {
                modServerTCP.HackingAttempt(index, "Admin Cloning");
                return;
            }

            modServerTCP.SendDataTo(index, "EDITMAP" + modTypes.SEP_CHAR);
            return;
        }

        // ::::::::::::::::::::::::::::::
        // :: Request edit item packet ::
        // ::::::::::::::::::::::::::::::
        if (parse[0].Equals("requestedititem", StringComparison.CurrentCultureIgnoreCase))
        {
            // Prevent hacking
            if (modTypes.GetPlayerAccess(index) < modTypes.ADMIN_DEVELOPER)
            {
                modServerTCP.HackingAttempt(index, "Admin Cloning");
                return;
            }

            modServerTCP.SendDataTo(index, "ITEMEDITOR" + modTypes.SEP_CHAR);
            return;
        }

        // ::::::::::::::::::::::
        // :: Edit item packet ::
        // ::::::::::::::::::::::
        if (parse[0].Equals("edititem", StringComparison.CurrentCultureIgnoreCase))
        {
            // Prevent hacking
            if (modTypes.GetPlayerAccess(index) < modTypes.ADMIN_DEVELOPER)
            {
                modServerTCP.HackingAttempt(index, "Admin Cloning");
                return;
            }

            // Item item #
            var n = int.Parse(parse[1]);

            // Prevent hacking
            if (n is < 0 or > modTypes.MAX_ITEMS)
            {
                modServerTCP.HackingAttempt(index, "Invalid Item Index");
                return;
            }

            Console.WriteLine($"{modTypes.GetPlayerName(index)} editing item #{n}.");
            modServerTCP.SendEditItemTo(index, n);
            return;
        }

        // ::::::::::::::::::::::
        // :: Save item packet ::
        // ::::::::::::::::::::::
        if (parse[0].Equals("saveitem", StringComparison.CurrentCultureIgnoreCase))
        {
            // Prevent hacking
            if (modTypes.GetPlayerAccess(index) < modTypes.ADMIN_DEVELOPER)
            {
                modServerTCP.HackingAttempt(index, "Admin Cloning");
                return;
            }

            var n = int.Parse(parse[1]);
            if (n is < 0 or > modTypes.MAX_ITEMS)
            {
                modServerTCP.HackingAttempt(index, "Invalid Item Index");
                return;
            }

            // Update the item
            modTypes.Item[n].Name = parse[2].Trim();
            modTypes.Item[n].Pic = int.Parse(parse[3]);
            modTypes.Item[n].Type = int.Parse(parse[4]);
            modTypes.Item[n].Data1 = int.Parse(parse[5]);
            modTypes.Item[n].Data2 = int.Parse(parse[6]);
            modTypes.Item[n].Data3 = int.Parse(parse[7]);

            // Save it
            modServerTCP.SendUpdateItemToAll(n);
            modDatabase.SaveItem(n);
            Console.WriteLine($"{modTypes.GetPlayerName(index)} saved item #{n}.");
            return;
        }

        // :::::::::::::::::::::::::::::
        // :: Request edit npc packet ::
        // :::::::::::::::::::::::::::::
        if (parse[0].Equals("requesteditnpc", StringComparison.CurrentCultureIgnoreCase))
        {
            // Prevent hacking
            if (modTypes.GetPlayerAccess(index) < modTypes.ADMIN_DEVELOPER)
            {
                modServerTCP.HackingAttempt(index, "Admin Cloning");
                return;
            }

            modServerTCP.SendDataTo(index, "NPCEDITOR" + modTypes.SEP_CHAR);
            return;
        }

        // :::::::::::::::::::::
        // :: Edit npc packet ::
        // :::::::::::::::::::::
        if (parse[0].Equals("editnpc", StringComparison.CurrentCultureIgnoreCase))
        {
            // Prevent hacking
            if (modTypes.GetPlayerAccess(index) < modTypes.ADMIN_DEVELOPER)
            {
                modServerTCP.HackingAttempt(index, "Admin Cloning");
                return;
            }

            // Item npc #
            var n = int.Parse(parse[1]);

            // Prevent hacking
            if (n is < 0 or > modTypes.MAX_NPCS)
            {
                modServerTCP.HackingAttempt(index, "Invalid NPC Index");
                return;
            }

            Console.WriteLine($"{modTypes.GetPlayerName(index)} editing npc #{n}.");
            modServerTCP.SendEditNpcTo(index, n);
            return;
        }

        // :::::::::::::::::::::
        // :: Save npc packet ::
        // :::::::::::::::::::::
        if (parse[0].Equals("savenpc", StringComparison.CurrentCultureIgnoreCase))
        {
            // Prevent hacking
            if (modTypes.GetPlayerAccess(index) < modTypes.ADMIN_DEVELOPER)
            {
                modServerTCP.HackingAttempt(index, "Admin Cloning");
                return;
            }

            var n = int.Parse(parse[1]);

            // Prevent hacking
            if (n is < 0 or >= modTypes.MAX_NPCS)
            {
                modServerTCP.HackingAttempt(index, "Invalid NPC Index");
                return;
            }

            // Update the npc
            modTypes.Npc[n].Name = parse[2].Trim();
            modTypes.Npc[n].AttackSay = parse[3].Trim();
            modTypes.Npc[n].Sprite = int.Parse(parse[4]);
            modTypes.Npc[n].SpawnSecs = int.Parse(parse[5]);
            modTypes.Npc[n].Behavior = int.Parse(parse[6]);
            modTypes.Npc[n].Range = int.Parse(parse[7]);
            modTypes.Npc[n].DropChance = int.Parse(parse[8]);
            modTypes.Npc[n].DropItem = int.Parse(parse[9]);
            modTypes.Npc[n].DropItemValue = int.Parse(parse[10]);
            modTypes.Npc[n].STR = int.Parse(parse[11]);
            modTypes.Npc[n].DEF = int.Parse(parse[12]);
            modTypes.Npc[n].SPEED = int.Parse(parse[13]);
            modTypes.Npc[n].MAGI = int.Parse(parse[14]);

            // Save it
            modServerTCP.SendUpdateNpcToAll(n);
            modDatabase.SaveNpc(n);
            Console.WriteLine($"{modTypes.GetPlayerName(index)} saved npc #{n}.");
            return;
        }

        // ::::::::::::::::::::::::::::::
        // :: Request edit shop packet ::
        // ::::::::::::::::::::::::::::::
        if (parse[0].Equals("requesteditshop", StringComparison.CurrentCultureIgnoreCase))
        {
            // Prevent hacking
            if (modTypes.GetPlayerAccess(index) < modTypes.ADMIN_DEVELOPER)
            {
                modServerTCP.HackingAttempt(index, "Admin Cloning");
                return;
            }

            modServerTCP.SendDataTo(index, "SHOPEDITOR" + modTypes.SEP_CHAR);
            return;
        }

        // ::::::::::::::::::::::
        // :: Edit shop packet ::
        // ::::::::::::::::::::::
        if (parse[0].Equals("editshop", StringComparison.CurrentCultureIgnoreCase))
        {
            // Prevent hacking
            if (modTypes.GetPlayerAccess(index) < modTypes.ADMIN_DEVELOPER)
            {
                modServerTCP.HackingAttempt(index, "Admin Cloning");
                return;
            }

            // Item shop #
            var n = int.Parse(parse[1]);

            // Prevent hacking
            if (n is < 0 or > modTypes.MAX_SHOPS)
            {
                modServerTCP.HackingAttempt(index, "Invalid Shop Index");
                return;
            }

            Console.WriteLine($"{modTypes.GetPlayerName(index)} editing shop #{n}.");
            modServerTCP.SendEditShopTo(index, n);
            return;
        }

        // ::::::::::::::::::::::
        // :: Save shop packet ::
        // ::::::::::::::::::::::
        if (parse[0].Equals("saveshop", StringComparison.CurrentCultureIgnoreCase))
        {
            // Prevent hacking
            if (modTypes.GetPlayerAccess(index) < modTypes.ADMIN_DEVELOPER)
            {
                modServerTCP.HackingAttempt(index, "Admin Cloning");
                return;
            }

            var shopNum = int.Parse(parse[1]);

            // Prevent hacking
            if (shopNum is < 0 or > modTypes.MAX_SHOPS)
            {
                modServerTCP.HackingAttempt(index, "Invalid Shop Index");
                return;
            }

            // Update the shop
            modTypes.Shop[shopNum].Name = parse[2].Trim();
            modTypes.Shop[shopNum].JoinSay = parse[3].Trim();
            modTypes.Shop[shopNum].LeaveSay = parse[4].Trim();
            modTypes.Shop[shopNum].FixesItems = int.Parse(parse[5]);

            var n = 6;
            for (var i = 0; i <= modTypes.MAX_TRADES; i++)
            {
                modTypes.Shop[shopNum].TradeItem[i].GiveItem = int.Parse(parse[n]);
                modTypes.Shop[shopNum].TradeItem[i].GiveValue = int.Parse(parse[n + 1]);
                modTypes.Shop[shopNum].TradeItem[i].GetItem = int.Parse(parse[n + 2]);
                modTypes.Shop[shopNum].TradeItem[i].GetValue = int.Parse(parse[n + 3]);
                n += 4;
            }

            // Save it
            modServerTCP.SendUpdateShopToAll(shopNum);
            modDatabase.SaveShop(shopNum);
            Console.WriteLine($"{modTypes.GetPlayerName(index)} saving shop #{shopNum}.");
            return;
        }

        // :::::::::::::::::::::::::::::::
        // :: Request edit spell packet ::
        // :::::::::::::::::::::::::::::::
        if (parse[0].Equals("requesteditspell", StringComparison.CurrentCultureIgnoreCase))
        {
            // Prevent hacking
            if (modTypes.GetPlayerAccess(index) < modTypes.ADMIN_DEVELOPER)
            {
                modServerTCP.HackingAttempt(index, "Admin Cloning");
                return;
            }

            modServerTCP.SendDataTo(index, "SPELLEDITOR" + modTypes.SEP_CHAR);
            return;
        }

        // :::::::::::::::::::::::
        // :: Edit spell packet ::
        // :::::::::::::::::::::::
        if (parse[0].Equals("editspell", StringComparison.CurrentCultureIgnoreCase))
        {
            // Prevent hacking
            if (modTypes.GetPlayerAccess(index) < modTypes.ADMIN_DEVELOPER)
            {
                modServerTCP.HackingAttempt(index, "Admin Cloning");
                return;
            }

            // The spell #
            var n = int.Parse(parse[1]);

            // Prevent hacking
            if (n is < 0 or > modTypes.MAX_SPELLS)
            {
                modServerTCP.HackingAttempt(index, "Invalid Spell Index");
                return;
            }

            Console.WriteLine($"{modTypes.GetPlayerName(index)} editing spell #{n}.");
            modServerTCP.SendEditSpellTo(index, n);
            return;
        }

        // :::::::::::::::::::::::
        // :: Save spell packet ::
        // :::::::::::::::::::::::
        if (parse[0].Equals("savespell", StringComparison.CurrentCultureIgnoreCase))
        {
            // Prevent hacking
            if (modTypes.GetPlayerAccess(index) < modTypes.ADMIN_DEVELOPER)
            {
                modServerTCP.HackingAttempt(index, "Admin Cloning");
                return;
            }

            // Spell #
            var n = int.Parse(parse[1]);

            // Prevent hacking
            if (n is < 0 or > modTypes.MAX_SPELLS)
            {
                modServerTCP.HackingAttempt(index, "Invalid Spell Index");
                return;
            }

            // Update the spell
            modTypes.Spell[n].Name = parse[2];
            modTypes.Spell[n].ClassReq = int.Parse(parse[3]);
            modTypes.Spell[n].LevelReq = int.Parse(parse[4]);
            modTypes.Spell[n].Type = int.Parse(parse[5]);
            modTypes.Spell[n].Data1 = int.Parse(parse[6]);
            modTypes.Spell[n].Data2 = int.Parse(parse[7]);
            modTypes.Spell[n].Data3 = int.Parse(parse[8]);

            // Save it
            modServerTCP.SendUpdateSpellToAll(n);
            modDatabase.SaveSpell(n);
            Console.WriteLine($"{modTypes.GetPlayerName(index)} saving spell #{n}.");

            return;
        }

        // :::::::::::::::::::::::
        // :: Set access packet ::
        // :::::::::::::::::::::::
        if (parse[0].Equals("setaccess", StringComparison.CurrentCultureIgnoreCase))
        {
            // Prevent hacking
            if (modTypes.GetPlayerAccess(index) < modTypes.ADMIN_CREATOR)
            {
                modServerTCP.HackingAttempt(index, "Trying to use powers not available");
                return;
            }

            // The index
            var n = modGameLogic.FindPlayer(parse[1]);

            // The access
            var i = int.Parse(parse[2]);

            // Check for invalid access level
            if (i is >= 0 and <= 3)
            {
                // Check if player is on
                if (n > 0)
                {
                    if (modTypes.GetPlayerAccess(n) <= 0)
                    {
                        modServerTCP.GlobalMsg($"{modTypes.GetPlayerName(n)} has been blessed with administrative access.", modText.BrightBlue);
                    }

                    modTypes.SetPlayerAccess(n, i);
                    modServerTCP.SendPlayerData(n);
                    Console.WriteLine($"{modTypes.GetPlayerName(index)} has modified {modTypes.GetPlayerName(n)}'s access..");
                }
                else
                {
                    modServerTCP.PlayerMsg(index, "Player is not online.", modText.White);
                }
            }
            else
            {
                modServerTCP.PlayerMsg(index, "Invalid access level.", modText.Red);
            }

            return;
        }

        // :::::::::::::::::::::::
        // :: Who online packet ::
        // :::::::::::::::::::::::
        if (parse[0].Equals("whosonline", StringComparison.CurrentCultureIgnoreCase))
        {
            modServerTCP.SendWhosOnline(index);
            return;
        }

        // :::::::::::::::::::::
        // :: Set MOTD packet ::
        // :::::::::::::::::::::
        if (parse[0].Equals("setmotd", StringComparison.CurrentCultureIgnoreCase))
        {
            // Prevent hacking
            if (modTypes.GetPlayerAccess(index) < modTypes.ADMIN_MAPPER)
            {
                modServerTCP.HackingAttempt(index, "Admin Cloning");
                return;
            }

            File.WriteAllText("Motd.txt", parse[1]);
            modServerTCP.GlobalMsg($"MOTD changed to: {parse[1]}", modText.BrightCyan);
            Console.WriteLine($"{modTypes.GetPlayerName(index)} changed MOTD to: {parse[1]}");
            return;
        }

        // ::::::::::::::::::
        // :: Trade packet ::
        // ::::::::::::::::::
        if (parse[0].Equals("trade", StringComparison.CurrentCultureIgnoreCase))
        {
            if (modTypes.Map[modTypes.GetPlayerMap(index)].Shop > 0)
            {
                modServerTCP.SendTrade(index, modTypes.Map[modTypes.GetPlayerMap(index)].Shop);
            }
            else
            {
                modServerTCP.PlayerMsg(index, "There is no shop here.", modText.BrightRed);
            }

            return;
        }

        // ::::::::::::::::::::::::::
        // :: Trade request packet ::
        // ::::::::::::::::::::::::::
        if (parse[0].Equals("traderequest", StringComparison.CurrentCultureIgnoreCase))
        {
            // Trade num
            var n = int.Parse(parse[1]);

            // Prevent hacking
            if (n is < 0 or > modTypes.MAX_TRADES)
            {
                modServerTCP.HackingAttempt(index, "Invalid Trade Index");
                return;
            }

            // Index for shop
            var i = modTypes.Map[modTypes.GetPlayerMap(index)].Shop;

            // Check if inv full
            var x = modGameLogic.FindOpenInvSlot(index, modTypes.Shop[i].TradeItem[n].GetItem);
            if (x == 0)
            {
                modServerTCP.PlayerMsg(index, "Trade unsuccessful, inventory full.", modText.BrightRed);
                return;
            }

            // Check if they have the item
            if (modGameLogic.HasItem(index, modTypes.Shop[i].TradeItem[n].GiveItem) >= modTypes.Shop[i].TradeItem[n].GiveValue)
            {
                modGameLogic.TakeItem(index, modTypes.Shop[i].TradeItem[n].GiveItem, modTypes.Shop[i].TradeItem[n].GiveValue);
                modGameLogic.GiveItem(index, modTypes.Shop[i].TradeItem[n].GetItem, modTypes.Shop[i].TradeItem[n].GetValue);
                modServerTCP.PlayerMsg(index, "The trade was successful!", modText.Yellow);
            }
            else
            {
                modServerTCP.PlayerMsg(index, "Trade unsuccessful.", modText.BrightRed);
            }

            return;
        }

        // :::::::::::::::::::::
        // :: Fix item packet ::
        // :::::::::::::::::::::
        if (parse[0].Equals("fixitem", StringComparison.CurrentCultureIgnoreCase))
        {
            // Inv num
            var n = int.Parse(parse[1]);

            var itemNum = modTypes.GetPlayerInvItemNum(index, n);

            // Make sure its a equipable item
            if (modTypes.Item[itemNum].Type < modTypes.ITEM_TYPE_WEAPON || modTypes.Item[itemNum].Type > modTypes.ITEM_TYPE_SHIELD)
            {
                modServerTCP.PlayerMsg(index, "You can only fix weapons, armors, helmets, and shields.", modText.BrightRed);
                return;
            }

            // Check if they have a full inventory
            if (modGameLogic.FindOpenInvSlot(index, itemNum) <= 0)
            {
                modServerTCP.PlayerMsg(index, "You have no inventory space left!", modText.BrightRed);
                return;
            }

            // Now check the rate of pay
            var i = modTypes.Item[itemNum].Data2 / 5;
            if (i <= 0) i = 1;

            var durNeeded = modTypes.Item[itemNum].Data1 - modTypes.GetPlayerInvItemDur(index, n);
            var goldNeeded = durNeeded * i / 2;
            if (goldNeeded <= 0) goldNeeded = 1;

            // Check if they even need it repaired
            if (durNeeded <= 0)
            {
                modServerTCP.PlayerMsg(index, "This item is in perfect condition!", modText.White);
                return;
            }

            // Check if they have enough for at least one point
            if (modGameLogic.HasItem(index, 1) >= i)
            {
                if (modGameLogic.HasItem(index, 1) >= goldNeeded)
                {
                    modGameLogic.TakeItem(index, 1, goldNeeded);
                    modTypes.SetPlayerInvItemDur(index, n, modTypes.Item[itemNum].Data1);
                    modServerTCP.PlayerMsg(index, $"Item has been totally restored for {goldNeeded} gold!", modText.BrightBlue);
                }
                else
                {
                    // They dont so restore as much as we can
                    durNeeded = modGameLogic.HasItem(index, 1) / i;
                    goldNeeded = durNeeded * i / 2;
                    if (goldNeeded <= 0) goldNeeded = 1;

                    modGameLogic.TakeItem(index, 1, goldNeeded);
                    modTypes.SetPlayerInvItemDur(index, n, modTypes.GetPlayerInvItemDur(index, n) + durNeeded);
                    modServerTCP.PlayerMsg(index, $"Item has been partially fixed for {goldNeeded} gold!", modText.BrightBlue);
                }
            }
            else
            {
                modServerTCP.PlayerMsg(index, "Insufficient gold to fix this item!", modText.BrightRed);
            }

            return;
        }

        // :::::::::::::::::::
        // :: Search packet ::
        // :::::::::::::::::::
        if (parse[0].Equals("search", StringComparison.CurrentCultureIgnoreCase))
        {
            var x = int.Parse(parse[1]);
            var y = int.Parse(parse[2]);

            // Prevent subscript out of range
            if (x < 0 || x > modTypes.MAX_MAPX || y < 0 || y > modTypes.MAX_MAPY)
            {
                return;
            }

            var mapNum = modTypes.GetPlayerMap(index);

            // Check for a player
            for (var i = 1; i <= modTypes.MAX_PLAYERS; i++)
            {
                if (modServerTCP.IsPlaying(i) && mapNum == modTypes.GetPlayerMap(i) && modTypes.GetPlayerX(i) == x && modTypes.GetPlayerY(i) == y)
                {
                    if (modTypes.GetPlayerLevel(i) >= modTypes.GetPlayerLevel(index) + 5)
                    {
                        modServerTCP.PlayerMsg(index, "You wouldn't stand a chance.", modText.BrightRed);
                    }
                    else
                    {
                        if (modTypes.GetPlayerLevel(i) > modTypes.GetPlayerLevel(index))
                        {
                            modServerTCP.PlayerMsg(index, "This one seems to have an advantage over you.", modText.Yellow);
                        }
                        else
                        {
                            if (modTypes.GetPlayerLevel(i) == modTypes.GetPlayerLevel(index))
                            {
                                modServerTCP.PlayerMsg(index, "This would be an even fight.", modText.White);
                            }
                            else
                            {
                                if (modTypes.GetPlayerLevel(index) >= modTypes.GetPlayerLevel(i) + 5)
                                {
                                    modServerTCP.PlayerMsg(index, "You could slaughter that player.", modText.BrightBlue);
                                }
                                else
                                {
                                    if (modTypes.GetPlayerLevel(index) >= modTypes.GetPlayerLevel(i))
                                    {
                                        modServerTCP.PlayerMsg(index, "You would have an advantage over that player.", modText.Yellow);
                                    }
                                }
                            }
                        }
                    }

                    // Change target
                    modTypes.Player[index].Target = i;
                    modTypes.Player[index].TargetType = modTypes.TARGET_TYPE_PLAYER;
                    modServerTCP.PlayerMsg(index, $"Your target is now {modTypes.GetPlayerName(i)}.", modText.Yellow);
                    return;
                }
            }

            // Check for an item
            for (var i = 1; i <= modTypes.MAX_MAP_ITEMS; i++)
            {
                if (modTypes.MapItem[mapNum, i].Num > 0)
                {
                    if (modTypes.MapItem[mapNum, i].X == x && modTypes.MapItem[mapNum, i].Y == y)
                    {
                        modServerTCP.PlayerMsg(index, $"You see a {modTypes.Item[modTypes.MapItem[mapNum, i].Num].Name.Trim()}.", modText.Yellow);
                        return;
                    }
                }
            }

            // Check for an npc
            for (var i = 1; i <= modTypes.MAX_MAP_NPCS; i++)
            {
                if (modTypes.MapNpc[mapNum, i].Num > 0)
                {
                    if (modTypes.MapNpc[mapNum, i].X == x && modTypes.MapNpc[mapNum, i].Y == y)
                    {
                        // Change target
                        modTypes.Player[index].Target = i;
                        modTypes.Player[index].TargetType = modTypes.TARGET_TYPE_NPC;
                        modServerTCP.PlayerMsg(index, $"Your target is now a {modTypes.Npc[modTypes.MapNpc[mapNum, i].Num].Name.Trim()}.", modText.Yellow);
                        return;
                    }
                }
            }

            return;
        }

        // ::::::::::::::::::
        // :: Party packet ::
        // ::::::::::::::::::
        if (parse[0].Equals("party", StringComparison.CurrentCultureIgnoreCase))
        {
            var n = modGameLogic.FindPlayer(parse[1]);

            // Prevent partying with self
            if (n == index)
            {
                return;
            }

            // Check for a previous party and if so drop it
            if (modTypes.Player[index].InParty == modTypes.YES)
            {
                modServerTCP.PlayerMsg(index, "You are already in a party!", modText.Pink);
                return;
            }

            if (n > 0)
            {
                // Check if its an admin
                if (modTypes.GetPlayerAccess(index) > modTypes.ADMIN_MONITER)
                {
                    modServerTCP.PlayerMsg(index, "You can't join a party, you are an admin!", modText.BrightBlue);
                    return;
                }

                if (modTypes.GetPlayerAccess(n) > modTypes.ADMIN_MONITER)
                {
                    modServerTCP.PlayerMsg(index, "Admins cannot join parties!", modText.BrightBlue);
                    return;
                }

                // Make sure they are in right level range
                if (modTypes.GetPlayerLevel(index) + 5 < modTypes.GetPlayerLevel(n) || modTypes.GetPlayerLevel(index) - 5 > modTypes.GetPlayerLevel(n))
                {
                    modServerTCP.PlayerMsg(index, "There is more then a 5 level gap between you two, party failed.", modText.Pink);
                    return;
                }

                if (modTypes.Player[n].InParty == modTypes.NO)
                {
                    modServerTCP.PlayerMsg(index, $"Party request has been sent to {modTypes.GetPlayerName(n)}.", modText.Pink);
                    modServerTCP.PlayerMsg(n, $"{modTypes.GetPlayerName(index)} wants you to join their party.  Type /join to join, or /leave to decline.", modText.Pink);

                    modTypes.Player[index].PartyStarter = modTypes.YES;
                    modTypes.Player[index].PartyPlayer = n;
                    modTypes.Player[n].PartyPlayer = index;
                }
                else
                {
                    modServerTCP.PlayerMsg(index, "Player is already in a party!", modText.Pink);
                }
            }
            else
            {
                modServerTCP.PlayerMsg(index, "Player is not online.", modText.White);
            }

            return;
        }

        // :::::::::::::::::::::::
        // :: Join party packet ::
        // :::::::::::::::::::::::
        if (parse[0].Equals("joinparty", StringComparison.CurrentCultureIgnoreCase))
        {
            var n = modTypes.Player[index].PartyPlayer;

            if (n > 0)
            {
                // Check to make sure they aren't the starter
                if (modTypes.Player[index].PartyStarter == modTypes.NO)
                {
                    // Check to make sure that each of there party players match
                    if (modTypes.Player[n].PartyPlayer == index)
                    {
                        modServerTCP.PlayerMsg(index, $"You have joined {modTypes.GetPlayerName(n)}'s party!", modText.Pink);
                        modServerTCP.PlayerMsg(n, $"{modTypes.GetPlayerName(index)} has joined your party!", modText.Pink);

                        modTypes.Player[index].InParty = modTypes.YES;
                        modTypes.Player[n].InParty = modTypes.YES;
                    }
                    else
                    {
                        modServerTCP.PlayerMsg(index, "Party failed.", modText.Pink);
                    }
                }
                else
                {
                    modServerTCP.PlayerMsg(index, "You have not been invited into a party!", modText.Pink);
                }
            }

            return;
        }

        // ::::::::::::::::::::::::
        // :: Leave party packet ::
        // ::::::::::::::::::::::::
        if (parse[0].Equals("leaveparty", StringComparison.CurrentCultureIgnoreCase))
        {
            var n = modTypes.Player[index].PartyPlayer;

            if (n > 0)
            {
                if (modTypes.Player[index].InParty == modTypes.YES)
                {
                    modServerTCP.PlayerMsg(index, "You have left the party.", modText.Pink);
                    modServerTCP.PlayerMsg(n, $"{modTypes.GetPlayerName(index)} has left the party.", modText.Pink);
                }
                else
                {
                    modServerTCP.PlayerMsg(index, "Declined party request.", modText.Pink);
                    modServerTCP.PlayerMsg(n, $"{modTypes.GetPlayerName(index)} declined your request.", modText.Pink);
                }

                modTypes.Player[index].PartyPlayer = 0;
                modTypes.Player[index].PartyStarter = modTypes.NO;
                modTypes.Player[index].InParty = modTypes.NO;
                modTypes.Player[n].PartyPlayer = 0;
                modTypes.Player[n].PartyStarter = modTypes.NO;
                modTypes.Player[n].InParty = modTypes.NO;
            }
            else
            {
                modServerTCP.PlayerMsg(index, "You are not in a party!", modText.Pink);
            }

            return;
        }

        // :::::::::::::::::::
        // :: Spells packet ::
        // :::::::::::::::::::
        if (parse[0].Equals("spells", StringComparison.CurrentCultureIgnoreCase))
        {
            modServerTCP.SendPlayerSpells(index);
            return;
        }

        // :::::::::::::::::
        // :: Cast packet ::
        // :::::::::::::::::
        if (parse[0].Equals("cast", StringComparison.CurrentCultureIgnoreCase))
        {
            // Spell slot
            var n = int.Parse(parse[1]);

            modGameLogic.CastSpell(index, n);

            return;
        }

        // :::::::::::::::::::::
        // :: Location packet ::
        // :::::::::::::::::::::
        if (parse[0].Equals("requestlocation", StringComparison.CurrentCultureIgnoreCase))
        {
            if (modTypes.GetPlayerAccess(index) < modTypes.ADMIN_MAPPER)
            {
                modServerTCP.HackingAttempt(index, "Admin Cloning");
                return;
            }

            modServerTCP.PlayerMsg(index, $"Map: {modTypes.GetPlayerMap(index)}, X: {modTypes.GetPlayerX(index)}, Y: {modTypes.GetPlayerY(index)}", modText.Pink);
        }
    }
}