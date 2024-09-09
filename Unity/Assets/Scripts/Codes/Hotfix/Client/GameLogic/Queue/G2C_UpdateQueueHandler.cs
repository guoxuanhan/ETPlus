namespace ET.Client
{
    [MessageHandler(SceneType.Client)]
    public class G2C_UpdateQueueHandler: AMHandler<G2C_UpdateQueue>
    {
        protected override async ETTask Run(Session session, G2C_UpdateQueue message)
        {
            await EventSystem.Instance.PublishAsync(session.DomainScene(),
                new ET.EventType.UpdateLoginQueueInfo() { QueueIndex = message.QueueIndex, QueueCount = message.QueueCount });
        }
    }
}