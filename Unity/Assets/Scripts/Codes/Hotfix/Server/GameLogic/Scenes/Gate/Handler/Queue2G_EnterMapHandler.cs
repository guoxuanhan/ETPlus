namespace ET.Server
{
    [ActorMessageHandler(SceneType.Gate)]
    public class Queue2G_EnterMapHandler: AMActorRpcHandler<Scene, Queue2G_EnterMap, G2Queue_EnterMap>
    {
        protected override async ETTask Run(Scene scene, Queue2G_EnterMap request, G2Queue_EnterMap response)
        {
            using (await LoginHelper.GetGateUserLock(request.Account))
            {
                GateUser gateUser = scene.GetComponent<GateUserMgrComponent>().Get(request.Account);

                Log.Console($"-> 测试 {request.Account} 排队完成！");

                if (gateUser == null || gateUser.GetComponent<MultiLoginComponent>() != null || gateUser.State == Enum_GateUserState.InGate ||
                    gateUser.GetComponent<GateQueueComponent>() == null)
                {
                    response.NeedRemove = true;
                    return;
                }

                if (gateUser.State == Enum_GateUserState.InMap)
                {
                    return;
                }

                gateUser.EnterMap().Coroutine();
            }
        }
    }
}