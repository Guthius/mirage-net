using Mirage.Compat;
using Mirage.Modules;
using Mirage.Windows;

namespace Mirage.Forms;

public partial class frmMirage : Form
{
    public frmMirage()
    {
        InitializeComponent();
    }

    private void frmMirage_FormClosed(object sender, FormClosedEventArgs e)
    {
        modGameLogic.GameDestroy();
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

    private void Socket_DataArrival(object sender, DataArrivalEventArgs e)
    {
        if (modClientTCP.IsConnected())
        {
            modClientTCP.IncomingData(e.Bytes);
        }
    }

    private void frmMirage_KeyPress(object sender, KeyPressEventArgs e)
    {
        modGameLogic.HandleKeypresses(e.KeyChar);

        e.Handled = true;
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

    private void picInventory_Click(object sender, EventArgs e)
    {
        modGameLogic.UpdateInventory();
        picInv.Visible = true;
    }

    private void lblUseItem_Click(object sender, EventArgs e)
    {
        modClientTCP.SendUseItem(lstInv.SelectedIndex + 1);
    }

    private void lblDropItem_Click(object sender, EventArgs e)
    {
        var invNum = lstInv.SelectedIndex + 1;

        var invItemNum = modTypes.GetPlayerInvItemNum(modGameLogic.MyIndex, invNum);
        if (invItemNum is > 0 and <= modTypes.MAX_ITEMS)
        {
            if (modTypes.Item[invItemNum].Type == modTypes.ITEM_TYPE_CURRENCY)
            {
                using var frmDrop = new frmDrop();

                frmDrop.ShowDialog();
            }
            else
            {
                modClientTCP.SendDropItem(invNum, 0);
            }
        }
    }

    private void lblCast_Click(object sender, EventArgs e)
    {
        if (modTypes.Player[modGameLogic.MyIndex].Spell[lstSpells.SelectedIndex] > 0)
        {
            if (modGameLogic.GetTickCount() > modTypes.Player[modGameLogic.MyIndex].AttackTimer + 1000)
            {
                if (modTypes.Player[modGameLogic.MyIndex].Moving == 0)
                {
                    modClientTCP.SendData("cast" + modTypes.SEP_CHAR + (lstSpells.SelectedIndex + 1) + modTypes.SEP_CHAR);
                    modTypes.Player[modGameLogic.MyIndex].Attacking = 1;
                    modTypes.Player[modGameLogic.MyIndex].AttackTimer = modGameLogic.GetTickCount();
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

    private void lblCancel_Click(object sender, EventArgs e)
    {
        picInv.Visible = false;
    }

    private void lblSpellsCancel_Click(object sender, EventArgs e)
    {
        picPlayerSpells.Visible = false;
    }

    private void picSpells_Click(object sender, EventArgs e)
    {
        modClientTCP.SendData("spells" + modTypes.SEP_CHAR);
    }

    private void picStats_Click(object sender, EventArgs e)
    {
        modClientTCP.SendData("getstats" + modTypes.SEP_CHAR);
    }

    private void picTrain_Click(object sender, EventArgs e)
    {
        var frmTraining = new WIndowTraining();

        frmTraining.ShowDialog();
    }

    private void picTrade_Click(object sender, EventArgs e)
    {
        modClientTCP.SendData("trade" + modTypes.SEP_CHAR);
    }

    private void picQuit_Click(object sender, EventArgs e)
    {
        modGameLogic.GameDestroy();
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