namespace ET.Client
{
    [Event(SceneType.GameLogic)]
    public class LoginFinish_RemoveLoginUI : AEvent<Scene, LoginFinish>
    {
        protected override async ETTask Run(Scene root, LoginFinish args)
        {
            root.GetComponent<FUIComponent>().ClosePanel(PanelId.LoginPanel);
            await ETTask.CompletedTask;
        }
    }
}