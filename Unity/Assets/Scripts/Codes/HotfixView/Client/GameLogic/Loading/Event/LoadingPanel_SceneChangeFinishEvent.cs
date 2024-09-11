namespace ET.Client
{
    [Event(SceneType.Client)]
    public class LoadingPanel_SceneChangeFinishEvent: AEvent<EventType.SceneChangeFinish>
    {
        protected override async ETTask Run(Scene scene, EventType.SceneChangeFinish args)
        {
            FUIComponent.Instance.HidePanel(PanelId.LoadingPanel);
            await ETTask.CompletedTask;
        }
    }
}