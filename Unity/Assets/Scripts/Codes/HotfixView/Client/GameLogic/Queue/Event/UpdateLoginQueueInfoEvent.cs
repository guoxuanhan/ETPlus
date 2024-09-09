using ET.EventType;

namespace ET.Client
{
    [Event(SceneType.Current)]
    public class UpdateLoginQueueInfoEvent: AEvent<EventType.UpdateLoginQueueInfo>
    {
        protected override async ETTask Run(Scene scene, UpdateLoginQueueInfo args)
        {
            await FUIComponent.Instance.ShowPanelAsync(PanelId.QueueInfoPanel);

            FUIComponent.Instance.GetPanelLogic<QueueInfoPanel>().RefreshQueueInfo(args.QueueIndex, args.QueueCount);
        }
    }
}