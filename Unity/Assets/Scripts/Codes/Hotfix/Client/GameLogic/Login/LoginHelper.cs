using System;
using System.Net;
using System.Net.Sockets;

namespace ET.Client
{
    public static class LoginHelper
    {
        public static async ETTask Login(Scene clientScene, string account, string password)
        {
            try
            {
                // 创建一个ETModel层的Session
                clientScene.RemoveComponent<RouterAddressComponent>();
                // 获取路由跟realmDispatcher地址
                RouterAddressComponent routerAddressComponent = clientScene.GetComponent<RouterAddressComponent>();
                if (routerAddressComponent == null)
                {
                    routerAddressComponent =
                            clientScene.AddComponent<RouterAddressComponent, string, int>(ConstValue.RouterHttpHost, ConstValue.RouterHttpPort);
                    await routerAddressComponent.Init();

                    clientScene.AddComponent<NetClientComponent, AddressFamily>(routerAddressComponent.RouterManagerIPAddress.AddressFamily);
                }

                IPEndPoint realmAddress = routerAddressComponent.GetRealmAddress(account);

                R2C_LoginAccount r2CLogin;
                using (Session session = await RouterHelper.CreateRouterSession(clientScene, realmAddress))
                {
                    r2CLogin = (R2C_LoginAccount)await session.Call(new C2R_LoginAccount() { Account = account, Password = password });
                }

                // // 创建一个gate Session,并且保存到SessionComponent中
                // Session gateSession = await RouterHelper.CreateRouterSession(clientScene, NetworkHelper.ToIPEndPoint(r2CLogin.Address));
                // clientScene.AddComponent<SessionComponent>().Session = gateSession;
                //
                // G2C_LoginGate g2CLoginGate = (G2C_LoginGate)await gateSession.Call(
                //     new C2G_LoginGate() { Key = r2CLogin.Key, GateId = r2CLogin.GateId });

                // Log.Debug("登陆gate成功!");

                await EventSystem.Instance.PublishAsync(clientScene, new EventType.LoginFinish());
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public static async ETTask<int> LoginAccount(Scene clientScene, string account, string password)
        {
            // 创建一个ETModel层的Session
            clientScene.RemoveComponent<RouterAddressComponent>();

            // 获取路由跟realmDispatcher地址
            RouterAddressComponent routerAddressComponent = clientScene.GetComponent<RouterAddressComponent>();
            if (routerAddressComponent == null)
            {
                routerAddressComponent =
                        clientScene.AddComponent<RouterAddressComponent, string, int>(ConstValue.RouterHttpHost, ConstValue.RouterHttpPort);
                await routerAddressComponent.Init();

                clientScene.AddComponent<NetClientComponent, AddressFamily>(routerAddressComponent.RouterManagerIPAddress.AddressFamily);
            }

            IPEndPoint realmAddress = routerAddressComponent.GetRealmAddress(account);

            Session session = await RouterHelper.CreateRouterSession(clientScene, realmAddress);

            R2C_LoginAccount r2CLoginAccount =
                    (R2C_LoginAccount)await session.Call(new C2R_LoginAccount() { Account = account, Password = password });
            if (r2CLoginAccount.Error != ErrorCode.ERR_Success)
            {
                Log.Error($"登录账号失败：{r2CLoginAccount.Error}");
                return r2CLoginAccount.Error;
            }
            
            // TODO: 临时使用第一个区服
            ServerZoneInfo serverZoneInfo = r2CLoginAccount.ServerZoneInfoList[0];

            SessionComponent sessionComponent = clientScene.GetComponent<SessionComponent>();
            if (sessionComponent == null)
            {
                sessionComponent = clientScene.AddComponent<SessionComponent>();
            }

            sessionComponent.Session = session;
            return ErrorCode.ERR_Success;
        }

        public static async ETTask<int> LoginZone(Scene clientScene, int zone)
        {
            R2C_LoginZone r2CLoginZone =
                    (R2C_LoginZone)await clientScene.GetComponent<SessionComponent>().Session.Call(new C2R_LoginZone() { ZoneId = zone });
            if (r2CLoginZone.Error != ErrorCode.ERR_Success)
            {
                Log.Error($"登录区服失败：{r2CLoginZone.Error}");
                return r2CLoginZone.Error;
            }

            clientScene.GetComponent<SessionComponent>().Session?.Dispose();

            // 创建一个gate Session,并且保存到SessionComponent中
            Session gateSession = await RouterHelper.CreateRouterSession(clientScene, NetworkHelper.ToIPEndPoint(r2CLoginZone.GateAddress));
            clientScene.GetComponent<SessionComponent>().Session = gateSession;

            G2C_LoginGate g2CLoginGate = (G2C_LoginGate)await gateSession.Call(new C2G_LoginGate() { GateKey = r2CLoginZone.GateKey });
            if (g2CLoginGate.Error != ErrorCode.ERR_Success)
            {
                Log.Error($"登录网关失败：{g2CLoginGate.Error}");
                return g2CLoginGate.Error;
            }
            
            return ErrorCode.ERR_Success;
        }
    }
}