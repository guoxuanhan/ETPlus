using System;

namespace ET.Server
{
    public static class LoginHelper
    {
        public static async ETTask<CoroutineLock> GetGateUserLock(string account)
        {
            if (string.IsNullOrEmpty(account))
            {
                throw new Exception("GetGateUserLock but account is null!");
            }

            return await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginGate, account.GetLongHashCode());
        }

        public static async ETTask<CoroutineLock> GetGateUserLock(this GateUser gateUser)
        {
            AccountZoneDB accountZoneDB = gateUser.GetComponent<AccountZoneDB>();
            return await GetGateUserLock(accountZoneDB.Account);
        }

        public static async ETTask OfflineWithLock(this GateUser self, bool dispose = true)
        {
            if (self == null || self.IsDisposed)
            {
                return;
            }

            long instanceId = self.InstanceId;

            using (await self.GetGateUserLock())
            {
                if (instanceId != self.InstanceId)
                {
                    return;
                }

                await self.Offline(dispose);
            }
        }

        public static async ETTask Offline(this GateUser self, bool dispose = true)
        {
            if (self == null || self.IsDisposed)
            {
                return;
            }

            AccountZoneDB accountZoneDB = self.GetComponent<AccountZoneDB>();
            if (accountZoneDB != null) // 已经登录进Gate服中
            {
                MessageHelper.SendActor(self.DomainZone(), SceneType.Queue,
                    new G2Queue_Disconnect() { UnitId = accountZoneDB.LastLoginRoleId, IsProtect = false });
            }

            if (dispose)
            {
                self.DomainScene().GetComponent<GateUserMgrComponent>().Remove(accountZoneDB?.Account);
            }
            else
            {
                self.State = Enum_GateUserState.InGate;
                self.RemoveComponent<GateQueueComponent>();
            }

            await ETTask.CompletedTask;
        }

        public static void OfflineSession(this GateUser self)
        {
            Log.Console($"-> 账号 {self.GetComponent<AccountZoneDB>()?.Account} 被顶号 {self.SessionInstanceId} 对外下线！");
            Session session = self.Session;

            if (session != null)
            {
                // 有其他玩家使用同样账号连接Gate服
                session.Send(new A2C_Disconnect() { Error = ErrorCode.ERR_LoginGateMultiLogin });

                session.RemoveComponent<SessionUserComponent>();
                session.Disconnect().Coroutine();
            }

            self.SessionInstanceId = 0;
            
            // 为了防止后续玩家顶掉之前玩家后一直不登录，这里就加一个定时器，到时间了后续顶号的还不上来就对内下线了

            self.RemoveComponent<GateUserDisconnectComponent>();
            self.AddComponent<GateUserDisconnectComponent, long>(ConstValue.LoginGate_GateUserDisconnectTime);
        }

        public static async ETTask EnterMap(this GateUser self)
        {
            AccountZoneDB accountZoneDB = self.GetComponent<AccountZoneDB>();

            Log.Console($"-> 测试 账号{accountZoneDB.Account} 进入游戏！");

            StartSceneConfig startSceneConfig = StartSceneConfigCategory.Instance.GetBySceneName(self.DomainZone(), "Map1");

            // 移除排队组件
            self.RemoveComponent<GateQueueComponent>();

            if (self.State == Enum_GateUserState.InMap)
            {
                // 顶号二次登录
                return;
            }

            self.State = Enum_GateUserState.InMap;

            GateMapComponent gateMapComponent = self.AddComponent<GateMapComponent>();

            gateMapComponent.Scene = await SceneFactory.CreateServerScene(gateMapComponent, self.Id, IdGenerater.Instance.GenerateInstanceId(),
                gateMapComponent.DomainZone(), "GateMap", SceneType.Map);

            Unit unit = UnitFactory.Create(gateMapComponent.Scene, accountZoneDB.LastLoginRoleId, UnitType.Player);

            // map场景服务器发送给客户端：会先通过gateUser的actorId发往gate服务器映射
            unit.AddComponent<UnitGateComponent, long>(self.InstanceId);

            // gate网关服务器发送给map上的unit映射，它就是map场景服务器上unit映射的actorId，通过它来发送
            self.Session.AddComponent<SessionPlayerComponent>().PlayerId = accountZoneDB.LastLoginRoleId;

            // 开始传送
            await TransferHelper.Transfer(unit, startSceneConfig.InstanceId, startSceneConfig.Name);
            
            // Player player = session.GetComponent<SessionPlayerComponent>().GetMyPlayer();
            //
            // // 在Gate上动态创建一个Map Scene，把Unit从DB中加载放进来，然后传送到真正的Map中，这样登陆跟传送的逻辑就完全一样了
            // GateMapComponent gateMapComponent = player.AddComponent<GateMapComponent>();
            // gateMapComponent.Scene = await SceneFactory.CreateServerScene(gateMapComponent, player.Id, IdGenerater.Instance.GenerateInstanceId(),
            //     gateMapComponent.DomainZone(), "GateMap", SceneType.Map);
            //
            // Scene scene = gateMapComponent.Scene;
            //
            // // 这里可以从DB中加载Unit
            // Unit unit = UnitFactory.Create(scene, player.Id, UnitType.Player);
            // unit.AddComponent<UnitGateComponent, long>(session.InstanceId);
            //
            // StartSceneConfig startSceneConfig = StartSceneConfigCategory.Instance.GetBySceneName(session.DomainZone(), "Map1");
            // // response.MyId = player.Id;
            //
            // // 等到一帧的最后面再传送，先让G2C_EnterMap返回，否则传送消息可能比G2C_EnterMap还早
            // TransferHelper.TransferAtFrameFinish(unit, startSceneConfig.InstanceId, startSceneConfig.Name).Coroutine();
            
        }
        
    }
}