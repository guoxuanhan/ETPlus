namespace ET.Server
{
    [ActorMessageHandler(SceneType.Gate)]
    public class R2G_GetLoginKeyHandler: AMActorRpcHandler<Scene, R2G_GetLoginKey, G2R_GetLoginKey>
    {
        protected override async ETTask Run(Scene scene, R2G_GetLoginKey request, G2R_GetLoginKey response)
        {
            GateUser gateUser = scene.GetComponent<GateUserMgrComponent>().Get(request.Info.Account);

            if (gateUser != null)
            {
                long instanceId = gateUser.InstanceId;

                using (await gateUser.GetGateUserLock())
                {
                    if (instanceId != gateUser.InstanceId)
                    {
                        // 已经被顶号登录
                        return;
                    }
                    
                    gateUser.OfflineSession();
                }

            }

            long key = RandomGenerator.RandInt64();
            scene.GetComponent<GateSessionKeyComponent>().Add(key, request.Info);
            response.GateKey = key;
            await ETTask.CompletedTask;
        }
    }
}