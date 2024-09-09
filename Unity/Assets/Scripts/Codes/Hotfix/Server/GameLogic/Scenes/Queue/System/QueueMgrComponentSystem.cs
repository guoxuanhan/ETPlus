using System.Collections.Generic;

namespace ET.Server
{
    public class QueueMgrComponentAwakeSystem: AwakeSystem<QueueMgrComponent>
    {
        protected override void Awake(QueueMgrComponent self)
        {
            self.Timer_Ticket = TimerComponent.Instance.NewRepeatedTimer(ConstValue.Queue_TicketTime, TimerInvokeType.Queue_TicketTimer, self);
            self.Timer_Update = TimerComponent.Instance.NewRepeatedTimer(ConstValue.Queue_TicketUpdateTime, TimerInvokeType.Queue_UpdateTimer, self);
            self.Timer_ClearProtect = TimerComponent.Instance.NewRepeatedTimer(ConstValue.Queue_ClearProtectTime, TimerInvokeType.Queue_ClearProtectTimer, self);
        }
    }

    public class QueueMgrComponentDestroySystem: DestroySystem<QueueMgrComponent>
    {
        protected override void Destroy(QueueMgrComponent self)
        {
            TimerComponent.Instance.Remove(ref self.Timer_Ticket);
            TimerComponent.Instance.Remove(ref self.Timer_Update);
            TimerComponent.Instance.Remove(ref self.Timer_ClearProtect);
            
            self.Online.Clear();
            self.Queue.Clear();
            self.Protects.Clear();
        }
    }

    [FriendOfAttribute(typeof(ET.Server.QueueMgrComponent))]
    [FriendOfAttribute(typeof(ET.Server.QueueInfo))]
    public static class QueueMgrComponentSystem
    {
        /// <summary>
        /// 尝试进入排队
        /// </summary>
        /// <param name="self"></param>
        /// <param name="account"></param>
        /// <param name="unitId"></param>
        /// <param name="gateActorId"></param>
        /// <returns>true: 需要排队 false:可以直接进入游戏</returns>
        public static bool TryEnqueue(this QueueMgrComponent self, string account, long unitId, long gateActorId)
        {
            if (self.Protects.ContainKey(unitId))
            {
                // 进入场景服务器后，掉线之后，会进入掉线保护状态，登录不需要排队
                self.Protects.Remove(unitId);

                if (self.Queue.ContainKey(unitId))
                {
                    // 排队排到一半，掉线了，重新排队
                    return true;
                }

                return false;
            }

            if (self.Online.Contains(unitId))
            {
                return false;
            }

            if (self.Queue.ContainKey(unitId))
            {
                // 重复发送网络消息（重复排队），按之前的排队位置继续排队
                return true;
            }

            self.Enqueue(account, unitId, gateActorId);
            return true;
        }

        public static void Enqueue(this QueueMgrComponent self, string account, long unitId, long gateActorId)
        {
            if (self.Queue.ContainKey(unitId))
            {
                return;
            }

            QueueInfo queueInfo = self.AddChild<QueueInfo>();
            queueInfo.Account = account;
            queueInfo.UnitId = unitId;
            queueInfo.GateActorId = gateActorId;
            queueInfo.QueueIndex = self.Queue.Count + 1;
            self.Queue.AddLast(unitId, queueInfo);
        }

        public static int GetIndex(this QueueMgrComponent self, long unitId)
        {
            return self.Queue[unitId]?.QueueIndex ?? 1;
        }

        public static void Ticket(this QueueMgrComponent self)
        {
            if (self.Online.Count >= ConstValue.Queue_MaxOnline)
            {
                // 服务器承载人数已满
                return;
            }

            for (int i = 0; i < ConstValue.Queue_TicketCount; i++)
            {
                if (self.Queue.Count <= 0)
                {
                    return;
                }

                QueueInfo queueInfo = self.Queue.First;
                self.EnterMap(queueInfo.UnitId).Coroutine();
            }
            
        }

        public static async ETTask EnterMap(this QueueMgrComponent self, long unitId)
        {
            if (!self.Online.Add(unitId))
            {
                // 已经在场景服务器中
                return;
            }

            QueueInfo queueInfo = self.Queue.Remove(unitId);
            if (queueInfo != null)
            {
                G2Queue_EnterMap g2QueueEnterMap = (G2Queue_EnterMap)await MessageHelper.CallActor(queueInfo.GateActorId,
                    new Queue2G_EnterMap() { Account = queueInfo.Account, UnitId = queueInfo.UnitId });

                if (g2QueueEnterMap.NeedRemove)
                {
                    self.Online.Remove(unitId);
                }

                queueInfo.Dispose();
            }
        }

        public static void UpdateQueue(this QueueMgrComponent self)
        {
            using (DictionaryComponent<long, Queue2G_UpdateInfo> dict = DictionaryComponent<long, Queue2G_UpdateInfo>.Create())
            {
                using (var enumerator = self.Queue.GetEnumerator())
                {
                    int index = 1;
                    while (enumerator.MoveNext())
                    {
                        QueueInfo queueInfo = enumerator.Current;
                        queueInfo.QueueIndex = index;
                        ++index;

                        Queue2G_UpdateInfo queue2GUpdateInfo;

                        if (!dict.TryGetValue(queueInfo.GateActorId, out queue2GUpdateInfo))
                        {
                            queue2GUpdateInfo = new() { QueueCount = self.Queue.Count };
                            dict.Add(queueInfo.GateActorId, queue2GUpdateInfo);
                        }

                        queue2GUpdateInfo.AccountList.Add(queueInfo.Account);
                        queue2GUpdateInfo.QueueIndexList.Add(queueInfo.QueueIndex);
                    }

                }

                foreach (var kv in dict)
                {
                    MessageHelper.SendActor(kv.Key, kv.Value);
                }
            }
        }


    }
}

