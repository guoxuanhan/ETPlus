namespace ET.Server
{
    [ActorMessageHandler(SceneType.Gate)]
    public class G2Name_CheckNameHandler: AMActorRpcHandler<Scene, G2Name_CheckName, Name2G_CheckName>
    {
        protected override async ETTask Run(Scene unit, G2Name_CheckName request, Name2G_CheckName response)
        {
            
            
            await ETTask.CompletedTask;
        }
    }
}