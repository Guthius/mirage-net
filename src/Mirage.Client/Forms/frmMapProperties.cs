using Mirage.Client.Modules;
using Mirage.Game.Constants;

namespace Mirage.Client.Forms;

public partial class frmMapProperties : Form
{
    public frmMapProperties()
    {
        InitializeComponent();
    }

    private void frmMapProperties_Load(object sender, EventArgs e)
    {
        txtName.Text = modTypes.Map.Name;
        txtUp.Text = modTypes.Map.Up.ToString();
        txtDown.Text = modTypes.Map.Down.ToString();
        txtLeft.Text = modTypes.Map.Left.ToString();
        txtRight.Text = modTypes.Map.Right.ToString();
        cmbMoral.SelectedIndex = modTypes.Map.Moral;
        scrlMusic.Value = modTypes.Map.Music;
        txtBootMap.Text = modTypes.Map.BootMap.ToString();
        txtBootX.Text = modTypes.Map.BootX.ToString();
        txtBootY.Text = modTypes.Map.BootY.ToString();

        cmbShop.Items.Add("No Shop");
        for (var i = 1; i <= Limits.MaxShops; i++)
        {
            cmbShop.Items.Add($"{i}: {modTypes.Shop[i].Name.Trim()}");
        }

        cmbShop.SelectedIndex = modTypes.Map.Shop;

        cmbNpc_0.Items.Add("No NPC");
        cmbNpc_1.Items.Add("No NPC");
        cmbNpc_2.Items.Add("No NPC");
        cmbNpc_3.Items.Add("No NPC");
        cmbNpc_4.Items.Add("No NPC");

        for (var i = 1; i <= Limits.MaxNpcs; i++)
        {
            cmbNpc_0.Items.Add($"{i}: {modTypes.Npc[i].Name.Trim()}");
            cmbNpc_1.Items.Add($"{i}: {modTypes.Npc[i].Name.Trim()}");
            cmbNpc_2.Items.Add($"{i}: {modTypes.Npc[i].Name.Trim()}");
            cmbNpc_3.Items.Add($"{i}: {modTypes.Npc[i].Name.Trim()}");
            cmbNpc_4.Items.Add($"{i}: {modTypes.Npc[i].Name.Trim()}");
        }

        cmbNpc_0.SelectedIndex = modTypes.Map.Npc[1];
        cmbNpc_1.SelectedIndex = modTypes.Map.Npc[2];
        cmbNpc_2.SelectedIndex = modTypes.Map.Npc[3];
        cmbNpc_3.SelectedIndex = modTypes.Map.Npc[4];
        cmbNpc_4.SelectedIndex = modTypes.Map.Npc[5];
    }

    private void scrlMusic_Scroll(object sender, ScrollEventArgs e)
    {
        lblMusic.Text = scrlMusic.Value.ToString();
    }

    private void cmdOk_Click(object sender, EventArgs e)
    {
        modTypes.Map.Name = txtName.Text.Trim();
        modTypes.Map.Up = int.Parse(txtUp.Text);
        modTypes.Map.Down = int.Parse(txtDown.Text);
        modTypes.Map.Left = int.Parse(txtLeft.Text);
        modTypes.Map.Right = int.Parse(txtRight.Text);
        modTypes.Map.Moral = cmbMoral.SelectedIndex;
        modTypes.Map.Music = scrlMusic.Value;
        modTypes.Map.BootMap = int.Parse(txtBootMap.Text);
        modTypes.Map.BootX = int.Parse(txtBootX.Text);
        modTypes.Map.BootY = int.Parse(txtBootY.Text);
        modTypes.Map.Shop = cmbShop.SelectedIndex;

        modTypes.Map.Npc[1] = cmbNpc_0.SelectedIndex;
        modTypes.Map.Npc[2] = cmbNpc_1.SelectedIndex;
        modTypes.Map.Npc[3] = cmbNpc_2.SelectedIndex;
        modTypes.Map.Npc[4] = cmbNpc_3.SelectedIndex;
        modTypes.Map.Npc[5] = cmbNpc_4.SelectedIndex;

        Close();
    }

    private void cmdCancel_Click(object sender, EventArgs e)
    {
        Close();
    }
}