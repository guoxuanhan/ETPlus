namespace ET.Server
{
    /// <summary>
    /// GateUser的状态定义
    /// </summary>
    public enum Enum_GateUserState
    {
        InGate = 1,
        InQueue = 2,
        InMap = 3,
    }

    /// <summary>
    /// 玩家在gate服上的映射
    /// </summary>
    [ChildOf(typeof (GateUserMgrComponent))]
    public class GateUser: Entity, IAwake, IDestroy
    {
        /// <summary>
        /// 所链接的Session instanceId
        /// </summary>
        public long SessionInstanceId { get; set; }

        /// <summary>
        /// 所链接的Session实体
        /// </summary>
        public Session Session => Root.Instance.Get(this.SessionInstanceId) as Session;

        /// <summary>
        /// 记录GateUser的状态
        /// </summary>
        public Enum_GateUserState State { get; set; } = Enum_GateUserState.InGate;
    }
}