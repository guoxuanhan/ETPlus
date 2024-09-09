namespace ET.Server
{
    [Invoke(TimerInvokeType.Queue_TicketTimer)]
    public class QueueMgrComponent_Queue_TicketTimer: ATimer<QueueMgrComponent>
    {
        protected override void Run(QueueMgrComponent self)
        {
            self.Ticket();
        }
    }

    [Invoke(TimerInvokeType.Queue_UpdateTimer)]
    public class QueueMgrComponent_Queue_UpdateTimer: ATimer<QueueMgrComponent>
    {
        protected override void Run(QueueMgrComponent self)
        {
            self.UpdateQueue();
        }
    }

    [Invoke(TimerInvokeType.Queue_ClearProtectTimer)]
    public class QueueMgrComponent_Queue_ClearProtectTimer: ATimer<QueueMgrComponent>
    {
        protected override void Run(QueueMgrComponent self)
        {
        }
    }
}