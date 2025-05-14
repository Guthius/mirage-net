using Mirage.Client.Modules;
using Mirage.Client.Net;
using Mirage.Net.Protocol.FromClient;

namespace Mirage.Client.Forms;

public partial class frmMirage : Form
{
    public frmMirage()
    {
        InitializeComponent();
    }

    private void frmMirage_FormClosed(object sender, FormClosedEventArgs e)
    {
    }

    private void picScreen_MouseDown(object sender, MouseEventArgs e)
    {
        modGameLogic.EditorMouseDown(e.Button, e.X, e.Y);
        modGameLogic.PlayerSearch(e.X, e.Y);
    }

    private void picScreen_MouseMove(object sender, MouseEventArgs e)
    {
        modGameLogic.EditorMouseDown(e.Button, e.X, e.Y);
    }

    private void frmMirage_KeyPress(object sender, KeyPressEventArgs e)
    {
    }

    private void picScreen_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
    {
        modGameLogic.CheckInput(true, e.KeyCode);
    }
    
    private void frmMirage_KeyUp(object sender, KeyEventArgs e)
    {
        modGameLogic.CheckInput(false, e.KeyCode);
    }

    private void txtChat_Enter(object sender, EventArgs e)
    {
        picScreen.Focus();
    }
    
    private void lblCast_Click(object sender, EventArgs e)
    {
        if (modTypes.Player[modGameLogic.MyIndex].Spell[lstSpells.SelectedIndex] > 0)
        {
            if (Environment.TickCount > modTypes.Player[modGameLogic.MyIndex].AttackTimer + 1000)
            {
                if (modTypes.Player[modGameLogic.MyIndex].Moving == 0)
                {
                    Network.Send(new CastRequest(lstSpells.SelectedIndex + 1));
                    modTypes.Player[modGameLogic.MyIndex].Attacking = 1;
                    modTypes.Player[modGameLogic.MyIndex].AttackTimer = Environment.TickCount;
                    modTypes.Player[modGameLogic.MyIndex].CastedSpell = modTypes.YES;
                }
                else
                {
                    modText.AddText("Cannot cast while walking.", modText.BrightRed);
                }
            }
        }
        else
        {
            modText.AddText("No spell here.", modText.BrightRed);
        }
    }
    
    private void lblSpellsCancel_Click(object sender, EventArgs e)
    {
        picPlayerSpells.Visible = false;
    }

    private void optLayers_Click(object sender, EventArgs e)
    {
        if (optLayers.Checked)
        {
            fraLayers.Visible = true;
            fraAttribs.Visible = false;
        }
    }

    private void optAttribs_Click(object sender, EventArgs e)
    {
        if (optAttribs.Checked)
        {
            fraLayers.Visible = false;
            fraAttribs.Visible = true;
        }
    }

    private void picBackSelect_MouseDown(object sender, MouseEventArgs e)
    {
        modGameLogic.EditorChooseTitle(e.Button, e.X, e.Y);
    }

    private void picBackSelect_MouseMove(object sender, MouseEventArgs e)
    {
        modGameLogic.EditorChooseTitle(e.Button, e.X, e.Y);
    }

    private void cmdSend_Click(object sender, EventArgs e)
    {
        modGameLogic.EditorSend();
    }

    private void cmdCancel_Click(object sender, EventArgs e)
    {
        modGameLogic.EditorCancel();
    }

    private void cmdProperties_Click(object sender, EventArgs e)
    {
        using var frmMapProperties = new frmMapProperties();

        frmMapProperties.ShowDialog();
    }

    private void optWarp_Click(object sender, EventArgs e)
    {
        using var frmMapWarp = new frmMapWarp();

        frmMapWarp.ShowDialog();
    }

    private void optItem_Click(object sender, EventArgs e)
    {
        using var frmMapItem = new frmMapItem();

        frmMapItem.ShowDialog();
    }

    private void optKey_Click(object sender, EventArgs e)
    {
        using var frmMapKey = new frmMapKey();

        frmMapKey.ShowDialog();
    }

    private void optKeyOpen_Click(object sender, EventArgs e)
    {
        using var frmKeyOpen = new frmKeyOpen();

        frmKeyOpen.ShowDialog();
    }

    private void scrlPicture_Scroll(object sender, ScrollEventArgs e)
    {
        modGameLogic.EditorTileScroll();
    }

    private void cmdClear_Click(object sender, EventArgs e)
    {
        modGameLogic.EditorClearLayer();
    }

    private void cmdClear2_Click(object sender, EventArgs e)
    {
        modGameLogic.EditorClearAttribs();
    }
}