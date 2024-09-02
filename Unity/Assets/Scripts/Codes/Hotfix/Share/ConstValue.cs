namespace ET
{
    public static class ConstValue
    {
        public const string RouterHttpHost = "127.0.0.1";
        public const int RouterHttpPort = 30300;
        public const int SessionTimeoutTime = 30 * 1000;
        public const int LoginGate_GateUserDisconnectTime = 20 * 1000;          // 顶号保留时间
        public const int LoginGate_GateUserSessionDisconnectTime = 60 * 1000;   // 断线保留时间
    }
}