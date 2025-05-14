using Mirage.Client.Modules;
using Mirage.Client.Net;
using Mirage.Net.Protocol.FromClient;

namespace Mirage.Client.Forms;

public partial class frmIndex : Form
{
	public frmIndex()
	{
		InitializeComponent();
	}

	private void cmdOk_Click(object sender, EventArgs e)
	{
		modGameLogic.EditorIndex = lstIndex.SelectedIndex + 1;
		if (modGameLogic.InItemsEditor)
		{
			Network.Send(new EditItemRequest(modGameLogic.EditorIndex));
		}
		if (modGameLogic.InNpcEditor)
		{
			Network.Send(new EditNpcRequest(modGameLogic.EditorIndex));
		}
		if (modGameLogic.InShopEditor)
		{
			Network.Send(new EditShopRequest(modGameLogic.EditorIndex));
		}
		if (modGameLogic.InSpellEditor)
		{
			Network.Send(new EditSpellRequest(modGameLogic.EditorIndex));
		}
		Close();
	}
	
	private void cmdCancel_Click(object sender, EventArgs e)
	{
		modGameLogic.InItemsEditor = false;
		modGameLogic.InNpcEditor = false;
		modGameLogic.InShopEditor = false;
		modGameLogic.InSpellEditor = false;
		Close();
	}
}