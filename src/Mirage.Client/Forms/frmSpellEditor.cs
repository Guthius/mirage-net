using Mirage.Client.Modules;
using Mirage.Client.Net;
using Mirage.Game.Data;
using Mirage.Net.Protocol.FromClient;

namespace Mirage.Client.Forms;

public partial class frmSpellEditor : Form
{
    public frmSpellEditor()
    {
        InitializeComponent();
    }

    private void frmSpellEditor_Load(object sender, EventArgs e)
    {
        cmbClassReq.Items.Add("All Classes");
        for (var i = 0; i < modTypes.Max_Classes; i++)
        {
            cmbClassReq.Items.Add(modTypes.Class[i].Name);
        }

        txtName.Text = modTypes.Spell[modGameLogic.EditorIndex].Name;
        cmbClassReq.SelectedIndex = modTypes.Spell[modGameLogic.EditorIndex].ClassReq;
        scrlLevelReq.Value = modTypes.Spell[modGameLogic.EditorIndex].LevelReq;
        lblLevelReq.Text = scrlLevelReq.Value.ToString();
        cmbType.SelectedIndex = modTypes.Spell[modGameLogic.EditorIndex].Type;

        if (modTypes.Spell[modGameLogic.EditorIndex].Type != modTypes.SPELL_TYPE_GIVEITEM)
        {
            fraVitals.Visible = true;
            fraGiveItem.Visible = false;
            scrlVitalMod.Value = modTypes.Spell[modGameLogic.EditorIndex].Data1;
            lblVitalMod.Text = scrlVitalMod.Value.ToString();
        }
        else
        {
            fraVitals.Visible = false;
            fraGiveItem.Visible = true;
            scrlItemNum.Value = modTypes.Spell[modGameLogic.EditorIndex].Data1;
            fraGiveItem.Text = $"Give Item {modTypes.Item[scrlItemNum.Value].Name.Trim()}";
            lblItemNum.Text = scrlItemNum.Value.ToString();
            scrlItemValue.Value = modTypes.Spell[modGameLogic.EditorIndex].Data2;
            lblItemValue.Text = scrlItemValue.Value.ToString();
        }
    }

    private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (cmbType.SelectedIndex != modTypes.SPELL_TYPE_GIVEITEM)
        {
            fraVitals.Visible = true;
            fraGiveItem.Visible = false;
        }
        else
        {
            fraVitals.Visible = false;
            fraGiveItem.Visible = true;
        }
    }

    private void scrlItemNum_Scroll(object sender, ScrollEventArgs e)
    {
        fraGiveItem.Text = $"Give Item {modTypes.Item[scrlItemNum.Value].Name.Trim()}";
        lblItemNum.Text = scrlItemNum.Value.ToString();
    }

    private void scrlItemValue_Scroll(object sender, ScrollEventArgs e)
    {
        lblItemValue.Text = scrlItemValue.Value.ToString();
    }

    private void scrlLevelReq_Scroll(object sender, ScrollEventArgs e)
    {
        lblLevelReq.Text = scrlLevelReq.Value.ToString();
    }

    private void scrlVitalMod_Scroll(object sender, ScrollEventArgs e)
    {
        lblVitalMod.Text = scrlVitalMod.Value.ToString();
    }

    private void cmdOk_Click(object sender, EventArgs e)
    {
        ref var spell = ref modTypes.Spell[modGameLogic.EditorIndex];


        spell.Name = txtName.Text;
        spell.ClassReq = cmbClassReq.SelectedIndex;
        spell.LevelReq = scrlLevelReq.Value;
        spell.Type = cmbType.SelectedIndex;

        if (cmbType.SelectedIndex != modTypes.SPELL_TYPE_GIVEITEM)
        {
            spell.Data1 = scrlVitalMod.Value;
        }
        else
        {
            spell.Data1 = scrlItemNum.Value;
            spell.Data2 = scrlItemValue.Value;
        }

        Network.Send(new UpdateSpellRequest(new SpellInfo
        {
            Id = modGameLogic.EditorIndex,
            Name = spell.Name.Trim(),
            RequiredClassId = string.Empty, // spell.ClassReq,
            RequiredLevel = spell.LevelReq,
            Type = (SpellType) spell.Type,
            Data1 = spell.Data1,
            Data2 = spell.Data2,
            Data3 = spell.Data3
        }));

        modGameLogic.InSpellEditor = false;
        Close();
    }

    private void cmdCancel_Click(object sender, EventArgs e)
    {
        modGameLogic.InSpellEditor = false;
        Close();
    }
}