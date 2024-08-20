namespace ET.Client
{
	[Event(SceneType.Client)]
	public class LoginFinish_CreateLobbyUI: AEvent<EventType.LoginFinish>
	{
		protected override async ETTask Run(Scene scene, EventType.LoginFinish args)
		{
			// await UIHelper.Create(scene, UIType.UILobby, UILayer.Mid);
			scene.GetComponent<FUIComponent>().ClosePanel(PanelId.LoginPanel);
			await scene.GetComponent<FUIComponent>().ShowPanelAsync(PanelId.LobbyPanel);
		}
	}
}
