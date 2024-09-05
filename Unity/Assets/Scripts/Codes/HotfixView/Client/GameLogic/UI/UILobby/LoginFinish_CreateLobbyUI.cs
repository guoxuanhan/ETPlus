namespace ET.Client
{
	[Event(SceneType.Client)]
	public class LoginFinish_CreateLobbyUI: AEvent<EventType.LoginFinish>
	{
		protected override async ETTask Run(Scene scene, EventType.LoginFinish args)
		{
			// await UIHelper.Create(scene, UIType.UILobby, UILayer.Mid);
			FUIComponent.Instance.ClosePanel(PanelId.LoginPanel);
			await FUIComponent.Instance.ShowPanelAsync(PanelId.LobbyPanel);
		}
	}
}
