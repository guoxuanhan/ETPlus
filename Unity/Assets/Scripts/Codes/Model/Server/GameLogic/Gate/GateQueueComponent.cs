namespace ET.Server
{
    [ComponentOf(typeof (GateUser))]
    public class GateQueueComponent: Entity, IAwake, IDestroy
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        public long UnitId { get; set; }

        /// <summary>
        /// 排队名次
        /// </summary>
        public int QueueIndex { get; set; }

        /// <summary>
        /// 排队总数
        /// </summary>
        public int QueueCount { get; set; }
    }
}