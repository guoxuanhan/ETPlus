namespace ET.Server
{
    [Invoke(TimerInvokeType.GateUserDisconnectTimeout)]
    public class GateUserDisconnectComponent_Timer: ATimer<GateUserDisconnectComponent>
    {
        protected override void Run(GateUserDisconnectComponent self)
        {
            // 对内下线，并释放GateUser
            self.GetParent<GateUser>().OfflineWithLock().Coroutine();
        }
    }

    [ObjectSystem]
    public class GateUserDisconnectComponentAwakeSystem: AwakeSystem<GateUserDisconnectComponent, long>
    {
        protected override void Awake(GateUserDisconnectComponent self, long time)
        {
            self.Timer = TimerComponent.Instance.NewOnceTimer(TimeHelper.ServerNow() + time, TimerInvokeType.GateUserDisconnectTimeout, self);
        }
    }

    [ObjectSystem]
    public class GateUserDisconnectComponentDestroySystem: DestroySystem<GateUserDisconnectComponent>
    {
        protected override void Destroy(GateUserDisconnectComponent self)
        {
            TimerComponent.Instance.Remove(ref self.Timer);
        }
    }

    public static class GateUserDisconnectComponentSystem
    {
    }
}