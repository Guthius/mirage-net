using System.Runtime.InteropServices;
using Mirage.Forms;
using Mirage.Windows;
using SFML.Graphics;
using SFML.System;
using Color = SFML.Graphics.Color;
using Image = System.Drawing.Image;
using ObjectDisposedException = System.ObjectDisposedException;

namespace Mirage.Modules;

public static partial class modGameLogic
{
    [LibraryImport("kernel32.dll", EntryPoint = "GetTickCount")]
    internal static partial int GetTickCount();

    // Menu states
    public const int MENU_STATE_NEWACCOUNT = 0;
    public const int MENU_STATE_DELACCOUNT = 1;
    public const int MENU_STATE_LOGIN = 2;
    public const int MENU_STATE_GETCHARS = 3;
    public const int MENU_STATE_NEWCHAR = 4;
    public const int MENU_STATE_ADDCHAR = 5;
    public const int MENU_STATE_DELCHAR = 6;
    public const int MENU_STATE_USECHAR = 7;
    public const int MENU_STATE_INIT = 8;

    // Speed moving vars
    public const int WALK_SPEED = 4;
    public const int RUN_SPEED = 8;

    // Game direction vars
    public static bool DirUp;
    public static bool DirDown;
    public static bool DirLeft;
    public static bool DirRight;
    public static bool ShiftDown;
    public static bool ControlDown;

    // Game text buffer
    public static string MyText = string.Empty;

    // Index of actual player
    public static int MyIndex;

    // Map animation #, used to keep track of what map animation is currently on
    public static byte MapAnim;
    public static int MapAnimTimer;

    // Used to freeze controls when getting a new map
    public static bool GettingMap = true;

    // Used to check if in editor or not and variables for use in editor
    public static bool InEditor;
    public static int EditorTileX;
    public static int EditorTileY;
    public static int EditorWarpMap;
    public static int EditorWarpX;
    public static int EditorWarpY;

    // Used for map item editor
    public static int ItemEditorNum;
    public static int ItemEditorValue;

    // Used for map key editor
    public static int KeyEditorNum;
    public static int KeyEditorTake;

    // Used for map key opene ditor
    public static int KeyOpenEditorX;
    public static int KeyOpenEditorY;

    // Map for local use
    public static modTypes.MapRec SaveMap = new();
    public static readonly modTypes.MapItemRec[] SaveMapItem = new modTypes.MapItemRec[modTypes.MAX_MAP_ITEMS + 1];
    public static readonly modTypes.MapNpcRec[] SaveMapNpc = new modTypes.MapNpcRec[modTypes.MAX_MAP_NPCS + 1];

    // Used for index based editors
    public static bool InItemsEditor;
    public static bool InNpcEditor;
    public static bool InShopEditor;
    public static bool InSpellEditor;
    public static int EditorIndex;

    // Game fps
    public static int GameFPS;

    // Used for atmosphere
    public static int GameWeather;
    public static int GameTime;

    public static void SetStatus(string caption)
    {
        My.Forms.frmSendGetData.SetStatus(caption);
    }

