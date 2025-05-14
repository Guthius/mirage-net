using Mirage.Client.Modules;
using Mirage.Client.Net;
using Mirage.Game.Data;
using Mirage.Net.Protocol.FromClient;

namespace Mirage.Client.Forms;

public partial class frmItemEditor : Form
{
    private readonly Image _items;

    public frmItemEditor()
    {
        _items = Image.FromFile("Assets/Items.png");

        InitializeComponent();
    }

    private void frmItemEditor_Load(object sender, EventArgs e)
    {
        txtName.Text = modTypes.Item[modGameLogic.EditorIndex].Name.Trim();
        scrlPic.Value = modTypes.Item[modGameLogic.EditorIndex].Pic;
        lblPic.Text = scrlPic.Value.ToString();
        cmbType.SelectedIndex = modTypes.Item[modGameLogic.EditorIndex].Type;

        if (cmbType.SelectedIndex is >= modTypes.ITEM_TYPE_WEAPON and <= modTypes.ITEM_TYPE_SHIELD)
        {
            fraEquipment.Visible = true;
            scrlDurability.Value = modTypes.Item[modGameLogic.EditorIndex].Data1;
            lblDurability.Text = scrlDurability.Value.ToString();
            scrlStrength.Value = modTypes.Item[modGameLogic.EditorIndex].Data2;
            lblStrength.Text = scrlStrength.Value.ToString();
        }
        else
        {
            fraEquipment.Visible = false;
        }

        if (cmbType.SelectedIndex is >= modTypes.ITEM_TYPE_POTIONADDHP and <= modTypes.ITEM_TYPE_POTIONSUBSP)
        {
            fraVitals.Visible = true;
            scrlVitalMod.Value = modTypes.Item[modGameLogic.EditorIndex].Data1;
            lblVitalMod.Text = scrlVitalMod.Value.ToString();
        }
        else
        {
            fraVitals.Visible = false;
        }

        if (cmbType.SelectedIndex == modTypes.ITEM_TYPE_SPELL)
        {
            fraSpell.Visible = true;
            scrlSpell.Value = modTypes.Item[modGameLogic.EditorIndex].Data1;
            lblSpellName.Text = modTypes.Spell[scrlSpell.Value].Name.Trim();
            lblSpell.Text = scrlSpell.Value.ToString();
        }
        else
        {
            fraSpell.Visible = false;
        }

        UpdatePic();
    }

    private void cmdOk_Click(object sender, EventArgs e)
    {
        modTypes.Item[modGameLogic.EditorIndex].Name = txtName.Text;
        modTypes.Item[modGameLogic.EditorIndex].Pic = scrlPic.Value;
        modTypes.Item[modGameLogic.EditorIndex].Type = cmbType.SelectedIndex;

        switch (cmbType.SelectedIndex)
        {
            case >= modTypes.ITEM_TYPE_WEAPON and <= modTypes.ITEM_TYPE_SHIELD:
                modTypes.Item[modGameLogic.EditorIndex].Data1 = scrlDurability.Value;
                modTypes.Item[modGameLogic.EditorIndex].Data2 = scrlStrength.Value;
                modTypes.Item[modGameLogic.EditorIndex].Data3 = 0;
                break;

            case >= modTypes.ITEM_TYPE_POTIONADDHP and <= modTypes.ITEM_TYPE_POTIONSUBSP:
                modTypes.Item[modGameLogic.EditorIndex].Data1 = scrlVitalMod.Value;
                modTypes.Item[modGameLogic.EditorIndex].Data2 = 0;
                modTypes.Item[modGameLogic.EditorIndex].Data3 = 0;
                break;

            case modTypes.ITEM_TYPE_SPELL:
                modTypes.Item[modGameLogic.EditorIndex].Data1 = scrlSpell.Value;
                modTypes.Item[modGameLogic.EditorIndex].Data2 = 0;
                modTypes.Item[modGameLogic.EditorIndex].Data3 = 0;
                break;
        }
        
        var itemId = modGameLogic.EditorIndex;
        
        Network.Send(new UpdateItemRequest(new ItemInfo
        {
            Id = itemId,
            Name = modTypes.Item[itemId].Name.Trim(),
            Sprite = modTypes.Item[itemId].Pic,
            Type = (ItemType) modTypes.Item[itemId].Type,
            Data1 = modTypes.Item[itemId].Data1,
            Data2 = modTypes.Item[itemId].Data2,
            Data3 = modTypes.Item[itemId].Data3
        }));
        
        modGameLogic.InItemsEditor = false;
        Close();
    }

    private void cmdCancel_Click(object sender, EventArgs e)
    {
        modGameLogic.InItemsEditor = false;
        Close();
    }

    private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
    {
        fraEquipment.Visible = cmbType.SelectedIndex is >= modTypes.ITEM_TYPE_WEAPON and <= modTypes.ITEM_TYPE_SHIELD;
        fraVitals.Visible = cmbType.SelectedIndex is >= modTypes.ITEM_TYPE_POTIONADDHP and <= modTypes.ITEM_TYPE_POTIONSUBSP;
        fraSpell.Visible = cmbType.SelectedIndex == modTypes.ITEM_TYPE_SPELL;
    }

    private void scrlPic_Scroll(object sender, ScrollEventArgs e)
    {
        lblPic.Text = scrlPic.Value.ToString();

        UpdatePic();
    }

    private void scrlVitalMod_Scroll(object sender, ScrollEventArgs e)
    {
        lblVitalMod.Text = scrlVitalMod.Value.ToString();
    }

    private void scrlDurability_Scroll(object sender, ScrollEventArgs e)
    {
        lblDurability.Text = scrlDurability.Value.ToString();
    }

    private void scrlStrength_Scroll(object sender, ScrollEventArgs e)
    {
        lblStrength.Text = scrlStrength.Value.ToString();
    }

    private void scrlSpell_Scroll(object sender, ScrollEventArgs e)
    {
        lblSpellName.Text = modTypes.Spell[scrlSpell.Value].Name.Trim();
        lblSpell.Text = scrlSpell.Value.ToString();
    }

    private void UpdatePic()
    {
        var image = new Bitmap(modTypes.PIC_X, modTypes.PIC_Y);

        using (var g = Graphics.FromImage(image))
        {
            g.DrawImage(_items,
                new Rectangle(0, 0, modTypes.PIC_X, modTypes.PIC_Y),
                new Rectangle(0, scrlPic.Value * modTypes.PIC_Y, modTypes.PIC_X, modTypes.PIC_Y),
                GraphicsUnit.Pixel);
        }

        picPic.Image = image;
    }
}