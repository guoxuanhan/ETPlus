namespace ET.Server
{
    /// <summary>
    /// 定时销毁GateUser
    /// </summary>
    [ComponentOf(typeof (GateUser))]
    public class GateUserDisconnectComponent: Entity, IAwake<long>, IDestroy
    {
        public long Timer;
    }
}