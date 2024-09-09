namespace ET.Server
{
    [ActorMessageHandler(SceneType.Queue)]
    [FriendOfAttribute(typeof (ET.Server.QueueMgrComponent))]
    public class G2Queue_EnqueueHandler: AMActorRpcHandler<Scene, G2Queue_Enqueue, Queue2G_Enqueue>
    {
        protected override async ETTask Run(Scene scene, G2Queue_Enqueue request, Queue2G_Enqueue response)
        {
            QueueMgrComponent queueMgrComponent = scene.GetComponent<QueueMgrComponent>();

            if (queueMgrComponent.TryEnqueue(request.Account, request.UnitId, request.GateActorId))
            {
                response.InQueue = true;
                response.QueueCount = queueMgrComponent.Queue.Count;
                response.QueueIndex = queueMgrComponent.GetIndex(request.UnitId);
            }

            await ETTask.CompletedTask;
        }
    }
}