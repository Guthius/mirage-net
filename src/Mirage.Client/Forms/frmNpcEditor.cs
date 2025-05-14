using Mirage.Client.Modules;

namespace Mirage.Client.Forms;

public partial class frmNpcEditor : Form
{
    private readonly Image _items;

    public frmNpcEditor()
    {
        _items = Image.FromFile("Assets/Sprites.png");

        InitializeComponent();
    }

    private void frmNpcEditor_Load(object sender, EventArgs e)
    {
        ref var npc = ref modTypes.Npc[modGameLogic.EditorIndex];
        
        txtName.Text = modTypes.Npc[modGameLogic.EditorIndex].Name.Trim();
        txtAttackSay.Text = modTypes.Npc[modGameLogic.EditorIndex].AttackSay.Trim();
        scrlSprite.Value = modTypes.Npc[modGameLogic.EditorIndex].Sprite;
        lblSprite.Text = scrlSprite.Value.ToString();
        txtSpawnSecs.Text = modTypes.Npc[modGameLogic.EditorIndex].SpawnSecs.ToString();
        cmbBehavior.SelectedIndex = modTypes.Npc[modGameLogic.EditorIndex].Behavior;
        scrlRange.Value = modTypes.Npc[modGameLogic.EditorIndex].Range;
        lblRange.Text = scrlRange.Value.ToString();
        txtChance.Text = modTypes.Npc[modGameLogic.EditorIndex].DropChance.ToString();
        scrlNum.Value = modTypes.Npc[modGameLogic.EditorIndex].DropItem;
        lblNum.Text = scrlNum.Value.ToString();
        if (scrlNum.Value > 0)
        {
            lblItemName.Text = modTypes.Item[scrlNum.Value].Name.Trim();
        }
        scrlValue.Value = modTypes.Npc[modGameLogic.EditorIndex].DropItemValue;
        lblValue.Text = scrlValue.Value.ToString();
        scrlSTR.Value = modTypes.Npc[modGameLogic.EditorIndex].STR;
        lblSTR.Text = scrlSTR.Value.ToString();
        scrlDEF.Value = modTypes.Npc[modGameLogic.EditorIndex].DEF;
        lblDEF.Text = scrlDEF.Value.ToString();
        scrlSPEED.Value = modTypes.Npc[modGameLogic.EditorIndex].SPEED;
        lblSPEED.Text = scrlSPEED.Value.ToString();
        scrlMAGI.Value = modTypes.Npc[modGameLogic.EditorIndex].MAGI;
        lblMAGI.Text = scrlMAGI.Value.ToString();
        lblStartHP.Text = (scrlSTR.Value * scrlDEF.Value).ToString();
        lblExpGiven.Text = (scrlSTR.Value * scrlDEF.Value * 2).ToString();
        
        UpdateSprite();
    }

    private void scrlSprite_Scroll(object sender, ScrollEventArgs e)
    {
        lblSprite.Text = scrlSprite.Value.ToString();

        UpdateSprite();
    }

    private void scrlRange_Scroll(object sender, ScrollEventArgs e)
    {
        lblRange.Text = scrlRange.Value.ToString();
    }

    private void scrlSTR_Scroll(object sender, ScrollEventArgs e)
    {
        lblSTR.Text = scrlSTR.Value.ToString();
        lblStartHP.Text = (scrlSTR.Value * scrlDEF.Value).ToString();
        lblExpGiven.Text = (scrlSTR.Value * scrlDEF.Value * 2).ToString();
    }

    private void scrlDEF_Scroll(object sender, ScrollEventArgs e)
    {
        lblDEF.Text = scrlDEF.Value.ToString();
        lblStartHP.Text = (scrlSTR.Value * scrlDEF.Value).ToString();
        lblExpGiven.Text = (scrlSTR.Value * scrlDEF.Value * 2).ToString();
    }

    private void scrlSPEED_Scroll(object sender, ScrollEventArgs e)
    {
        lblSPEED.Text = scrlSPEED.Value.ToString();
    }

    private void scrlMAGI_Scroll(object sender, ScrollEventArgs e)
    {
        lblMAGI.Text = scrlMAGI.Value.ToString();
    }

    private void scrlNum_Scroll(object sender, ScrollEventArgs e)
    {
        lblNum.Text = scrlNum.Value.ToString();
        if (scrlNum.Value > 0)
        {
            lblItemName.Text = modTypes.Item[scrlNum.Value].Name.Trim();
        }
    }

    private void scrlValue_Scroll(object sender, ScrollEventArgs e)
    {
        lblValue.Text = scrlValue.Value.ToString();
    }

    private void cmdOk_Click(object sender, EventArgs e)
    {
        var editorIndex = modGameLogic.EditorIndex;

        modTypes.Npc[editorIndex].Name = txtName.Text.Trim();
        modTypes.Npc[editorIndex].AttackSay = txtAttackSay.Text.Trim();
        modTypes.Npc[editorIndex].Sprite = scrlSprite.Value;
        modTypes.Npc[editorIndex].SpawnSecs = int.Parse(txtSpawnSecs.Text);
        modTypes.Npc[editorIndex].Behavior = cmbBehavior.SelectedIndex;
        modTypes.Npc[editorIndex].Range = scrlRange.Value;
        modTypes.Npc[editorIndex].DropChance = int.Parse(txtChance.Text);
        modTypes.Npc[editorIndex].DropItem = scrlNum.Value;
        modTypes.Npc[editorIndex].DropItemValue = scrlValue.Value;
        modTypes.Npc[editorIndex].STR = scrlSTR.Value;
        modTypes.Npc[editorIndex].DEF = scrlDEF.Value;
        modTypes.Npc[editorIndex].SPEED = scrlSPEED.Value;
        modTypes.Npc[editorIndex].MAGI = scrlMAGI.Value;

        modClientTCP.SendSaveNpc(editorIndex);
        modGameLogic.InNpcEditor = false;

        Close();
    }

    private void cmdCancel_Click(object sender, EventArgs e)
    {
        modGameLogic.InNpcEditor = false;

        Close();
    }

    private void UpdateSprite()
    {
        var image = new Bitmap(modTypes.PIC_X, modTypes.PIC_Y);

        using (var g = Graphics.FromImage(image))
        {
            g.DrawImage(_items,
                new Rectangle(0, 0, modTypes.PIC_X, modTypes.PIC_Y),
                new Rectangle(3 * modTypes.PIC_X, scrlSprite.Value * modTypes.PIC_Y, modTypes.PIC_X, modTypes.PIC_Y),
                GraphicsUnit.Pixel);
        }

        picSprite.Image = image;
    }
}