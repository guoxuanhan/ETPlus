namespace ET.Client
{
	[FriendOf(typeof(LoginPanel))]
	public static class LoginPanelSystem
	{
		public static void Awake(this LoginPanel self)
		{
		}

		public static void RegisterUIEvent(this LoginPanel self)
		{
			self.FUILoginPanel.login.AddListnerAsync(self.Login);
		}

		public static void OnShow(this LoginPanel self, Entity contextData = null)
		{
			string content = "";
			if (contextData != null)
			{
				content = (contextData as LoginPanel_ContextData)?.TestData;
			}

			Log.Info($"展示登录界面： {content}");
		}

		public static void OnHide(this LoginPanel self)
		{
		}

		public static void BeforeUnload(this LoginPanel self)
		{
		}

		private static async ETTask Login(this LoginPanel self)
		{
			string account = self.FUILoginPanel.account.asCom.GetChild("input").text;
			string password = self.FUILoginPanel.password.asCom.GetChild("input").text;
			await LoginHelper.Login(self.DomainScene(), account, password);
		}
	}
}