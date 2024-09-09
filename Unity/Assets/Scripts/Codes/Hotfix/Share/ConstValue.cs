namespace ET
{
    public static class ConstValue
    {
        public const string RouterHttpHost = "127.0.0.1";
        public const int RouterHttpPort = 30300;
        public const int SessionTimeoutTime = 30 * 1000;
        public const int LoginGate_GateUserDisconnectTime = 20 * 1000;          // 顶号保留时间
        public const int LoginGate_GateUserSessionDisconnectTime = 60 * 1000;   // 断线保留时间

        public const int Queue_MaxOnline = 3000;                                // 最大在线人数
        public const int Queue_TicketTime = 1 * 1000;                           // 放人时间间隔
        public const int Queue_TicketCount = 1;                                 // 每次放几个人
        public const int Queue_TicketUpdateTime = 10 * 1000;                    // 更新排名时间间隔
        public const int Queue_ClearProtectTime = 10 * 1000;                    // 掉线保护时间间隔
        public const int Queue_ProtectTime = 5 * 60 * 1000;                     // 掉线保护时长
    }
}