namespace ET.Client
{
    [ObjectSystem]
    public class SessionComponentAwakeSystem0: AwakeSystem<SessionComponent>
    {
        protected override void Awake(SessionComponent self)
        {
            self.NetworkProtocol = self.ClientScene().GetComponent<NetClientComponent>().NetworkProtocol;
        }
    }

    [ObjectSystem]
    public class SessionComponentAwakeSystem1: AwakeSystem<SessionComponent, NetworkProtocol>
    {
        protected override void Awake(SessionComponent self, NetworkProtocol networkProtocol)
        {
            self.NetworkProtocol = networkProtocol;
        }
    }

    [ObjectSystem]
    public class SessionComponentDestroySystem: DestroySystem<SessionComponent>
    {
        protected override void Destroy(SessionComponent self)
        {
            self.Session.Dispose();
        }
    }

    public static class SessionComponentSystem
    {
        public static async ETTask<IResponse> Call(this SessionComponent self, IRequest request)
        {
            IResponse response = await self.Session.Call(request);
            return response;
        }

        public static void Send(this SessionComponent self, IRequest request)
        {
            self.Session.Send(request);
        }

        public static void Send(this SessionComponent self, IMessage message)
        {
            self.Session.Send(message);
        }

        public static bool HappendError(this Session session, int errorCode, string message = "")
        {
            if (errorCode != ErrorCode.ERR_Success)
            {
                Log.Error($"请求服务器发生了错误：{errorCode} {message}");
                return true;
            }

            return false;
        }
    }
}