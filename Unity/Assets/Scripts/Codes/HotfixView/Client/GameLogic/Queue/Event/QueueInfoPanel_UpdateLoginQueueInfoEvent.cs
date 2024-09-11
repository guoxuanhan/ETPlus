using ET.EventType;

namespace ET.Client
{
    [Event(SceneType.Client)]
    public class QueueInfoPanel_UpdateLoginQueueInfoEvent: AEvent<UpdateLoginQueueInfo>
    {
        protected override async ETTask Run(Scene scene, UpdateLoginQueueInfo args)
        {
            await FUIComponent.Instance.ShowPanelAsync(PanelId.QueueInfoPanel);

            FUIComponent.Instance.GetPanelLogic<QueueInfoPanel>().RefreshQueueInfo(args.QueueIndex, args.QueueCount);
        }
    }
}