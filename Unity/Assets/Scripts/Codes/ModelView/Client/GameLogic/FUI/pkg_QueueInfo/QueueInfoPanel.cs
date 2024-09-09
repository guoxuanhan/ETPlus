using ET.Client.pkg_QueueInfo;

namespace ET.Client
{
	[ComponentOf(typeof(FUIEntity))]
	public class QueueInfoPanel: Entity, IAwake
	{
		private FUI_QueueInfoPanel _fuiQueueInfoPanel;

		public FUI_QueueInfoPanel FUIQueueInfoPanel
		{
			get => _fuiQueueInfoPanel ??= (FUI_QueueInfoPanel)this.GetParent<FUIEntity>().GComponent;
		}
	}
}
