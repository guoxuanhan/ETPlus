namespace ET.Server
{
    [Invoke(TimerInvokeType.MultiLoginTimer)]
    public class MultiLoginComponent_Timer: ATimer<MultiLoginComponent>
    {
        protected override void Run(MultiLoginComponent self)
        {
            // 对内下线，保留GateUser
            self.GetParent<GateUser>()?.OfflineWithLock(false).Coroutine();
        }
    }

    [ObjectSystem]
    public class MultiLoginComponentAwakeSystem: AwakeSystem<MultiLoginComponent>
    {
        protected override void Awake(MultiLoginComponent self)
        {
            self.TimerOver = TimerComponent.Instance.NewOnceTimer(TimeHelper.ServerNow() + (20 * 1000), TimerInvokeType.MultiLoginTimer, self);
        }
    }

    [ObjectSystem]
    public class MultiLoginComponentDestroySystem: DestroySystem<MultiLoginComponent>
    {
        protected override void Destroy(MultiLoginComponent self)
        {
            TimerComponent.Instance.Remove(ref self.TimerOver);
        }
    }

    public static class MultiLoginComponentSystem
    {
    }
}