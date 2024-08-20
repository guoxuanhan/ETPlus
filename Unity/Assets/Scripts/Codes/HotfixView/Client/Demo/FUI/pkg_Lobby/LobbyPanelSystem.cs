namespace ET.Client
{
	[FriendOf(typeof(LobbyPanel))]
	public static class LobbyPanelSystem
	{
		public static void Awake(this LobbyPanel self)
		{
		}

		public static void RegisterUIEvent(this LobbyPanel self)
		{
			self.FUILobbyPanel.enter.AddListnerAsync(self.EnterMap);
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
		
		private static async ETTask EnterMap(this LobbyPanel self)
		{
			await EnterMapHelper.EnterMapAsync(self.ClientScene());
			self.DomainScene().GetComponent<FUIComponent>().ClosePanel(PanelId.LobbyPanel);
		}
	}
}