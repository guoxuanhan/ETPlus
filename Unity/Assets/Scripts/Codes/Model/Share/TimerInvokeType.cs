namespace ET
{
    [UniqueId(100, 10000)]
    public static class TimerInvokeType
    {
        // 框架层100-200，逻辑层的timer type从200起
        public const int WaitTimer = 100;
        public const int SessionIdleChecker = 101;
        public const int ActorLocationSenderChecker = 102;
        public const int ActorMessageSenderChecker = 103;

        // 框架层100-200，逻辑层的timer type 200-300
        public const int MoveTimer = 201;
        public const int AITimer = 202;
        public const int SessionAcceptTimeout = 203;

        public const int MultiLoginTimer = 501;             // 顶号时上个角色保留时间
        public const int GateUserDisconnectTimeout = 502;   // 顶号保留时间

        public const int Queue_TicketTimer = 503;           // 排队服放行的定时器
        public const int Queue_UpdateTimer = 504;           // 排队服更新排名
        public const int Queue_ClearProtectTimer = 505;     // 排队服定时清理离线保护信息

        public const int LoadingTimer = 506;                // 客户端加载界面的定时器
    }
}