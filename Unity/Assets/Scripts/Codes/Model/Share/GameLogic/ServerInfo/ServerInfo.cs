namespace ET
{
    /// <summary>
    /// 服务器信息类型定义
    /// </summary>
    public enum Enum_ServerZoneType
    {
        Common = 0,
        Game = 1,
        Robot = 2,
        Router = 3,
    }

    /// <summary>
    /// 服务器信息状态定义
    /// </summary>
    public enum Enum_ServerState
    {
        Close = 0,
        Running_通畅 = 1,
        Running_拥挤 = 2,
        Running_火爆 = 3,
        Running_爆满 = 4,
    }

    [ChildOf(typeof (ServerInfoComponent))]
    public class ServerInfo: Entity, IAwake, IDestroy
    {
        /// <summary>
        /// 区服Id
        /// </summary>
        public int ZoneId { get; set; }

        /// <summary>
        /// 服务器状态
        /// </summary>
        public Enum_ServerState State { get; set; } = Enum_ServerState.Close;
    }
}