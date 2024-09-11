using ET.Client.pkg_Loading;

namespace ET.Client
{
	[ComponentOf(typeof(FUIEntity))]
	public class LoadingPanel: Entity, IAwake
	{
		private FUI_LoadingPanel _fuiLoadingPanel;

		public FUI_LoadingPanel FUILoadingPanel
		{
			get => _fuiLoadingPanel ??= (FUI_LoadingPanel)this.GetParent<FUIEntity>().GComponent;
		}
	}
}
