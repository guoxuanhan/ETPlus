namespace ET.Client
{
    [Event(SceneType.Client)]
    public class QueueInfoPanel_SceneChangeStartEvent: AEvent<EventType.SceneChangeStart>
    {
        protected override async ETTask Run(Scene scene, EventType.SceneChangeStart args)
        {
            FUIComponent.Instance.ClosePanel(PanelId.QueueInfoPanel);
            await ETTask.CompletedTask;
        }
    }
}