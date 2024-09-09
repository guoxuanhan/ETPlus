using System;

namespace ET.Client
{
	[FriendOf(typeof(QueueInfoPanel))]
	public static class QueueInfoPanelSystem
	{
		public static void Awake(this QueueInfoPanel self)
		{
		}

		public static void RegisterUIEvent(this QueueInfoPanel self)
		{
			self.FUIQueueInfoPanel.btn_close.AddListnerAsync(self.OnClickCancelQueueHandler);
		}

		public static void OnShow(this QueueInfoPanel self, Entity contextData = null)
		{
		}

		public static void OnHide(this QueueInfoPanel self)
		{
		}

		public static void BeforeUnload(this QueueInfoPanel self)
		{
		}

		public static void RefreshQueueInfo(this QueueInfoPanel self, int index, int count)
		{
			string text = $"当前共有[color=#00cc00]{count}人[/color]正在排队进入服务器\r\n您正位于[color=#00cc00]第{index}位[/color]";
			self.FUIQueueInfoPanel.txt_info.text = text;
		}

		private static async ETTask OnClickCancelQueueHandler(this QueueInfoPanel self)
		{
			try
			{
				int errorCode = await LoginHelper.CancelQueue(self.ClientScene());
				if (errorCode != ErrorCode.ERR_Success)
				{
					return;
				}

				FUIComponent.Instance.HidePanel(PanelId.QueueInfoPanel);
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}
	}
}