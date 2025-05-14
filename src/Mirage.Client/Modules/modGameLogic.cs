using Mirage.Client.Forms;
using Mirage.Client.Net;
using Mirage.Game.Constants;
using Mirage.Game.Data;
using Mirage.Net.Protocol.FromClient;
using Image = System.Drawing.Image;

namespace Mirage.Client.Modules;

public static class modGameLogic
{
    // Speed moving vars
    public const int WALK_SPEED = 4;
    public const int RUN_SPEED = 8;

    // Index of actual player
    public static int MyIndex;
    
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
    public static readonly modTypes.MapItemRec[] SaveMapItem = new modTypes.MapItemRec[Limits.MaxMapItems + 1];
    public static readonly modTypes.MapNpcRec[] SaveMapNpc = new modTypes.MapNpcRec[Limits.MaxMapNpcs + 1];

    // Used for index based editors
    public static bool InItemsEditor;
    public static bool InNpcEditor;
    public static bool InShopEditor;
    public static bool InSpellEditor;
    public static int EditorIndex;

    // Game fps
    public static int GameFPS;

    public static void CheckMapGetItem()
    {
        if (Environment.TickCount > modTypes.Player[MyIndex].MapGetTimer + 250)
        {
            modTypes.Player[MyIndex].MapGetTimer = Environment.TickCount;
            Network.Send<PickupItemRequest>();
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
            }
        }
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
                Options.GameName,
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
                Options.GameName,
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
                Options.GameName,
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
                Options.GameName,
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
            Options.GameName,
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