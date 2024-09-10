namespace ET.Server
{
    [ActorMessageHandler(SceneType.Queue)]
    public class G2Queue_DisconnectHandler: AMActorHandler<Scene, G2Queue_Disconnect>
    {
        protected override async ETTask Run(Scene scene, G2Queue_Disconnect message)
        {
            scene.GetComponent<QueueMgrComponent>().Disconnect(message.UnitId, message.IsProtect);
            await ETTask.CompletedTask;
        }
    }
}