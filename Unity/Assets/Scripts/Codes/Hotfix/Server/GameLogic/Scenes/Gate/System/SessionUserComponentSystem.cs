namespace ET.Server
{
    [ObjectSystem]
    public class SessionUserComponentAwakeSystem: AwakeSystem<SessionUserComponent, long>
    {
        protected override void Awake(SessionUserComponent self, long instanceId)
        {
            self.GateUserInstanceId = instanceId;
        }
    }

    [ObjectSystem]
    public class SessionUserComponentDestroySystem: DestroySystem<SessionUserComponent>
    {
        protected override void Destroy(SessionUserComponent self)
        {
            GateUser gateUser = self.User;
            if (gateUser != null && self.GetParent<Session>().IsDisposed)
            {
                // 如果是主动断开，应该先移除SessionUserComponent，在销毁Session，否则就认为是突然断开了
                // Session突然断开，若一段时间没有重连就下线
                gateUser.AddComponent<GateUserDisconnectComponent, long>(ConstValue.LoginGate_GateUserSessionDisconnectTime);
            }

            self.GateUserInstanceId = 0;
        }
    }

    public static class SessionUserComponentSystem
    {
    }
}