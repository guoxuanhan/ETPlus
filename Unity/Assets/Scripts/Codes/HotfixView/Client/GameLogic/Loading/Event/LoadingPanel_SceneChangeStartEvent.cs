namespace ET.Client
{
    [Event(SceneType.Client)]
    public class LoadingPanel_SceneChangeStartEvent: AEvent<EventType.SceneChangeStart>
    {
        protected override async ETTask Run(Scene scene, EventType.SceneChangeStart args)
        {
            await FUIComponent.Instance.ShowPanelAsync(PanelId.LoadingPanel);
        }
    }
}