namespace ET.Server
{
    /// <summary>
    /// 当前Session对应的GateUser（Session）
    /// </summary>
    [ComponentOf(typeof (Session))]
    public class SessionUserComponent: Entity, IAwake<long>, IDestroy
    {
        public long GateUserInstanceId { get; set; }

        public GateUser User => Root.Instance.Get(this.GateUserInstanceId) as GateUser;
    }
}