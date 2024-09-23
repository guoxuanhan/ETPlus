using FairyGUI;

namespace ET.Client
{
	[Event(SceneType.Client)]
	public class LobbyPanel_SceneChangeStartEvent: AEvent<EventType.SceneChangeFinish>
	{
		protected override async ETTask Run(Scene scene, EventType.SceneChangeFinish args)
		{
			await FUIComponent.Instance.ShowPanelAsync(PanelId.LobbyPanel);
		}
	}
	
	[FriendOf(typeof(LobbyPanel))]
	public static class LobbyPanelSystem
	{
		public static void Awake(this LobbyPanel self)
		{
			var joystick = new Joystick(self.FUILobbyPanel);
			joystick.onStart.Set(self.JoystickStart);
			joystick.onMove.Set(self.JoystickMove);
			joystick.onEnd.Set(self.JoystickEnd);
		}

		public static void RegisterUIEvent(this LobbyPanel self)
		{
		}

		public static void OnShow(this LobbyPanel self, Entity contextData = null)
		{
		}

		public static void OnHide(this LobbyPanel self)
		{
		}

		public static void BeforeUnload(this LobbyPanel self)
		{
		}

		private static void JoystickStart(this LobbyPanel self, EventContext context)
		{
			float degree = (float)context.data;
			self.FUILobbyPanel.GetChild("n9").text = degree.ToString();
		}

		private static void JoystickMove(this LobbyPanel self, EventContext context)
		{
			float degree = (float)context.data;
			self.FUILobbyPanel.GetChild("n9").text = degree.ToString();
		}

		private static void JoystickEnd(this LobbyPanel self, EventContext context)
		{
			float degree = (float)context.data;
			self.FUILobbyPanel.GetChild("n9").text = degree.ToString();
		}
	}
}