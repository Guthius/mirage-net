using Mirage.Forms;

namespace Mirage.Modules;

public static class modHandleData
{
    public static void HandleData(string data)
    {
        // Handle Data
        var parse = data.Split(modTypes.SEP_CHAR);
        if (parse.Length == 0)
        {
            return;
        }
        
        // ::::::::::::::::::::::::::
        // :: Alert message packet ::
        // ::::::::::::::::::::::::::
        if (parse[0].Equals("alertmsg", StringComparison.CurrentCultureIgnoreCase))
        {
            var msg = parse[1];

            My.Forms.frmSendGetData.Hide();
            My.Forms.frmMainMenu.Show();

            MessageBox.Show(msg, modTypes.GAME_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);

            return;
        }

        // :::::::::::::::::::::::::::
        // :: All characters packet ::
        // :::::::::::::::::::::::::::
        if (parse[0].Equals("allchars", StringComparison.CurrentCultureIgnoreCase))
        {
            var n = 1;

            My.Forms.frmChars.Show();
            My.Forms.frmSendGetData.Hide();

            My.Forms.frmChars.lstChars.Items.Clear();
            for (var i = 1; i <= modTypes.MAX_CHARS; i++)
            {
                var name = parse[n];
                var msg = parse[n + 1];
                var level = int.Parse(parse[n + 2]);

                if (string.IsNullOrEmpty(name))
                {
                    My.Forms.frmChars.lstChars.Items.Add("Free Character Slot");
                }
                else
                {
                    My.Forms.frmChars.lstChars.Items.Add($"{name} a level {level} {msg}");
                }

                n += 3;
            }

            My.Forms.frmChars.lstChars.SelectedIndex = 0;
            return;
        }

        // :::::::::::::::::::::::::::::::::
        // :: Login was successful packet ::
        // :::::::::::::::::::::::::::::::::
        if (parse[0].Equals("loginok", StringComparison.CurrentCultureIgnoreCase))
        {
            // Now we can receive game data
            modGameLogic.MyIndex = int.Parse(parse[1]);

            My.Forms.frmSendGetData.Show();
            My.Forms.frmChars.Hide();

            modGameLogic.SetStatus("Receiving game data...");
            return;
        }

        // :::::::::::::::::::::::::::::::::::::::
        // :: New character classes data packet ::
        // :::::::::::::::::::::::::::::::::::::::
        if (parse[0].Equals("newcharclasses", StringComparison.CurrentCultureIgnoreCase))
        {
            // Max classes
            modTypes.Max_Classes = int.Parse(parse[1]);
            modTypes.Class = new modTypes.ClassRec[modTypes.Max_Classes];

            var n = 2;
            for (var i = 0; i < modTypes.Max_Classes; i++)
            {
                modTypes.Class[i].Name = parse[n];

                modTypes.Class[i].HP = int.Parse(parse[n + 1]);
                modTypes.Class[i].MP = int.Parse(parse[n + 2]);
                modTypes.Class[i].SP = int.Parse(parse[n + 3]);

                modTypes.Class[i].STR = int.Parse(parse[n + 4]);
                modTypes.Class[i].DEF = int.Parse(parse[n + 5]);
                modTypes.Class[i].SPEED = int.Parse(parse[n + 6]);
                modTypes.Class[i].MAGI = int.Parse(parse[n + 7]);

                n += 8;
            }

            // Used for if the player is creating a new character
            My.Forms.frmNewChar.RefreshClasses();
            My.Forms.frmNewChar.Show();
            My.Forms.frmSendGetData.Hide();
            return;
        }

        // :::::::::::::::::::::::::
        // :: Classes data packet ::
        // :::::::::::::::::::::::::
        if (parse[0].Equals("classesdata", StringComparison.CurrentCultureIgnoreCase))
        {
            // Max classes
            modTypes.Max_Classes = int.Parse(parse[1]);
            modTypes.Class = new modTypes.ClassRec[modTypes.Max_Classes];

            var n = 2;
            for (var i = 0; i < modTypes.Max_Classes; i++)
            {
                modTypes.Class[i].Name = parse[n];

                modTypes.Class[i].HP = int.Parse(parse[n + 1]);
                modTypes.Class[i].MP = int.Parse(parse[n + 2]);
                modTypes.Class[i].SP = int.Parse(parse[n + 3]);

                modTypes.Class[i].STR = int.Parse(parse[n + 4]);
                modTypes.Class[i].DEF = int.Parse(parse[n + 5]);
                modTypes.Class[i].SPEED = int.Parse(parse[n + 6]);
                modTypes.Class[i].MAGI = int.Parse(parse[n + 7]);

                n += 8;
            }

            return;
        }

        // ::::::::::::::::::::
        // :: In game packet ::
        // ::::::::::::::::::::
        if (parse[0].Equals("ingame", StringComparison.CurrentCultureIgnoreCase))
        {
            modClientTCP.InGame = true;

            modGameLogic.GameInit();
            modGameLogic.GameLoop();
            Application.Exit();
        }

        // :::::::::::::::::::::::::::::
        // :: Player inventory packet ::
        // :::::::::::::::::::::::::::::
        if (parse[0].Equals("playerinv", StringComparison.CurrentCultureIgnoreCase))
        {
            var n = 1;
            for (var i = 1; i <= modTypes.MAX_INV; i++)
            {
                modTypes.SetPlayerInvItemNum(modGameLogic.MyIndex, i, int.Parse(parse[n]));
                modTypes.SetPlayerInvItemValue(modGameLogic.MyIndex, i, int.Parse(parse[n + 1]));
                modTypes.SetPlayerInvItemDur(modGameLogic.MyIndex, i, int.Parse(parse[n + 2]));

                n += 3;
            }

            modGameLogic.UpdateInventory();
            return;
        }

        // ::::::::::::::::::::::::::::::::::::
        // :: Player inventory update packet ::
        // ::::::::::::::::::::::::::::::::::::
        if (parse[0].Equals("playerinvupdate", StringComparison.CurrentCultureIgnoreCase))
        {
            var n = int.Parse(parse[1]);

            modTypes.SetPlayerInvItemNum(modGameLogic.MyIndex, n, int.Parse(parse[2]));
            modTypes.SetPlayerInvItemValue(modGameLogic.MyIndex, n, int.Parse(parse[3]));
            modTypes.SetPlayerInvItemDur(modGameLogic.MyIndex, n, int.Parse(parse[4]));
            modGameLogic.UpdateInventory();
            return;
        }

        // ::::::::::::::::::::::::::::::::::
        // :: Player worn equipment packet ::
        // ::::::::::::::::::::::::::::::::::
        if (parse[0].Equals("playerworneq", StringComparison.CurrentCultureIgnoreCase))
        {
            modTypes.SetPlayerArmorSlot(modGameLogic.MyIndex, int.Parse(parse[1]));
            modTypes.SetPlayerWeaponSlot(modGameLogic.MyIndex, int.Parse(parse[2]));
            modTypes.SetPlayerHelmetSlot(modGameLogic.MyIndex, int.Parse(parse[3]));
            modTypes.SetPlayerShieldSlot(modGameLogic.MyIndex, int.Parse(parse[4]));
            modGameLogic.UpdateInventory();
            return;
        }

        // ::::::::::::::::::::::
        // :: Player hp packet ::
        // ::::::::::::::::::::::
        if (parse[0].Equals("playerhp", StringComparison.CurrentCultureIgnoreCase))
        {
            modTypes.Player[modGameLogic.MyIndex].MaxHP = int.Parse(parse[1]);
            modTypes.SetPlayerHP(modGameLogic.MyIndex, int.Parse(parse[2]));
            if (modTypes.GetPlayerMaxHP(modGameLogic.MyIndex) > 0)
            {
                var percent = (float) modTypes.GetPlayerHP(modGameLogic.MyIndex) / modTypes.GetPlayerMaxHP(modGameLogic.MyIndex) * 100;
                My.Forms.frmMirage.lblHP.Text = $"{(int) percent}%";
            }

            return;
        }

        // ::::::::::::::::::::::
        // :: Player mp packet ::
        // ::::::::::::::::::::::
        if (parse[0].Equals("playermp", StringComparison.CurrentCultureIgnoreCase))
        {
            modTypes.Player[modGameLogic.MyIndex].MaxMP = int.Parse(parse[1]);
            modTypes.SetPlayerMP(modGameLogic.MyIndex, int.Parse(parse[2]));
            if (modTypes.GetPlayerMaxMP(modGameLogic.MyIndex) > 0)
            {
                var percent = (float) modTypes.GetPlayerMP(modGameLogic.MyIndex) / modTypes.GetPlayerMaxMP(modGameLogic.MyIndex) * 100;
                My.Forms.frmMirage.lblMP.Text = $"{(int) percent}%";
            }

            return;
        }

        // ::::::::::::::::::::::
        // :: Player sp packet ::
        // ::::::::::::::::::::::
        if (parse[0].Equals("playersp", StringComparison.CurrentCultureIgnoreCase))
        {
            modTypes.Player[modGameLogic.MyIndex].MaxSP = int.Parse(parse[1]);
            modTypes.SetPlayerSP(modGameLogic.MyIndex, int.Parse(parse[2]));
            if (modTypes.GetPlayerMaxSP(modGameLogic.MyIndex) > 0)
            {
                var percent = (float) modTypes.GetPlayerSP(modGameLogic.MyIndex) / modTypes.GetPlayerMaxSP(modGameLogic.MyIndex) * 100;
                My.Forms.frmMirage.lblSP.Text = $"{(int) percent}%";
            }

            return;
        }

        // :::::::::::::::::::::::::
        // :: Player stats packet ::
        // :::::::::::::::::::::::::
        if (parse[0].Equals("playerstats", StringComparison.CurrentCultureIgnoreCase))
        {
            modTypes.SetPlayerSTR(modGameLogic.MyIndex, int.Parse(parse[1]));
            modTypes.SetPlayerDEF(modGameLogic.MyIndex, int.Parse(parse[2]));
            modTypes.SetPlayerSPEED(modGameLogic.MyIndex, int.Parse(parse[3]));
            modTypes.SetPlayerMAGI(modGameLogic.MyIndex, int.Parse(parse[4]));
            return;
        }

        // ::::::::::::::::::::::::
        // :: Player data packet ::
        // ::::::::::::::::::::::::
        if (parse[0].Equals("playerdata", StringComparison.CurrentCultureIgnoreCase))
        {
            var i = int.Parse(parse[1]);

            modTypes.SetPlayerName(i, parse[2]);
            modTypes.SetPlayerSprite(i, int.Parse(parse[3]));
            modTypes.SetPlayerMap(i, int.Parse(parse[4]));
            modTypes.SetPlayerX(i, int.Parse(parse[5]));
            modTypes.SetPlayerY(i, int.Parse(parse[6]));
            modTypes.SetPlayerDir(i, int.Parse(parse[7]));
            modTypes.SetPlayerAccess(i, int.Parse(parse[8]));
            modTypes.SetPlayerPK(i, int.Parse(parse[9]));

            modTypes.Player[i].Moving = 0;
            modTypes.Player[i].XOffset = 0;
            modTypes.Player[i].YOffset = 0;

            if (i == modGameLogic.MyIndex)
            {
                modGameLogic.DirUp = false;
                modGameLogic.DirDown = false;
                modGameLogic.DirLeft = false;
                modGameLogic.DirRight = false;
            }

            return;
        }

        // ::::::::::::::::::::::::::::
        // :: Player movement packet ::
        // ::::::::::::::::::::::::::::
        if (parse[0].Equals("playermove", StringComparison.CurrentCultureIgnoreCase))
        {
            var i = int.Parse(parse[1]);
            var x = int.Parse(parse[2]);
            var y = int.Parse(parse[3]);
            var dir = int.Parse(parse[4]);
            var n = int.Parse(parse[5]);

            modTypes.SetPlayerX(i, x);
            modTypes.SetPlayerY(i, y);
            modTypes.SetPlayerDir(i, dir);

            modTypes.Player[i].XOffset = 0;
            modTypes.Player[i].YOffset = 0;
            modTypes.Player[i].Moving = n;

            switch (modTypes.GetPlayerDir(i))
            {
                case modTypes.DIR_UP:
                    modTypes.Player[i].YOffset = modTypes.PIC_Y;
                    break;
                case modTypes.DIR_DOWN:
                    modTypes.Player[i].YOffset = modTypes.PIC_Y * -1;
                    break;
                case modTypes.DIR_LEFT:
                    modTypes.Player[i].XOffset = modTypes.PIC_X;
                    break;
                case modTypes.DIR_RIGHT:
                    modTypes.Player[i].XOffset = modTypes.PIC_X * -1;
                    break;
            }

            return;
        }

        // :::::::::::::::::::::::::
        // :: Npc movement packet ::
        // :::::::::::::::::::::::::
        if (parse[0].Equals("npcmove", StringComparison.CurrentCultureIgnoreCase))
        {
            var i = int.Parse(parse[1]);
            var x = int.Parse(parse[2]);
            var y = int.Parse(parse[3]);
            var dir = int.Parse(parse[4]);
            var n = int.Parse(parse[5]);

            modTypes.MapNpc[i].X = x;
            modTypes.MapNpc[i].Y = y;
            modTypes.MapNpc[i].Dir = dir;
            modTypes.MapNpc[i].XOffset = 0;
            modTypes.MapNpc[i].YOffset = 0;
            modTypes.MapNpc[i].Moving = n;

            switch (modTypes.MapNpc[i].Dir)
            {
                case modTypes.DIR_UP:
                    modTypes.MapNpc[i].YOffset = modTypes.PIC_Y;
                    break;
                case modTypes.DIR_DOWN:
                    modTypes.MapNpc[i].YOffset = modTypes.PIC_Y * -1;
                    break;
                case modTypes.DIR_LEFT:
                    modTypes.MapNpc[i].XOffset = modTypes.PIC_X;
                    break;
                case modTypes.DIR_RIGHT:
                    modTypes.MapNpc[i].XOffset = modTypes.PIC_X * -1;
                    break;
            }

            return;
        }

        // :::::::::::::::::::::::::::::
        // :: Player direction packet ::
        // :::::::::::::::::::::::::::::
        if (parse[0].Equals("playerdir", StringComparison.CurrentCultureIgnoreCase))
        {
            var i = int.Parse(parse[1]);
            var dir = int.Parse(parse[2]);
            modTypes.SetPlayerDir(i, dir);

            modTypes.Player[i].XOffset = 0;
            modTypes.Player[i].YOffset = 0;
            modTypes.Player[i].Moving = 0;
            return;
        }

        // ::::::::::::::::::::::::::
        // :: NPC direction packet ::
        // ::::::::::::::::::::::::::
        if (parse[0].Equals("npcdir", StringComparison.CurrentCultureIgnoreCase))
        {
            var i = int.Parse(parse[1]);
            var dir = int.Parse(parse[2]);
            modTypes.MapNpc[i].Dir = dir;
            modTypes.MapNpc[i].XOffset = 0;
            modTypes.MapNpc[i].YOffset = 0;
            modTypes.MapNpc[i].Moving = 0;
            return;
        }

        // :::::::::::::::::::::::::::::::
        // :: Player XY location packet ::
        // :::::::::::::::::::::::::::::::
        if (parse[0].Equals("playerxy", StringComparison.CurrentCultureIgnoreCase))
        {
            var x = int.Parse(parse[1]);
            var y = int.Parse(parse[2]);

            modTypes.SetPlayerX(modGameLogic.MyIndex, x);
            modTypes.SetPlayerY(modGameLogic.MyIndex, y);

            // Make sure they aren't walking
            modTypes.Player[modGameLogic.MyIndex].XOffset = 0;
            modTypes.Player[modGameLogic.MyIndex].YOffset = 0;
            modTypes.Player[modGameLogic.MyIndex].Moving = 0;
            return;
        }

        // ::::::::::::::::::::::::::
        // :: Player attack packet ::
        // ::::::::::::::::::::::::::
        if (parse[0].Equals("attack", StringComparison.CurrentCultureIgnoreCase))
        {
            var i = int.Parse(parse[1]);

            // Set player to attacking
            modTypes.Player[i].Attacking = 1;
            modTypes.Player[i].AttackTimer = modGameLogic.GetTickCount();
            return;
        }

        // :::::::::::::::::::::::
        // :: NPC attack packet ::
        // :::::::::::::::::::::::
        if (parse[0].Equals("npcattack", StringComparison.CurrentCultureIgnoreCase))
        {
            var i = int.Parse(parse[1]);

            // Set player to attacking
            modTypes.MapNpc[i].Attacking = 1;
            modTypes.MapNpc[i].AttackTimer = modGameLogic.GetTickCount();
            return;
        }

        // ::::::::::::::::::::::::::
        // :: Check for map packet ::
        // ::::::::::::::::::::::::::
        if (parse[0].Equals("checkformap", StringComparison.CurrentCultureIgnoreCase))
        {
            // Erase all players except self
            for (var i = 1; i <= modTypes.MAX_PLAYERS; i++)
            {
                if (i != modGameLogic.MyIndex)
                {
                    modTypes.SetPlayerMap(i, 0);
                }
            }

            // Erase all temporary tile values
            modTypes.ClearTempTile();

            // Get map num
            var mapNum = int.Parse(parse[1]);

            // Get revision
            var revision = int.Parse(parse[2]);

            // Check to see if map exist
            if (modDatabase.MapExist(mapNum))
            {
                // Check to see if the revisions match
                if (modDatabase.GetMapRevision(mapNum) == revision)
                {
                    // We do so we dont need the map

                    // Load the map
                    modDatabase.LoadMap(mapNum);

                    modClientTCP.SendData("needmap" + modTypes.SEP_CHAR + "no" + modTypes.SEP_CHAR);
                    return;
                }
            }

            // Either the revisions didn't match or we dont have the map, so we need it
            modClientTCP.SendData("needmap" + modTypes.SEP_CHAR + "yes" + modTypes.SEP_CHAR);
            return;
        }


        // :::::::::::::::::::::
        // :: Map data packet ::
        // :::::::::::::::::::::
        if (parse[0].Equals("mapdata", StringComparison.CurrentCultureIgnoreCase))
        {
            var mapNum = int.Parse(parse[1]);

            var n = 1;

            modGameLogic.SaveMap.Name = parse[n + 1];
            modGameLogic.SaveMap.Revision = int.Parse(parse[n + 2]);
            modGameLogic.SaveMap.Moral = int.Parse(parse[n + 3]);
            modGameLogic.SaveMap.Up = int.Parse(parse[n + 4]);
            modGameLogic.SaveMap.Down = int.Parse(parse[n + 5]);
            modGameLogic.SaveMap.Left = int.Parse(parse[n + 6]);
            modGameLogic.SaveMap.Right = int.Parse(parse[n + 7]);
            modGameLogic.SaveMap.Music = int.Parse(parse[n + 8]);
            modGameLogic.SaveMap.BootMap = int.Parse(parse[n + 9]);
            modGameLogic.SaveMap.BootX = int.Parse(parse[n + 10]);
            modGameLogic.SaveMap.BootY = int.Parse(parse[n + 11]);
            modGameLogic.SaveMap.Shop = int.Parse(parse[n + 12]);

            n += 13;

            for (var y = 0; y <= modTypes.MAX_MAPY; y++)
            {
                for (var x = 0; x <= modTypes.MAX_MAPX; x++)
                {
                    modGameLogic.SaveMap.Tile[x, y].Ground = int.Parse(parse[n]);
                    modGameLogic.SaveMap.Tile[x, y].Mask = int.Parse(parse[n + 1]);
                    modGameLogic.SaveMap.Tile[x, y].Anim = int.Parse(parse[n + 2]);
                    modGameLogic.SaveMap.Tile[x, y].Fringe = int.Parse(parse[n + 3]);
                    modGameLogic.SaveMap.Tile[x, y].Type = int.Parse(parse[n + 4]);
                    modGameLogic.SaveMap.Tile[x, y].Data1 = int.Parse(parse[n + 5]);
                    modGameLogic.SaveMap.Tile[x, y].Data2 = int.Parse(parse[n + 6]);
                    modGameLogic.SaveMap.Tile[x, y].Data3 = int.Parse(parse[n + 7]);

                    n += 8;
                }
            }

            for (var i = 1; i <= modTypes.MAX_MAP_NPCS; i++)
            {
                modGameLogic.SaveMap.Npc[i] = int.Parse(parse[n]);
                n++;
            }

            // Save the map
            modDatabase.SaveLocalMap(mapNum);

            // Check if we get a map from someone else and if we were editing a map cancel it out
            if (!modGameLogic.InEditor)
            {
                return;
            }

            modGameLogic.InEditor = false;
            My.Forms.frmMirage.picMapEditor.Visible = false;

            Application.OpenForms.OfType<frmMapWarp>().FirstOrDefault()?.Close();
            Application.OpenForms.OfType<frmMapProperties>().FirstOrDefault()?.Close();

            return;
        }

        // :::::::::::::::::::::::::::
        // :: Map items data packet ::
        // :::::::::::::::::::::::::::
        if (parse[0].Equals("mapitemdata", StringComparison.CurrentCultureIgnoreCase))
        {
            var n = 1;

            for (var i = 1; i <= modTypes.MAX_MAP_NPCS; i++)
            {
                modGameLogic.SaveMapItem[i].Num = int.Parse(parse[n]);
                modGameLogic.SaveMapItem[i].Value = int.Parse(parse[n + 1]);
                modGameLogic.SaveMapItem[i].Dur = int.Parse(parse[n + 2]);
                modGameLogic.SaveMapItem[i].X = int.Parse(parse[n + 3]);
                modGameLogic.SaveMapItem[i].Y = int.Parse(parse[n + 4]);

                n += 5;
            }

            return;
        }

        // :::::::::::::::::::::::::
        // :: Map npc data packet ::
        // :::::::::::::::::::::::::
        if (parse[0].Equals("mapnpcdata", StringComparison.CurrentCultureIgnoreCase))
        {
            var n = 1;

            for (var i = 1; i <= modTypes.MAX_MAP_NPCS; i++)
            {
                modGameLogic.SaveMapNpc[i].Num = int.Parse(parse[n]);
                modGameLogic.SaveMapNpc[i].X = int.Parse(parse[n + 1]);
                modGameLogic.SaveMapNpc[i].Y = int.Parse(parse[n + 2]);
                modGameLogic.SaveMapNpc[i].Dir = int.Parse(parse[n + 3]);
                n += 4;
            }

            return;
        }

        // :::::::::::::::::::::::::::::::
        // :: Map send completed packet ::
        // :::::::::::::::::::::::::::::::
        if (parse[0].Equals("mapdone", StringComparison.CurrentCultureIgnoreCase))
        {
            modTypes.Map = modGameLogic.SaveMap;

            for (var i = 1; i <= modTypes.MAX_MAP_ITEMS; i++)
            {
                modTypes.MapItem[i] = modGameLogic.SaveMapItem[i];
            }

            for (var i = 1; i <= modTypes.MAX_MAP_NPCS; i++)
            {
                modTypes.MapNpc[i] = modGameLogic.SaveMapNpc[i];
            }

            modGameLogic.GettingMap = false;

            // Play music
            modSound.StopMusic();
            if (modTypes.Map.Music > 0)
            {
                modSound.PlayOgg($"Music{modTypes.Map.Music}.ogg");
            }

            return;
        }

        // ::::::::::::::::::::
        // :: Social packets ::
        // ::::::::::::::::::::
        if (parse[0].Equals("saymsg", StringComparison.CurrentCultureIgnoreCase) ||
            parse[0].Equals("broadcastmsg", StringComparison.CurrentCultureIgnoreCase) ||
            parse[0].Equals("globalmsg", StringComparison.CurrentCultureIgnoreCase) ||
            parse[0].Equals("playermsg", StringComparison.CurrentCultureIgnoreCase) |
            parse[0].Equals("mapmsg", StringComparison.CurrentCultureIgnoreCase) ||
            parse[0].Equals("adminmsg", StringComparison.CurrentCultureIgnoreCase))
        {
            modText.AddText(parse[1], int.Parse(parse[2]));
            return;
        }

        // :::::::::::::::::::::::
        // :: Item spawn packet ::
        // :::::::::::::::::::::::
        if (parse[0].Equals("spawnitem", StringComparison.CurrentCultureIgnoreCase))
        {
            var n = int.Parse(parse[1]);

            modTypes.MapItem[n].Num = int.Parse(parse[2]);
            modTypes.MapItem[n].Value = int.Parse(parse[3]);
            modTypes.MapItem[n].Dur = int.Parse(parse[4]);
            modTypes.MapItem[n].X = int.Parse(parse[5]);
            modTypes.MapItem[n].Y = int.Parse(parse[6]);
            return;
        }

        // ::::::::::::::::::::::::
        // :: Item editor packet ::
        // ::::::::::::::::::::::::
        if (parse[0].Equals("itemeditor", StringComparison.CurrentCultureIgnoreCase))
        {
            modGameLogic.InItemsEditor = true;

            using var frmIndex = new frmIndex();

            for (var i = 1; i <= modTypes.MAX_INV; i++)
            {
                frmIndex.lstIndex.Items.Add($"{i}: {modTypes.Item[i].Name.Trim()}");
            }

            frmIndex.lstIndex.SelectedIndex = 0;
            frmIndex.ShowDialog();

            return;
        }

        // ::::::::::::::::::::::::
        // :: Update item packet ::
        // ::::::::::::::::::::::::
        if (parse[0].Equals("updateitem", StringComparison.CurrentCultureIgnoreCase))
        {
            var n = int.Parse(parse[1]);

            // Update the item
            modTypes.Item[n].Name = parse[2];
            modTypes.Item[n].Pic = int.Parse(parse[3]);
            modTypes.Item[n].Type = int.Parse(parse[4]);
            modTypes.Item[n].Data1 = 0;
            modTypes.Item[n].Data2 = 0;
            modTypes.Item[n].Data3 = 0;
            return;
        }

        // ::::::::::::::::::::::
        // :: Edit item packet :: <- Used for item editor admins only
        // ::::::::::::::::::::::
        if (parse[0].Equals("edititem", StringComparison.CurrentCultureIgnoreCase))
        {
            var n = int.Parse(parse[1]);

            // Update the item
            modTypes.Item[n].Name = parse[2];
            modTypes.Item[n].Pic = int.Parse(parse[3]);
            modTypes.Item[n].Type = int.Parse(parse[4]);
            modTypes.Item[n].Data1 = int.Parse(parse[5]);
            modTypes.Item[n].Data2 = int.Parse(parse[6]);
            modTypes.Item[n].Data3 = int.Parse(parse[7]);

            // Initialize the item editor
            using var frmItemEditor = new frmItemEditor();

            frmItemEditor.ShowDialog();

            return;
        }

        // ::::::::::::::::::::::
        // :: Npc spawn packet ::
        // ::::::::::::::::::::::
        if (parse[0].Equals("spawnnpc", StringComparison.CurrentCultureIgnoreCase))
        {
            var n = int.Parse(parse[1]);

            modTypes.MapNpc[n].Num = int.Parse(parse[2]);
            modTypes.MapNpc[n].X = int.Parse(parse[3]);
            modTypes.MapNpc[n].Y = int.Parse(parse[4]);
            modTypes.MapNpc[n].Dir = int.Parse(parse[5]);

            // Client use only
            modTypes.MapNpc[n].XOffset = 0;
            modTypes.MapNpc[n].YOffset = 0;
            modTypes.MapNpc[n].Moving = 0;
            return;
        }

        // :::::::::::::::::::::
        // :: Npc dead packet ::
        // :::::::::::::::::::::
        if (parse[0].Equals("npcdead", StringComparison.CurrentCultureIgnoreCase))
        {
            var n = int.Parse(parse[1]);

            modTypes.MapNpc[n].Num = 0;
            modTypes.MapNpc[n].X = 0;
            modTypes.MapNpc[n].Y = 0;
            modTypes.MapNpc[n].Dir = 0;

            // Client use only
            modTypes.MapNpc[n].XOffset = 0;
            modTypes.MapNpc[n].YOffset = 0;
            modTypes.MapNpc[n].Moving = 0;
            return;
        }

        // :::::::::::::::::::::::
        // :: Npc editor packet ::
        // :::::::::::::::::::::::
        if (parse[0].Equals("npceditor", StringComparison.CurrentCultureIgnoreCase))
        {
            modGameLogic.InNpcEditor = true;

            using var frmIndex = new frmIndex();

            // Add the names
            for (var i = 1; i <= modTypes.MAX_NPCS; i++)
            {
                frmIndex.lstIndex.Items.Add($"{i}: {modTypes.Npc[i].Name.Trim()}");
            }

            frmIndex.lstIndex.SelectedIndex = 0;
            frmIndex.ShowDialog();
        }

        // :::::::::::::::::::::::
        // :: Update npc packet ::
        // :::::::::::::::::::::::
        if (parse[0].Equals("updatenpc", StringComparison.CurrentCultureIgnoreCase))
        {
            var n = int.Parse(parse[1]);

            // Update the npc
            modTypes.Npc[n].Name = parse[2];
            modTypes.Npc[n].AttackSay = "";
            modTypes.Npc[n].Sprite = int.Parse(parse[3]);
            modTypes.Npc[n].SpawnSecs = 0;
            modTypes.Npc[n].Behavior = 0;
            modTypes.Npc[n].Range = 0;
            modTypes.Npc[n].DropChance = 0;
            modTypes.Npc[n].DropItem = 0;
            modTypes.Npc[n].DropItemValue = 0;
            modTypes.Npc[n].STR = 0;
            modTypes.Npc[n].DEF = 0;
            modTypes.Npc[n].SPEED = 0;
            modTypes.Npc[n].MAGI = 0;
            return;
        }

        // :::::::::::::::::::::
        // :: Edit npc packet :: <- Used for item editor admins only
        // :::::::::::::::::::::
        if (parse[0].Equals("editnpc", StringComparison.CurrentCultureIgnoreCase))
        {
            var n = int.Parse(parse[1]);

            // Update the npc
            modTypes.Npc[n].Name = parse[2];
            modTypes.Npc[n].AttackSay = parse[3];
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

            // Initialize the npc editor
            using var frmNpcEditor = new frmNpcEditor();

            frmNpcEditor.ShowDialog();

            return;
        }

        // ::::::::::::::::::::
        // :: Map key packet ::
        // ::::::::::::::::::::
        if (parse[0].Equals("mapkey", StringComparison.CurrentCultureIgnoreCase))
        {
            var x = int.Parse(parse[1]);
            var y = int.Parse(parse[2]);
            var n = int.Parse(parse[3]);

            modTypes.TempTile[x, y].DoorOpen = n;
            return;
        }

        // :::::::::::::::::::::
        // :: Edit map packet ::
        // :::::::::::::::::::::
        if (parse[0].Equals("editmap", StringComparison.CurrentCultureIgnoreCase))
        {
            modGameLogic.EditorInit();
            return;
        }

        // ::::::::::::::::::::::::
        // :: Shop editor packet ::
        // ::::::::::::::::::::::::
        if (parse[0].Equals("shopeditor", StringComparison.CurrentCultureIgnoreCase))
        {
            modGameLogic.InShopEditor = true;

            using var frmIndex = new frmIndex();

            // Add the names
            for (var i = 1; i <= modTypes.MAX_SHOPS; i++)
            {
                frmIndex.lstIndex.Items.Add($"{i}: {modTypes.Shop[i].Name.Trim()}");
            }

            frmIndex.lstIndex.SelectedIndex = 0;
            frmIndex.ShowDialog();
        }

        // ::::::::::::::::::::::::
        // :: Update shop packet ::
        // ::::::::::::::::::::::::
        if (parse[0].Equals("updateshop", StringComparison.CurrentCultureIgnoreCase))
        {
            var n = int.Parse(parse[1]);

            // Update the shop name
            modTypes.Shop[n].Name = parse[2];
            return;
        }

        // ::::::::::::::::::::::
        // :: Edit shop packet :: <- Used for shop editor admins only
        // ::::::::::::::::::::::
        if (parse[0].Equals("editshop", StringComparison.CurrentCultureIgnoreCase))
        {
            var shopNum = int.Parse(parse[1]);

            // Update the shop
            modTypes.Shop[shopNum].Name = parse[2];
            modTypes.Shop[shopNum].JoinSay = parse[3];
            modTypes.Shop[shopNum].LeaveSay = parse[4];
            modTypes.Shop[shopNum].FixesItems = int.Parse(parse[5]);

            var n = 6;
            for (var i = 1; i <= modTypes.MAX_TRADES; i++)
            {
                modTypes.Shop[shopNum].TradeItem[i].GiveItem = int.Parse(parse[n]);
                modTypes.Shop[shopNum].TradeItem[i].GiveValue = int.Parse(parse[n + 1]);
                modTypes.Shop[shopNum].TradeItem[i].GetItem = int.Parse(parse[n + 2]);
                modTypes.Shop[shopNum].TradeItem[i].GetValue = int.Parse(parse[n + 3]);

                n += 4;
            }

            // Initialize the shop editor
            using var frmShopEditor = new frmShopEditor();

            frmShopEditor.ShowDialog();

            return;
        }

        // :::::::::::::::::::::::::
        // :: Spell editor packet ::
        // :::::::::::::::::::::::::
        if (parse[0].Equals("spelleditor", StringComparison.CurrentCultureIgnoreCase))
        {
            modGameLogic.InSpellEditor = true;

            using var frmIndex = new frmIndex();

            // Add the names
            for (var i = 1; i <= modTypes.MAX_SPELLS; i++)
            {
                frmIndex.lstIndex.Items.Add($"{i}: {modTypes.Spell[i].Name.Trim()}");
            }

            frmIndex.lstIndex.SelectedIndex = 0;
            frmIndex.ShowDialog();
        }

        // :::::::::::::::::::::::::
        // :: Update spell packet ::
        // :::::::::::::::::::::::::
        if (parse[0].Equals("updatespell", StringComparison.CurrentCultureIgnoreCase))
        {
            var n = int.Parse(parse[1]);

            // Update the spell name
            modTypes.Spell[n].Name = parse[2];
            return;
        }

        // :::::::::::::::::::::::
        // :: Edit spell packet :: <- Used for spell editor admins only
        // :::::::::::::::::::::::
        if (parse[0].Equals("editspell", StringComparison.CurrentCultureIgnoreCase))
        {
            var n = int.Parse(parse[1]);

            // Update the spell
            modTypes.Spell[n].Name = parse[2];
            modTypes.Spell[n].ClassReq = int.Parse(parse[3]);
            modTypes.Spell[n].LevelReq = int.Parse(parse[4]);
            modTypes.Spell[n].Type = int.Parse(parse[5]);
            modTypes.Spell[n].Data1 = int.Parse(parse[6]);
            modTypes.Spell[n].Data2 = int.Parse(parse[7]);
            modTypes.Spell[n].Data3 = int.Parse(parse[8]);

            // Initialize the spell editor
            using var frmSpellEditor = new frmSpellEditor();

            frmSpellEditor.ShowDialog();

            return;
        }

        // ::::::::::::::::::
        // :: Trade packet ::
        // ::::::::::::::::::
        if (parse[0].Equals("trade", StringComparison.CurrentCultureIgnoreCase))
        {
            var shopNum = int.Parse(parse[1]);

            using var frmTrade = new frmTrade();

            frmTrade.picFixItems.Visible = int.Parse(parse[2]) == 1;

            var n = 3;
            for (var i = 1; i <= modTypes.MAX_TRADES; i++)
            {
                var giveItem = int.Parse(parse[n]);
                var giveValue = int.Parse(parse[n + 1]);
                var getItem = int.Parse(parse[n + 2]);
                var getValue = int.Parse(parse[n + 3]);

                if (giveItem > 0 && getItem > 0)
                {
                    frmTrade.lstTrade.Items.Add($"Give {modTypes.Shop[shopNum].Name.Trim()} {giveValue} {modTypes.Item[giveItem].Name.Trim()} for {getValue} {modTypes.Item[getItem].Name.Trim()}");
                }

                n += 4;
            }

            if (frmTrade.lstTrade.Items.Count > 0)
            {
                frmTrade.lstTrade.SelectedItem = 0;
            }

            frmTrade.ShowDialog();
            return;
        }

        // :::::::::::::::::::
        // :: Spells packet ::
        // :::::::::::::::::::
        if (parse[0].Equals("spells", StringComparison.CurrentCultureIgnoreCase))
        {
            My.Forms.frmMirage.picPlayerSpells.Visible = true;
            My.Forms.frmMirage.lstSpells.Items.Clear();

            // Put spells known in player record
            for (var i = 1; i <= modTypes.MAX_PLAYER_SPELLS; i++)
            {
                modTypes.Player[modGameLogic.MyIndex].Spell[i] = int.Parse(parse[i]);
                if (modTypes.Player[modGameLogic.MyIndex].Spell[i] != 0)
                {
                    My.Forms.frmMirage.lstSpells.Items.Add($"{i}: {modTypes.Spell[modTypes.Player[modGameLogic.MyIndex].Spell[i]].Name.Trim()}");
                }
                else
                {
                    My.Forms.frmMirage.lstSpells.Items.Add("<free spells slot>");
                }
            }

            My.Forms.frmMirage.lstSpells.SelectedIndex = 0;
            return;
        }

        // ::::::::::::::::::::
        // :: Weather packet ::
        // ::::::::::::::::::::
        if (parse[0].Equals("weather", StringComparison.CurrentCultureIgnoreCase))
        {
            modGameLogic.GameWeather = int.Parse(parse[1]);
            return;
        }

        // :::::::::::::::::
        // :: Time packet ::
        // :::::::::::::::::
        if (parse[0].Equals("time", StringComparison.CurrentCultureIgnoreCase))
        {
            modGameLogic.GameTime = int.Parse(parse[1]);
        }
    }
}