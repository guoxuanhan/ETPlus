using ET.Client.pkg_ServerInfo;

namespace ET.Client
{
	[ComponentOf(typeof(FUIEntity))]
	public class ServerInfoPanel: Entity, IAwake
	{
		private FUI_ServerInfoPanel _fuiServerInfoPanel;

		public FUI_ServerInfoPanel FUIServerInfoPanel
		{
			get => _fuiServerInfoPanel ??= (FUI_ServerInfoPanel)this.GetParent<FUIEntity>().GComponent;
		}
	}
}
