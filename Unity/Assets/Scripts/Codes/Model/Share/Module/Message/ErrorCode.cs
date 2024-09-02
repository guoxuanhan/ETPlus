namespace ET
{
    public static partial class ErrorCode
    {
        public const int ERR_Success = 0;

        // 1-11004 是SocketError请看SocketError定义
        //-----------------------------------
        // 100000-109999是Core层的错误
        
        // 110000以下的错误请看ErrorCore.cs
        
        // 这里配置逻辑层的错误码
        // 110000 - 200000是抛异常的错误
        // 200001以上不抛异常
        
        public const int ERR_ResourceInitError = 300000;            // 资源初始化失败
        public const int ERR_ResourceUpdateVersionError = 300001;   // 资源更新版本号失败
        public const int ERR_ResourceUpdateManifestError = 300002;  // 资源更新清单失败
        public const int ERR_ResourceUpdateDownloadError = 300003;  // 资源更新下载失败

        public const int ERR_LoginRealmAddressError = 300010;       // 登录realm地址错误
        public const int ERR_LoginRealmAccountNotExist = 300011;    // 登录realm账号不存在
        public const int ERR_LoginRealmPasswordWrong = 300012;      // 登录realm账号或密码错误
        public const int ERR_LoginRealmRepeated = 300013;           // 登录realm重复登录

        public const int ERR_LoginZoneAccountNotLogin = 300014;     // 登录zone账号未登录
        public const int ERR_LoginZoneZoneNotExists = 300015;       // 登录zone区服不存在

        public const int ERR_LoginGateKeyError = 300016;            // 登录gate令牌错误
        public const int ERR_LoginGateMultiLogin = 300017;          // 登录gate顶号登录

        public const int ERR_GetRoleNoneGateUser = 300018;          // 获取role非法玩家不存在
        public const int ERR_GetRoleNoneAccountZone = 300019;       // 获取role区服账号信息不存在
    }
}