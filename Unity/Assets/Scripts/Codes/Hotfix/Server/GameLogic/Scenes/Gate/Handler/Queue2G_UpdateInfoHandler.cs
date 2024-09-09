namespace ET.Server
{
    [ActorMessageHandler(SceneType.Gate)]
    public class Queue2G_UpdateInfoHandler: AMActorHandler<Scene, Queue2G_UpdateInfo>
    {
        protected override async ETTask Run(Scene scene, Queue2G_UpdateInfo message)
        {
            if (message.QueueCount != message.QueueIndexList.Count)
            {
                return;
            }

            G2C_UpdateQueue g2CUpdateQueue = new() { QueueCount = message.QueueCount };

            GateUserMgrComponent gateUserMgrComponent = scene.GetComponent<GateUserMgrComponent>();

            for (int i = 0; i < message.AccountList.Count; i++)
            {
                string account = message.AccountList[i];
                GateUser gateUser = gateUserMgrComponent.Get(account);

                if (gateUser == null || gateUser.State != Enum_GateUserState.InQueue)
                {
                    continue;
                }

                GateQueueComponent gateQueueComponent = gateUser.GetComponent<GateQueueComponent>();
                gateQueueComponent.QueueIndex = message.QueueIndexList[i];
                gateQueueComponent.QueueCount = message.QueueCount;

                g2CUpdateQueue.QueueIndex = message.QueueIndexList[i];
                gateUser.Session.Send(g2CUpdateQueue);

                // 可能一帧发送大量的消息而造成的网络压力，这里使用一帧只发5条消息
                if ((i + 1) % 5 == 0)
                {
                    await TimerComponent.Instance.WaitFrameAsync();
                }
            }
        }
    }
}