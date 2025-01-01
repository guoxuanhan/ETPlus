namespace ET.Client
{
    [Event(SceneType.GameLogic)]
    public class LoginFinish_CreateLobbyUI : AEvent<Scene, LoginFinish>
    {
        protected override async ETTask Run(Scene root, LoginFinish args)
        {
            await root.GetComponent<FUIComponent>().ShowPanelAsync<LobbyPanel>();
        }
    }
}