    public static void MenuState(int state)
    {
        My.Forms.frmSendGetData.Show();
        SetStatus("Connecting to server...");
        switch (state)
        {
            case MENU_STATE_NEWACCOUNT:
                My.Forms.frmNewAccount.Hide();
                if (modClientTCP.ConnectToServer())
                {
                    SetStatus("Connected, sending new account information...");
                    modClientTCP.SendNewAccount(My.Forms.frmNewAccount.txtName.Text, My.Forms.frmNewAccount.txtPassword.Text);
                }

                break;

            case MENU_STATE_DELACCOUNT:
                My.Forms.frmDeleteAccount.Hide();
                if (modClientTCP.ConnectToServer())
                {
                    SetStatus("Connected, sending account deletion request...");
                    modClientTCP.SendDelAccount(My.Forms.frmDeleteAccount.txtName.Text, My.Forms.frmDeleteAccount.txtPassword.Text);
                }

                break;

            case MENU_STATE_LOGIN:
                My.Forms.frmLogin.Hide();
                if (modClientTCP.ConnectToServer())
                {
                    SetStatus("Connected, sending login information...");
                    modClientTCP.SendLogin(My.Forms.frmLogin.txtName.Text, My.Forms.frmLogin.txtPassword.Text);
                }

                break;

            case MENU_STATE_NEWCHAR:
                My.Forms.frmChars.Hide();
                SetStatus("Connected, getting available classes...");
                modClientTCP.SendGetClasses();
                break;

            case MENU_STATE_ADDCHAR:
                My.Forms.frmNewChar.Hide();
                if (modClientTCP.ConnectToServer())
                {
                    SetStatus("Connected, sending character addition data...");

                    modClientTCP.SendAddChar(
                        My.Forms.frmNewChar.txtName.Text,
                        My.Forms.frmNewChar.optMale.Checked ? modTypes.SEX_MALE : modTypes.SEX_FEMALE,
                        My.Forms.frmNewChar.cmbClass.SelectedIndex,
                        My.Forms.frmChars.lstChars.SelectedIndex + 1);
                }

                break;

            case MENU_STATE_DELCHAR:
                My.Forms.frmChars.Hide();
                if (modClientTCP.ConnectToServer())
                {
                    SetStatus("Connected, sending character deletion request...");
                    modClientTCP.SendDelChar(My.Forms.frmChars.lstChars.SelectedIndex + 1);
                }

                break;

            case MENU_STATE_USECHAR:
                My.Forms.frmChars.Hide();
                if (modClientTCP.ConnectToServer())
                {
                    SetStatus("Connected, sending char data...");
                    modClientTCP.SendUseChar(My.Forms.frmChars.lstChars.SelectedIndex + 1);
                }

                break;
        }

        if (modClientTCP.IsConnected())
        {
            return;
        }

        try
        {
            My.Forms.frmMainMenu.Show();
            My.Forms.frmSendGetData.Hide();
        }
        catch (ObjectDisposedException)
        {
            MessageBox.Show(
                "Sorry, the server seems to be down. Please try to reconnect in a few minutes or visit https://mirageuniverse.com/",
                modTypes.GAME_NAME,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }

    public static void GameInit()
    {
        My.Forms.frmMirage.Show();
        My.Forms.frmSendGetData.Hide();

        modDirectX.InitDirectX();
    }

    public static void GameLoop()
    {
        // Set the focus
        My.Forms.frmMirage.picScreen.Focus();

        // Used for calculating fps
        var tickFps = GetTickCount();
        var fps = 0;

        while (modClientTCP.InGame)
        {
            var tick = GetTickCount();

            // Check to make sure we are still connected
            if (!modClientTCP.IsConnected()) modClientTCP.InGame = false;

            modDirectX.Renderer.Clear(Color.Blue);

            // Blit out tiles layers ground/anim1/anim2
            for (var y = 0; y <= modTypes.MAX_MAPY; y++)
            {
                for (var x = 0; x <= modTypes.MAX_MAPX; x++)
                {
                    BltTile(x, y);
                }
            }

            // Blit out the items
            for (var i = 1; i <= modTypes.MAX_MAP_ITEMS; i++)
            {
                if (modTypes.MapItem[i].Num > 0)
                {
                    BltItem(i);
                }
            }

            // Blit out the npcs
            for (var i = 1; i <= modTypes.MAX_MAP_NPCS; i++)
            {
                BltNpc(i);
            }

            // Blit out players
            var mapNum = modTypes.GetPlayerMap(MyIndex);
            for (var i = 1; i <= modTypes.MAX_PLAYERS; i++)
            {
                if (modClientTCP.IsPlaying(i) && modTypes.GetPlayerMap(i) == mapNum)
                {
                    BltPlayer(i);
                }
            }

            // Blit out tile layer fringe
            for (var y = 0; y <= modTypes.MAX_MAPY; y++)
            {
                for (var x = 0; x <= modTypes.MAX_MAPX; x++)
                {
                    BltFringeTile(x, y);
                }
            }

            // Draw player names
            for (var i = 1; i <= modTypes.MAX_PLAYERS; i++)
            {
                if (modClientTCP.IsPlaying(i) && modTypes.GetPlayerMap(i) == mapNum)
                {
                    BltPlayerName(i);
                }
            }

            // Blit out attribs if in editor
            if (InEditor)
            {
                for (var y = 0; y <= modTypes.MAX_MAPY; y++)
                {
                    for (var x = 0; x <= modTypes.MAX_MAPX; x++)
                    {
                        var type = modTypes.Map.Tile[x, y].Type;
                        switch (type)
                        {
                            case modTypes.TILE_TYPE_BLOCKED:
                                modText.DrawText(x * modTypes.PIC_X + 8, y * modTypes.PIC_Y + 8, "B", modText.BrightRed);
                                break;
                            case modTypes.TILE_TYPE_WALKABLE:
                                modText.DrawText(x * modTypes.PIC_X + 8, y * modTypes.PIC_Y + 8, "P", modText.Yellow);
                                break;
                            case modTypes.TILE_TYPE_WARP:
                                modText.DrawText(x * modTypes.PIC_X + 8, y * modTypes.PIC_Y + 8, "W", modText.BrightBlue);
                                break;
                            case modTypes.TILE_TYPE_ITEM:
                                modText.DrawText(x * modTypes.PIC_X + 8, y * modTypes.PIC_Y + 8, "I", modText.White);
                                break;
                            case modTypes.TILE_TYPE_NPCAVOID:
                                modText.DrawText(x * modTypes.PIC_X + 8, y * modTypes.PIC_Y + 8, "N", modText.White);
                                break;
                            case modTypes.TILE_TYPE_KEY:
                                modText.DrawText(x * modTypes.PIC_X + 8, y * modTypes.PIC_Y + 8, "K", modText.White);
                                break;
                            case modTypes.TILE_TYPE_KEYOPEN:
                                modText.DrawText(x * modTypes.PIC_X + 8, y * modTypes.PIC_Y + 8, "O", modText.White);
                                break;
                        }
                    }
                }
            }

            // Blit the text they are putting in
            modText.DrawText(0, (modTypes.MAX_MAPY + 1) * modTypes.PIC_Y - 20, MyText, modText.White);

            // Draw map name
            var width = modText.MeasureText(modTypes.Map.Name);

            if (modTypes.Map.Moral == modTypes.MAP_MORAL_NONE)
            {
                var x = (modTypes.MAX_MAPX + 1) * modTypes.PIC_X / 2 - width / 2;
                modText.DrawText(x, 8, modTypes.Map.Name, modText.BrightRed);
            }
            else
            {
                var x = (modTypes.MAX_MAPX + 1) * modTypes.PIC_X / 2 - width / 2;
                modText.DrawText(x, 8, modTypes.Map.Name, modText.White);
            }

            // Check if we are getting a map, and if we are tell them so
            if (GettingMap)
            {
                modText.DrawText(50, 50, "Receiving Map...", modText.BrightCyan);
            }

            modDirectX.Renderer.Display();

            // Check if player is trying to move
            CheckMovement();

            // Check to see if player is trying to attack
            CheckAttack();

            // Process player movements (actually move them)
            for (var i = 1; i <= modTypes.MAX_PLAYERS; i++)
            {
                if (modClientTCP.IsPlaying(i))
                {
                    ProcessMovement(i);
                }
            }

            // Process npc movements (actually move them)
            for (var i = 1; i <= modTypes.MAX_MAP_NPCS; i++)
            {
                if (modTypes.Map.Npc[i] > 0)
                {
                    ProcessNpcMovement(i);
                }
            }

            // Change map animation every 250 milliseconds
            if (GetTickCount() > MapAnimTimer + 250)
            {
                if (MapAnim == 0)
                {
                    MapAnim = 1;
                }
                else
                {
                    MapAnim = 0;
                }

                MapAnimTimer = GetTickCount();
            }

            // Lock fps
            while (GetTickCount() < tick + 50)
            {
                Application.DoEvents();
            }

            // Calculate fps
            if (GetTickCount() > tickFps + 1000)
            {
                GameFPS = fps;
                tickFps = GetTickCount();
                fps = 0;
            }
            else
            {
                fps++;
            }

            Application.DoEvents();
        }

        My.Forms.frmMirage.Hide();
        My.Forms.frmSendGetData.Show();
        SetStatus("Destroying game data...");

        // Shutdown the game
        GameDestroy();

        // Report disconnection if server disconnects
        if (!modClientTCP.IsConnected())
        {
            MessageBox.Show(
                $"Thank you for playing {modTypes.GAME_NAME}!",
                modTypes.GAME_NAME,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }

    public static void GameDestroy()
    {
        modClientTCP.InGame = false;

        Environment.Exit(0);
    }

    private static void BltTile(int x, int y, int tileId)
    {
        var sprite = new Sprite
        {
            Texture = modDirectX.DDSD_Tile,
            Position = new Vector2f(x * modTypes.PIC_X, y * modTypes.PIC_Y),
        };

        var ty = tileId / 7;
        var tx = tileId % 7;

        sprite.TextureRect = new IntRect(
            tx * modTypes.PIC_X,
            ty * modTypes.PIC_Y,
            modTypes.PIC_X,
            modTypes.PIC_Y);

        modDirectX.Renderer.Draw(sprite);
    }

    public static void BltTile(int x, int y)
    {
        BltTile(x, y, modTypes.Map.Tile[x, y].Ground);

        var anim1 = modTypes.Map.Tile[x, y].Mask;
        var anim2 = modTypes.Map.Tile[x, y].Anim;

        if (anim2 > 0)
        {
            if (MapAnim == 0)
            {
                if (anim1 > 0 && modTypes.TempTile[x, y].DoorOpen == modTypes.NO)
                {
                    BltTile(x, y, anim1);
                }
            }
            else
            {
                BltTile(x, y, anim2);
            }
        }
        else
        {
            if (anim1 > 0 && modTypes.TempTile[x, y].DoorOpen == modTypes.NO)
            {
                BltTile(x, y, anim1);
            }
        }
    }

    public static void BltItem(int itemNum)
    {
        ref var mapItem = ref modTypes.MapItem[itemNum];
        ref var item = ref modTypes.Item[mapItem.Num];

        var sprite = new Sprite
        {
            Texture = modDirectX.DDSD_Item,
            TextureRect = new IntRect(0, item.Pic * modTypes.PIC_Y, modTypes.PIC_X, modTypes.PIC_Y),
            Position = new Vector2f(mapItem.X * modTypes.PIC_X, mapItem.Y * modTypes.PIC_Y),
        };

        modDirectX.Renderer.Draw(sprite);
    }

    public static void BltFringeTile(int x, int y)
    {
        var fringe = modTypes.Map.Tile[x, y].Fringe;

        if (fringe > 0)
        {
            BltTile(x, y, fringe);
        }
    }

    public static void BltPlayer(int index)
    {
        // Check for animation
        var anim = 0;
        if (modTypes.Player[index].Attacking == 0)
        {
            switch (modTypes.GetPlayerDir(index))
            {
                case modTypes.DIR_UP:
                    if (modTypes.Player[index].YOffset < modTypes.PIC_Y / 2) anim = 1;
                    break;
                case modTypes.DIR_DOWN:
                    if (modTypes.Player[index].YOffset < modTypes.PIC_Y / 2 * -1) anim = 1;
                    break;
                case modTypes.DIR_LEFT:
                    if (modTypes.Player[index].XOffset < modTypes.PIC_X / 2) anim = 1;
                    break;
                case modTypes.DIR_RIGHT:
                    if (modTypes.Player[index].XOffset < modTypes.PIC_X / 2 * -1) anim = 1;
                    break;
            }
        }
        else
        {
            if (modTypes.Player[index].AttackTimer + 500 > GetTickCount())
            {
                anim = 2;
            }
        }

        // Check to see if we want to stop making him attack
        if (modTypes.Player[index].AttackTimer + 1000 < GetTickCount())
        {
            modTypes.Player[index].Attacking = 0;
            modTypes.Player[index].AttackTimer = 0;
        }

        var x = modTypes.GetPlayerX(index) * modTypes.PIC_X + modTypes.Player[index].XOffset;
        var y = modTypes.GetPlayerY(index) * modTypes.PIC_Y + modTypes.Player[index].YOffset - 4;

        // Check if its out of bounds because of the offset
        if (y < 0)
        {
            y = 0;
        }

        var sprite = new Sprite
        {
            Texture = modDirectX.DDSD_Sprite,
            TextureRect = new IntRect(
                (modTypes.GetPlayerDir(index) * 3 + anim) * modTypes.PIC_X,
                modTypes.GetPlayerSprite(index) * modTypes.PIC_Y,
                modTypes.PIC_X,
                modTypes.PIC_Y),
            Position = new Vector2f(x, y),
        };

        modDirectX.Renderer.Draw(sprite);
    }

    public static void BltPlayerName(int index)
    {
        // Check access level
        int color;
        if (modTypes.GetPlayerPK(index) == modTypes.NO)
        {
            color = modTypes.GetPlayerAccess(index) switch
            {
                0 => modText.White,
                1 => modText.DarkGrey,
                2 => modText.Cyan,
                3 => modText.Blue,
                4 => modText.Pink,
                _ => modText.White
            };
        }
        else
        {
            color = modText.BrightRed;
        }

        // Draw name
        var width = modText.MeasureText(modTypes.Player[index].Name);
        var x = modTypes.Player[index].X * modTypes.PIC_X + modTypes.Player[index].XOffset + modTypes.PIC_X / 2 - width / 2;
        var y = modTypes.Player[index].Y * modTypes.PIC_Y + modTypes.Player[index].YOffset - 24;

        modText.DrawText(x, y, modTypes.Player[index].Name, color);
    }

    public static void BltNpc(int mapNpcNum)
    {
        if (modTypes.MapNpc[mapNpcNum].Num <= 0)
        {
            return;
        }

        ref var npc = ref modTypes.Npc[modTypes.MapNpc[mapNpcNum].Num];

        // Check for animation
        var anim = 0;
        if (modTypes.MapNpc[mapNpcNum].Attacking == 0)
        {
            switch (modTypes.MapNpc[mapNpcNum].Dir)
            {
                case modTypes.DIR_UP:
                    if (modTypes.MapNpc[mapNpcNum].YOffset < modTypes.PIC_Y / 2) anim = 1;
                    break;
                case modTypes.DIR_DOWN:
                    if (modTypes.MapNpc[mapNpcNum].YOffset < modTypes.PIC_Y / 2 * -1) anim = 1;
                    break;
                case modTypes.DIR_LEFT:
                    if (modTypes.MapNpc[mapNpcNum].XOffset < modTypes.PIC_X / 2) anim = 1;
                    break;
                case modTypes.DIR_RIGHT:
                    if (modTypes.MapNpc[mapNpcNum].XOffset < modTypes.PIC_X / 2 * -1) anim = 1;
                    break;
            }
        }
        else
        {
            if (modTypes.MapNpc[mapNpcNum].AttackTimer + 500 > GetTickCount())
            {
                anim = 2;
            }
        }

        // Check to see if we want to stop making him attack
        if (modTypes.MapNpc[mapNpcNum].AttackTimer + 1000 < GetTickCount())
        {
            modTypes.MapNpc[mapNpcNum].Attacking = 0;
            modTypes.MapNpc[mapNpcNum].AttackTimer = 0;
        }

        var x = modTypes.MapNpc[mapNpcNum].X * modTypes.PIC_X + modTypes.MapNpc[mapNpcNum].XOffset;
        var y = modTypes.MapNpc[mapNpcNum].Y * modTypes.PIC_Y + modTypes.MapNpc[mapNpcNum].YOffset - 4;

        // Check if its out of bounds because of the offset
        if (y < 0)
        {
            y = 0;
        }

        var sprite = new Sprite
        {
            Texture = modDirectX.DDSD_Sprite,
            TextureRect = new IntRect(
                (modTypes.MapNpc[mapNpcNum].Dir * 3 + anim) * modTypes.PIC_X,
                npc.Sprite * modTypes.PIC_Y,
                modTypes.PIC_X,
                modTypes.PIC_Y),
            Position = new Vector2f(x, y),
        };

        modDirectX.Renderer.Draw(sprite);
    }

    public static void ProcessMovement(int index)
    {
        // Check if player is walking, and if so process moving them over
        if (modTypes.Player[index].Moving == modTypes.MOVING_WALKING)
        {
            switch (modTypes.GetPlayerDir(index))
            {
                case modTypes.DIR_UP:
                    modTypes.Player[index].YOffset -= WALK_SPEED;
                    break;
                case modTypes.DIR_DOWN:
                    modTypes.Player[index].YOffset += WALK_SPEED;
                    break;
                case modTypes.DIR_LEFT:
                    modTypes.Player[index].XOffset -= WALK_SPEED;
                    break;
                case modTypes.DIR_RIGHT:
                    modTypes.Player[index].XOffset += WALK_SPEED;
                    break;
            }

            // Check if completed walking over to the next tile
            if (modTypes.Player[index].XOffset == 0 && modTypes.Player[index].YOffset == 0)
            {
                modTypes.Player[index].Moving = 0;
            }
        }

        // Check if player is running, and if so process moving them over
        if (modTypes.Player[index].Moving == modTypes.MOVING_RUNNING)
        {
            switch (modTypes.GetPlayerDir(index))
            {
                case modTypes.DIR_UP:
                    modTypes.Player[index].YOffset -= RUN_SPEED;
                    break;
                case modTypes.DIR_DOWN:
                    modTypes.Player[index].YOffset += RUN_SPEED;
                    break;
                case modTypes.DIR_LEFT:
                    modTypes.Player[index].XOffset -= RUN_SPEED;
                    break;
                case modTypes.DIR_RIGHT:
                    modTypes.Player[index].XOffset += RUN_SPEED;
                    break;
            }

            // Check if completed walking over to the next tile
            if (modTypes.Player[index].XOffset == 0 && modTypes.Player[index].YOffset == 0)
            {
                modTypes.Player[index].Moving = 0;
            }
        }
    }

    public static void ProcessNpcMovement(int mapNpcNum)
    {
        // Check if player is walking, and if so process moving them over
        if (modTypes.MapNpc[mapNpcNum].Moving == modTypes.MOVING_WALKING)
        {
            switch (modTypes.MapNpc[mapNpcNum].Dir)
            {
                case modTypes.DIR_UP:
                    modTypes.MapNpc[mapNpcNum].YOffset -= WALK_SPEED;
                    break;
                case modTypes.DIR_DOWN:
                    modTypes.MapNpc[mapNpcNum].YOffset += WALK_SPEED;
                    break;
                case modTypes.DIR_LEFT:
                    modTypes.MapNpc[mapNpcNum].XOffset -= WALK_SPEED;
                    break;
                case modTypes.DIR_RIGHT:
                    modTypes.MapNpc[mapNpcNum].XOffset += WALK_SPEED;
                    break;
            }

            // Check if completed walking over to the next tile
            if (modTypes.MapNpc[mapNpcNum].XOffset == 0 && modTypes.MapNpc[mapNpcNum].YOffset == 0)
            {
                modTypes.MapNpc[mapNpcNum].Moving = 0;
            }
        }
    }

    public static void HandleKeypresses(char keyAscii)
    {
        var key = (Keys) keyAscii;

        switch (key)
        {
            // Handle when the player presses the return key
            case Keys.Return:
            {
                MyText = MyText.Trim();
                if (MyText.Length == 0)
                {
                    return;
                }

                switch (MyText[0])
                {
                    // Broadcast message
                    case '\'':
                    {
                        var message = MyText[1..];
                        if (message.Length > 0)
                        {
                            modClientTCP.BroadcastMsg(message);
                        }

                        MyText = "";
                        return;
                    }

                    // Emote message
                    case '-':
                    {
                        var message = MyText[1..];
                        if (message.Length > 0)
                        {
                            modClientTCP.EmoteMsg(message);
                        }

                        MyText = "";
                        return;
                    }

                    // Player message
                    case '!':
                    {
                        var message = MyText[1..];

                        // Get the desired player from the user text
                        var space = message.IndexOf(' ');
                        if (space != -1)
                        {
                            var name = message[..space];

                            message = message[(space + 1)..].Trim();

                            // Make sure they are actually sending something
                            if (name.Length > 0 && message.Length > 0)
                            {
                                // Send the message to the player
                                modClientTCP.PlayerMsg(message, name);
                            }
                            else
                            {
                                modText.AddText("Usage: !playername msghere", modText.AlertColor);
                            }
                        }

                        MyText = "";
                        return;
                    }
                }

                // Commands //

                // Help
                if (MyText.StartsWith("/help", StringComparison.OrdinalIgnoreCase))
                {
                    modText.AddText("Social commands:", modText.HelpColor);
                    modText.AddText("'msghere = Broadcast Message", modText.HelpColor);
                    modText.AddText("-msghere = Emote Message", modText.HelpColor);
                    modText.AddText("!namehere msghere = Player Message", modText.HelpColor);
                    modText.AddText("Available Commands: /help, /info, /who, /fps, /inv, /stats, /train, /trade, /party, /join, /leave", modText.HelpColor);
                    MyText = "";
                    return;
                }

                // Verification User
                if (MyText.StartsWith("/info", StringComparison.OrdinalIgnoreCase))
                {
                    var name = MyText[6..].Trim();
                    if (name.Length > 0)
                    {
                        modClientTCP.SendData("playerinforequest" + modTypes.SEP_CHAR + name + modTypes.SEP_CHAR);
                    }

                    MyText = "";
                    return;
                }

                // Whos online
                if (MyText.StartsWith("/who", StringComparison.OrdinalIgnoreCase))
                {
                    modClientTCP.SendWhosOnline();
                    MyText = "";
                    return;
                }

                // Checking fps
                if (MyText.StartsWith("/fps", StringComparison.OrdinalIgnoreCase))
                {
                    modText.AddText($"FPS: {GameFPS}", modText.Pink);
                    MyText = "";
                    return;
                }

                // Show inventory
                if (MyText.StartsWith("/inv", StringComparison.OrdinalIgnoreCase))
                {
                    UpdateInventory();
                    My.Forms.frmMirage.picInv.Visible = true;
                    MyText = "";
                    return;
                }

                // Request stats
                if (MyText.StartsWith("/stats", StringComparison.OrdinalIgnoreCase))
                {
                    modClientTCP.SendData("getstats" + modTypes.SEP_CHAR);
                    MyText = "";
                    return;
                }

                // Show training
                if (MyText.StartsWith("/train", StringComparison.OrdinalIgnoreCase))
                {
                    var frmTraining = new WIndowTraining();
                    frmTraining.ShowDialog();
                    MyText = "";
                    return;
                }

                // Request trade
                if (MyText.StartsWith("/trade", StringComparison.OrdinalIgnoreCase))
                {
                    modClientTCP.SendData("trade" + modTypes.SEP_CHAR);
                    MyText = "";
                    return;
                }

                // Party request
                if (MyText.StartsWith("/party", StringComparison.OrdinalIgnoreCase))
                {
                    // Make sure they are actually sending something
                    if (MyText.Length > 7)
                    {
                        var name = MyText[7..].Trim();
                        modClientTCP.SendPartyRequest(name);
                    }
                    else
                    {
                        modText.AddText("Usage: /party playernamehere", modText.AlertColor);
                    }

                    MyText = "";
                    return;
                }

                // Join party
                if (MyText.StartsWith("/join", StringComparison.OrdinalIgnoreCase))
                {
                    modClientTCP.SendJoinParty();
                    MyText = "";
                    return;
                }

                // Leave party
                if (MyText.StartsWith("/leave", StringComparison.OrdinalIgnoreCase))
                {
                    modClientTCP.SendLeaveParty();
                    MyText = "";
                    return;
                }

                // Moniter Admin Commands
                if (modTypes.GetPlayerAccess(MyIndex) > 0)
                {
                    // Admin Help
                    if (MyText.StartsWith("/admin", StringComparison.OrdinalIgnoreCase))
                    {
                        modText.AddText("Social Commands:", modText.HelpColor);
                        modText.AddText("\"msghere = Global Admin Message", modText.HelpColor);
                        modText.AddText("=msghere = Private Admin Message", modText.HelpColor);
                        modText.AddText("Available Commands: /admin, /loc, /mapeditor, /warpmeto, /warptome, /warpto, /setsprite, /mapreport, /kick, /ban, /edititem, /respawn, /editnpc, /motd, /editshop, /ban, /editspell", modText.HelpColor);
                        MyText = "";
                        return;
                    }

                    // Kicking a player
                    if (MyText.StartsWith("/kick", StringComparison.OrdinalIgnoreCase))
                    {
                        if (MyText.Length > 6)
                        {
                            var name = MyText[6..].Trim();
                            modClientTCP.SendKick(name);
                        }

                        MyText = "";
                        return;
                    }

                    switch (MyText[0])
                    {
                        // Global Message
                        case '"':
                        {
                            MyText = MyText[1..].Trim();
                            if (MyText.Length > 0)
                            {
                                modClientTCP.GlobalMsg(MyText);
                            }

                            MyText = "";
                            return;
                        }

                        // Admin Message
                        case '=':
                        {
                            MyText = MyText[1..].Trim();
                            if (MyText.Length > 0)
                            {
                                modClientTCP.AdminMsg(MyText);
                            }

                            MyText = "";
                            return;
                        }
                    }
                }

                // Mapper Admin Commands
                if (modTypes.GetPlayerAccess(MyIndex) >= modTypes.ADMIN_MAPPER)
                {
                    // Location
                    if (MyText.StartsWith("/loc", StringComparison.OrdinalIgnoreCase))
                    {
                        modClientTCP.SendRequestLocation();
                        MyText = "";
                        return;
                    }

                    // Map Editor
                    if (MyText.StartsWith("/mapeditor", StringComparison.OrdinalIgnoreCase))
                    {
                        modClientTCP.SendRequestEditMap();
                        MyText = "";
                        return;
                    }

                    // Warping to a player
                    if (MyText.StartsWith("/warpmeto", StringComparison.OrdinalIgnoreCase))
                    {
                        if (MyText.Length > 10)
                        {
                            var name = MyText[10..].Trim();
                            if (name.Length > 0)
                            {
                                modClientTCP.WarpMeTo(name);
                            }
                        }

                        MyText = "";
                        return;
                    }

                    // Warping a player to you
                    if (MyText.StartsWith("/warptome", StringComparison.OrdinalIgnoreCase))
                    {
                        if (MyText.Length > 10)
                        {
                            var name = MyText[10..].Trim();
                            if (name.Length > 0)
                            {
                                modClientTCP.WarpToMe(name);
                            }
                        }

                        MyText = "";
                        return;
                    }

                    // Warping to a map
                    if (MyText.StartsWith("/warpto", StringComparison.OrdinalIgnoreCase))
                    {
                        if (MyText.Length > 8)
                        {
                            var str = MyText[8..].Trim();
                            if (int.TryParse(str, out var mapNum))
                            {
                                // Check to make sure its a valid map #
                                if (mapNum is > 0 and <= modTypes.MAX_MAPS)
                                {
                                    modClientTCP.WarpTo(mapNum);
                                }
                                else
                                {
                                    modText.AddText("Invalid map number.", modText.Red);
                                }
                            }
                        }

                        MyText = "";
                        return;
                    }

                    // Setting sprite
                    if (MyText.StartsWith("/setsprite", StringComparison.OrdinalIgnoreCase))
                    {
                        if (MyText.Length > 11)
                        {
                            var str = MyText[11..].Trim();
                            if (int.TryParse(str, out var sprite))
                            {
                                modClientTCP.SendSetSprite(sprite);
                            }
                        }

                        MyText = "";
                        return;
                    }

                    // Map report
                    if (MyText.StartsWith("/mapreport", StringComparison.OrdinalIgnoreCase))
                    {
                        modClientTCP.SendData("mapreport" + modTypes.SEP_CHAR);
                        MyText = "";
                        return;
                    }

                    // Respawn request
                    if (MyText.StartsWith("/respawn", StringComparison.OrdinalIgnoreCase))
                    {
                        modClientTCP.SendMapRespawn();
                        MyText = "";
                        return;
                    }

                    // MOTD change
                    if (MyText.StartsWith("/motd"))
                    {
                        if (MyText.Length > 6)
                        {
                            var motd = MyText[6..].Trim();
                            if (motd.Length > 0)
                            {
                                modClientTCP.SendMOTDChange(motd);
                            }
                        }

                        MyText = "";
                        return;
                    }

                    // Check the ban list
                    if (MyText.StartsWith("/banlist", StringComparison.OrdinalIgnoreCase))
                    {
                        modClientTCP.SendBanList();
                        MyText = "";
                        return;
                    }

                    // Banning a player
                    if (MyText.StartsWith("/ban"))
                    {
                        if (MyText.Length > 5)
                        {
                            var name = MyText[5..].Trim();
                            if (name.Length > 0)
                            {
                                modClientTCP.SendBan(name);
                            }
                        }

                        MyText = "";
                        return;
                    }
                }

                // // Developer Admin Commands //
                if (modTypes.GetPlayerAccess(MyIndex) >= modTypes.ADMIN_DEVELOPER)
                {
                    // Editing item request
                    if (MyText.StartsWith("/edititem"))
                    {
                        modClientTCP.SendRequestEditItem();
                        MyText = "";
                        return;
                    }

                    // Editing npc request
                    if (MyText.StartsWith("/editnpc"))
                    {
                        modClientTCP.SendRequestEditNpc();
                        MyText = "";
                        return;
                    }

                    // Editing shop request
                    if (MyText.StartsWith("/editshop"))
                    {
                        modClientTCP.SendRequestEditShop();
                        MyText = "";
                        return;
                    }

                    // Editing spell request
                    if (MyText.StartsWith("/editspell"))
                    {
                        modClientTCP.SendRequestEditSpell();
                        MyText = "";
                        return;
                    }
                }

                // // Creator Admin Commands //
                if (modTypes.GetPlayerAccess(MyIndex) >= modTypes.ADMIN_CREATOR)
                {
                    // Giving another player access
                    if (MyText.StartsWith("/setaccess"))
                    {
                        // Get access #
                        var i = int.Parse(MyText.Substring(11, 1));

                        MyText = MyText[13..];

                        modClientTCP.SendSetAccess(MyText, i);
                        MyText = "";
                        return;
                    }

                    // Ban destroy
                    if (MyText.StartsWith("/destroybanlist"))
                    {
                        modClientTCP.SendBanDestroy();
                        MyText = "";
                        return;
                    }
                }

                // Say message
                if (MyText.Trim().Length > 0)
                {
                    modClientTCP.SayMsg(MyText);
                }

                MyText = "";
                return;
            }
            // Handle when the user presses the backspace key
            case Keys.Back:
            {
                if (MyText.Length > 0)
                {
                    MyText = MyText[..^1];
                }

                return;
            }
        }

        // Make sure they just use standard keys, no gay shitty macro keys
        if (keyAscii >= 32 && keyAscii <= 126)
        {
            MyText += keyAscii;
        }
    }

    public static void CheckMapGetItem()
    {
        if (GetTickCount() > modTypes.Player[MyIndex].MapGetTimer + 250 && MyText.Trim() == "")
        {
            modTypes.Player[MyIndex].MapGetTimer = GetTickCount();
            modClientTCP.SendData("mapgetitem" + modTypes.SEP_CHAR);
        }
    }

    public static void CheckAttack()
    {
        if (ControlDown && modTypes.Player[MyIndex].AttackTimer + 1000 < GetTickCount() && modTypes.Player[MyIndex].Attacking == 0)
        {
            modTypes.Player[MyIndex].Attacking = 1;
            modTypes.Player[MyIndex].AttackTimer = GetTickCount();
            modClientTCP.SendData("attack" + modTypes.SEP_CHAR);
        }
    }

    public static void CheckInput(bool keyDown, Keys keyCode)
    {
        if (GettingMap)
        {
            return;
        }

        if (keyDown)
        {
            switch (keyCode)
            {
                case Keys.Return:
                    CheckMapGetItem();
                    break;

                case Keys.ControlKey:
                    ControlDown = true;
                    break;

                case Keys.Up:
                    DirUp = true;
                    DirDown = false;
                    DirLeft = false;
                    DirRight = false;
                    break;

                case Keys.Down:
                    DirUp = false;
                    DirDown = true;
                    DirLeft = false;
                    DirRight = false;
                    break;

                case Keys.Left:
                    DirUp = false;
                    DirDown = false;
                    DirLeft = true;
                    DirRight = false;
                    break;

                case Keys.Right:
                    DirUp = false;
                    DirDown = false;
                    DirLeft = false;
                    DirRight = true;
                    break;

                case Keys.ShiftKey:
                    ShiftDown = true;
                    break;
            }
        }
        else
        {
            switch (keyCode)
            {
                case Keys.Up:
                    DirUp = false;
                    break;

                case Keys.Down:
                    DirDown = false;
                    break;

                case Keys.Left:
                    DirLeft = false;
                    break;

                case Keys.Right:
                    DirRight = false;
                    break;

                case Keys.ShiftKey:
                    ShiftDown = false;
                    break;

                case Keys.ControlKey:
                    ControlDown = false;
                    break;
            }
        }
    }

    public static bool IsTryingToMove()
    {
        return DirUp || DirDown || DirLeft || DirRight;
    }

    public static bool CanMove()
    {
        // Make sure they aren't trying to move when they are already moving
        if (modTypes.Player[MyIndex].Moving != 0)
        {
            return false;
        }

        // Make sure they haven't just casted a spell
        if (modTypes.Player[MyIndex].CastedSpell == modTypes.YES)
        {
            if (GetTickCount() > modTypes.Player[MyIndex].AttackTimer + 1000)
            {
                modTypes.Player[MyIndex].CastedSpell = modTypes.NO;
            }
            else
            {
                return false;
            }
        }

        var curMap = modTypes.GetPlayerMap(MyIndex);
        var curX = modTypes.GetPlayerX(MyIndex);
        var curY = modTypes.GetPlayerY(MyIndex);
        var curDir = modTypes.GetPlayerDir(MyIndex);

        int destMap;
        var destX = curX;
        var destY = curY;
        int destDir;

        if (DirUp)
        {
            destMap = modTypes.Map.Up;
            destY--;
            destDir = modTypes.DIR_UP;
        }
        else if (DirDown)
        {
            destMap = modTypes.Map.Down;
            destY++;
            destDir = modTypes.DIR_DOWN;
        }
        else if (DirLeft)
        {
            destMap = modTypes.Map.Left;
            destX--;
            destDir = modTypes.DIR_LEFT;
        }

        else if (DirRight)
        {
            destMap = modTypes.Map.Right;
            destX++;
            destDir = modTypes.DIR_RIGHT;
        }
        else
        {
            return false;
        }

        modTypes.SetPlayerDir(MyIndex, destDir);

        // Check if they are trying to move out of bounds
        if (destX < 0 || destY < 0 || destX > modTypes.MAX_MAPX || destY > modTypes.MAX_MAPY)
        {
            if (curDir != destDir)
            {
                modClientTCP.SendPlayerDir();
            }

            if (curMap == destMap || destMap <= 0)
            {
                return false;
            }

            modClientTCP.SendPlayerRequestNewMap();

            GettingMap = true;

            return false;
        }

        switch (modTypes.Map.Tile[destX, destY].Type)
        {
            // Check to see if the destination tile is blocked or a closed door.
            case modTypes.TILE_TYPE_BLOCKED:
            case modTypes.TILE_TYPE_KEY when modTypes.TempTile[destX, destY].DoorOpen == modTypes.NO:
            {
                if (curDir != destDir)
                {
                    modClientTCP.SendPlayerDir();
                }

                return false;
            }
        }

        // Check whether a player occupies the target tile.
        for (var i = 1; i <= modTypes.MAX_PLAYERS; i++)
        {
            if (!modClientTCP.IsPlaying(i) || modTypes.GetPlayerMap(i) != curMap)
            {
                continue;
            }

            var px = modTypes.GetPlayerX(i);
            var py = modTypes.GetPlayerY(i);

            if (px != destX || py != destY)
            {
                continue;
            }

            if (curDir != destDir)
            {
                modClientTCP.SendPlayerDir();
            }

            return false;
        }

        // Check whether a NPC occupies the target tile.
        for (var i = 1; i <= modTypes.MAX_MAP_NPCS; i++)
        {
            if (modTypes.MapNpc[i].Num <= 0)
            {
                continue;
            }

            if (modTypes.MapNpc[i].X != destX || modTypes.MapNpc[i].Y != destY)
            {
                continue;
            }

            if (curDir != modTypes.DIR_UP)
            {
                modClientTCP.SendPlayerDir();
            }

            return false;
        }

        return true;
    }

    public static void CheckMovement()
    {
        if (GettingMap)
        {
            return;
        }

        if (!IsTryingToMove() || !CanMove())
        {
            return;
        }

        // Check if player has the shift key down for running
        modTypes.Player[MyIndex].Moving = ShiftDown ? modTypes.MOVING_RUNNING : modTypes.MOVING_WALKING;

        switch (modTypes.GetPlayerDir(MyIndex))
        {
            case modTypes.DIR_UP:
                modClientTCP.SendPlayerMove();
                modTypes.Player[MyIndex].YOffset = modTypes.PIC_Y;
                modTypes.SetPlayerY(MyIndex, modTypes.GetPlayerY(MyIndex) - 1);
                break;

            case modTypes.DIR_DOWN:
                modClientTCP.SendPlayerMove();
                modTypes.Player[MyIndex].YOffset = modTypes.PIC_Y * -1;
                modTypes.SetPlayerY(MyIndex, modTypes.GetPlayerY(MyIndex) + 1);
                break;

            case modTypes.DIR_LEFT:
                modClientTCP.SendPlayerMove();
                modTypes.Player[MyIndex].XOffset = modTypes.PIC_X;
                modTypes.SetPlayerX(MyIndex, modTypes.GetPlayerX(MyIndex) - 1);
                break;

            case modTypes.DIR_RIGHT:
                modClientTCP.SendPlayerMove();
                modTypes.Player[MyIndex].XOffset = modTypes.PIC_X * -1;
                modTypes.SetPlayerX(MyIndex, modTypes.GetPlayerX(MyIndex) + 1);
                break;
        }

        // Gotta check :)
        if (modTypes.Map.Tile[modTypes.GetPlayerX(MyIndex), modTypes.GetPlayerY(MyIndex)].Type == modTypes.TILE_TYPE_WARP)
        {
            GettingMap = true;
        }
    }

    public static int FindPlayer(string name)
    {
        name = name.Trim();

        for (var i = 1; i <= modTypes.MAX_PLAYERS; i++)
        {
            if (!modClientTCP.IsPlaying(i))
            {
                continue;
            }

            if (modTypes.GetPlayerName(i).Length < name.Length)
            {
                continue;
            }

            if (modTypes.GetPlayerName(i)[..name.Length].Equals(name, StringComparison.CurrentCultureIgnoreCase))
            {
                return i;
            }
        }

        return 0;
    }

    public static void EditorInit()
    {
        SaveMap = modTypes.Map;
        InEditor = true;
        My.Forms.frmMirage.picMapEditor.Visible = true;
        My.Forms.frmMirage.Size = My.Forms.frmMirage.Size with {Width = My.Forms.frmMirage.picMapEditor.Right + 16};
        My.Forms.frmMirage.picBackSelect.Visible = true;
        My.Forms.frmMirage.picBackSelect.Width = 7 * modTypes.PIC_X;
        My.Forms.frmMirage.picBackSelect.Height = 255 * modTypes.PIC_Y;
        My.Forms.frmMirage.picBackSelect.Image = Image.FromFile("Assets/Tiles.png");
    }

    public static void EditorMouseDown(MouseButtons button, int x, int y)
    {
        if (!InEditor)
        {
            return;
        }

        var tileX = x / modTypes.PIC_X;
        var tileY = y / modTypes.PIC_Y;

        if (tileX is < 0 or > modTypes.MAX_MAPX || tileY is < 0 or > modTypes.MAX_MAPY)
        {
            return;
        }

        ref var tile = ref modTypes.Map.Tile[tileX, tileY];

        switch (button)
        {
            case MouseButtons.Left when My.Forms.frmMirage.optLayers.Checked:
            {
                if (My.Forms.frmMirage.optGround.Checked) tile.Ground = EditorTileY * 7 + EditorTileX;
                if (My.Forms.frmMirage.optMask.Checked) tile.Mask = EditorTileY * 7 + EditorTileX;
                if (My.Forms.frmMirage.optAnim.Checked) tile.Anim = EditorTileY * 7 + EditorTileX;
                if (My.Forms.frmMirage.optFringe.Checked) tile.Fringe = EditorTileY * 7 + EditorTileX;
                break;
            }

            case MouseButtons.Left:
            {
                if (My.Forms.frmMirage.optBlocked.Checked) tile.Type = modTypes.TILE_TYPE_BLOCKED;
                if (My.Forms.frmMirage.optPass.Checked) tile.Type = modTypes.TILE_TYPE_WALKABLE;
                if (My.Forms.frmMirage.optWarp.Checked)
                {
                    tile.Type = modTypes.TILE_TYPE_WARP;
                    tile.Data1 = EditorWarpMap;
                    tile.Data2 = EditorWarpX;
                    tile.Data3 = EditorWarpY;
                }

                if (My.Forms.frmMirage.optItem.Checked)
                {
                    tile.Type = modTypes.TILE_TYPE_ITEM;
                    tile.Data1 = ItemEditorNum;
                    tile.Data2 = ItemEditorValue;
                    tile.Data3 = 0;
                }

                if (My.Forms.frmMirage.optNpcAvoid.Checked)
                {
                    tile.Type = modTypes.TILE_TYPE_NPCAVOID;
                    tile.Data1 = 0;
                    tile.Data2 = 0;
                    tile.Data3 = 0;
                }

                if (My.Forms.frmMirage.optKey.Checked)
                {
                    tile.Type = modTypes.TILE_TYPE_KEY;
                    tile.Data1 = KeyEditorNum;
                    tile.Data2 = KeyEditorTake;
                    tile.Data3 = 0;
                }

                if (My.Forms.frmMirage.optKeyOpen.Checked)
                {
                    tile.Type = modTypes.TILE_TYPE_KEYOPEN;
                    tile.Data1 = KeyOpenEditorX;
                    tile.Data2 = KeyOpenEditorY;
                    tile.Data3 = 0;
                }

                break;
            }

            case MouseButtons.Right when My.Forms.frmMirage.optLayers.Checked:
            {
                if (My.Forms.frmMirage.optGround.Checked) tile.Ground = 0;
                if (My.Forms.frmMirage.optMask.Checked) tile.Mask = 0;
                if (My.Forms.frmMirage.optAnim.Checked) tile.Anim = 0;
                if (My.Forms.frmMirage.optFringe.Checked) tile.Fringe = 0;
                break;
            }

            case MouseButtons.Right:
                tile.Type = 0;
                tile.Data1 = 0;
                tile.Data2 = 0;
                tile.Data3 = 0;
                break;
        }
    }

    public static void EditorChooseTitle(MouseButtons button, int x, int y)
    {
        if (button != MouseButtons.Left)
        {
            return;
        }

        EditorTileX = x / modTypes.PIC_X;
        EditorTileY = y / modTypes.PIC_Y;

        var bitmap = new Bitmap(32, 32);

        using (var g = Graphics.FromImage(bitmap))
        {
            g.DrawImage(My.Forms.frmMirage.picBackSelect.Image!,
                new Rectangle(0, 0, modTypes.PIC_X, modTypes.PIC_Y),
                new Rectangle(
                    EditorTileX * modTypes.PIC_X,
                    EditorTileY * modTypes.PIC_Y,
                    modTypes.PIC_X, modTypes.PIC_Y),
                GraphicsUnit.Pixel);
        }

        My.Forms.frmMirage.picSelect.Image = bitmap;
    }

    public static void EditorTileScroll()
    {
        My.Forms.frmMirage.picBackSelect.Top = My.Forms.frmMirage.scrlPicture.Value * modTypes.PIC_Y * -1;
    }

    public static void EditorSend()
    {
        modClientTCP.SendMap();
        EditorCancel();
    }

    public static void EditorCancel()
    {
        modTypes.Map = SaveMap;
        InEditor = false;
        My.Forms.frmMirage.Size = My.Forms.frmMirage.Size with {Width = My.Forms.frmMirage.picGUI.Right + 16};
        My.Forms.frmMirage.picMapEditor.Visible = false;
        My.Forms.frmMirage.picBackSelect.Visible = false;
        My.Forms.frmMirage.picScreen.Focus();
    }

    public static void EditorClearLayer()
    {
        // Ground layer
        if (My.Forms.frmMirage.optGround.Checked)
        {
            var yesNo = MessageBox.Show(
                "Are you sure you wish to clear the ground layer?",
                modTypes.GAME_NAME,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (yesNo != DialogResult.Yes)
            {
                return;
            }

            for (var y = 0; y <= modTypes.MAX_MAPY; y++)
            {
                for (var x = 0; x <= modTypes.MAX_MAPX; x++)
                {
                    modTypes.Map.Tile[x, y].Ground = 0;
                }
            }
        }

        // Mask layer
        if (My.Forms.frmMirage.optMask.Checked)
        {
            var yesNo = MessageBox.Show(
                "Are you sure you wish to clear the mask layer?",
                modTypes.GAME_NAME,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (yesNo != DialogResult.Yes)
            {
                return;
            }

            for (var y = 0; y <= modTypes.MAX_MAPY; y++)
            {
                for (var x = 0; x <= modTypes.MAX_MAPX; x++)
                {
                    modTypes.Map.Tile[x, y].Mask = 0;
                }
            }
        }

        // Animation layer
        if (My.Forms.frmMirage.optAnim.Checked)
        {
            var yesNo = MessageBox.Show(
                "Are you sure you wish to clear the animation layer?",
                modTypes.GAME_NAME,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (yesNo != DialogResult.Yes)
            {
                return;
            }

            for (var y = 0; y <= modTypes.MAX_MAPY; y++)
            {
                for (var x = 0; x <= modTypes.MAX_MAPX; x++)
                {
                    modTypes.Map.Tile[x, y].Anim = 0;
                }
            }
        }

        // Fringe layer
        if (My.Forms.frmMirage.optFringe.Checked)
        {
            var yesNo = MessageBox.Show(
                "Are you sure you wish to clear the fringe layer?",
                modTypes.GAME_NAME,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (yesNo != DialogResult.Yes)
            {
                return;
            }

            for (var y = 0; y <= modTypes.MAX_MAPY; y++)
            {
                for (var x = 0; x <= modTypes.MAX_MAPX; x++)
                {
                    modTypes.Map.Tile[x, y].Fringe = 0;
                }
            }
        }
    }

    public static void EditorClearAttribs()
    {
        var yesNo = MessageBox.Show(
            "Are you sure you wish to clear the attributes on this map?",
            modTypes.GAME_NAME,
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (yesNo != DialogResult.Yes)
        {
            return;
        }

        for (var y = 0; y <= modTypes.MAX_MAPY; y++)
        {
            for (var x = 0; x <= modTypes.MAX_MAPX; x++)
            {
                modTypes.Map.Tile[x, y].Type = modTypes.TILE_TYPE_WALKABLE;
            }
        }
    }

    public static void UpdateInventory()
    {
        My.Forms.frmMirage.lstInv.Items.Clear();

        // Show the inventory
        for (var i = 1; i <= modTypes.MAX_INV; i++)
        {
            var itemNum = modTypes.GetPlayerInvItemNum(MyIndex, i);
            if (itemNum is > 0 and <= modTypes.MAX_ITEMS)
            {
                if (modTypes.Item[itemNum].Type == modTypes.ITEM_TYPE_CURRENCY)
                {
                    My.Forms.frmMirage.lstInv.Items.Add($"{i}: {modTypes.Item[itemNum].Name} ({modTypes.GetPlayerInvItemValue(MyIndex, itemNum)})");
                }
                else
                {
                    // Check if this item is being worn
                    if (modTypes.GetPlayerWeaponSlot(MyIndex) == i || modTypes.GetPlayerArmorSlot(MyIndex) == i || modTypes.GetPlayerHelmetSlot(MyIndex) == i || modTypes.GetPlayerShieldSlot(MyIndex) == i)
                    {
                        My.Forms.frmMirage.lstInv.Items.Add($"{i}: {modTypes.Item[itemNum].Name} (worn)");
                    }
                    else
                    {
                        My.Forms.frmMirage.lstInv.Items.Add($"{i}: {modTypes.Item[itemNum].Name}");
                    }
                }
            }
            else
            {
                My.Forms.frmMirage.lstInv.Items.Add("<free inventory slot>");
            }
        }

        My.Forms.frmMirage.lstInv.SelectedIndex = 0;
    }

    public static void PlayerSearch(int x, int y)
    {
        var x1 = x / modTypes.PIC_X;
        var y1 = y / modTypes.PIC_Y;

        if (x1 is > 0 and <= modTypes.MAX_MAPX && y1 is >= 0 and <= modTypes.MAX_MAPY)
        {
            modClientTCP.SendData(
                "search" +
                modTypes.SEP_CHAR + x1 +
                modTypes.SEP_CHAR + y1 +
                modTypes.SEP_CHAR);
        }
    }
}