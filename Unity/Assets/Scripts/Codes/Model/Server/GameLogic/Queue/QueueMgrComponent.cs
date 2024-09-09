using System.Collections.Generic;

namespace ET.Server
{
    /// <summary>
    /// 排队信息
    /// </summary>
    [ChildOf(typeof (QueueMgrComponent))]
    public class QueueInfo: Entity, IAwake, IDestroy
    {
        public long UnitId;

        public long GateActorId;

        public string Account;

        public int QueueIndex;

        // 这里可以放玩家等级、VIP等级、ip等权限管理等等
    }

    /// <summary>
    /// 掉线保护排队信息
    /// </summary>
    public struct ProtectQueueInfo
    {
        public long UnitId;

        public long Timer;
    }

    [ComponentOf(typeof (Scene))]
    public class QueueMgrComponent: Entity, IAwake, IDestroy
    {
        /// <summary>
        /// 放行进场景服务器定时器
        /// </summary>
        public long Timer_Ticket;

        /// <summary>
        /// 清理掉线保护队列定时器
        /// </summary>
        public long Timer_ClearProtect;

        /// <summary>
        /// 更新排队排名定时器
        /// </summary>
        public long Timer_Update;

        /// <summary>
        /// 允许在线的玩家
        /// </summary>
        public HashSet<long> Online = new();

        /// <summary>
        /// 排队队列
        /// </summary>
        public HashLinkedList<long, QueueInfo> Queue = new();

        /// <summary>
        /// 掉线保护队列
        /// </summary>
        public HashLinkedList<long, ProtectQueueInfo> Protects = new();
    }
}