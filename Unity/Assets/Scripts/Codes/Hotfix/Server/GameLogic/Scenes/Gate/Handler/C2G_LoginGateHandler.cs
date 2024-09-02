using System.Collections.Generic;

namespace ET.Server
{
    [MessageHandler(SceneType.Gate)]
    public class C2G_LoginGateHandler: AMRpcHandler<C2G_LoginGate, G2C_LoginGate>
    {
        protected override async ETTask Run(Session session, C2G_LoginGate request, G2C_LoginGate response)
        {
            Scene scene = session.DomainScene();

            LoginGateInfo loginGateInfo = scene.GetComponent<GateSessionKeyComponent>().Get(request.GateKey);
            if (loginGateInfo == null)
            {
                response.Error = ErrorCode.ERR_LoginGateKeyError;
                response.Message = "Gate key验证失败!";
                return;
            }
            
            // 判断停服、维护、封号、ip等情况

            session.RemoveComponent<SessionAcceptTimeoutComponent>();

            string account = loginGateInfo.Account;
            long instanceId = session.InstanceId;
            
            using (await LoginHelper.GetGateUserLock(account))
            {
                if (instanceId != session.InstanceId)
                {
                    return;
                }

                GateUserMgrComponent gateUserMgrComponent = scene.GetComponent<GateUserMgrComponent>();
                GateUser gateUser = gateUserMgrComponent.Get(account);
                
                if (gateUser == null)
                {
                    DBComponent db = scene.GetDirectDB();

                    List<AccountZoneDB> list = await db.Query<AccountZoneDB>(d => d.Account == account);

                    if (list.Count == 0)
                    {
                        gateUser = gateUserMgrComponent.Create(loginGateInfo.Account, loginGateInfo.LoginZoneId);
                    }
                    else
                    {
                        gateUser = gateUserMgrComponent.Create(list[0]);
                    }

                    long id = gateUser.GetComponent<AccountZoneDB>().Id;

                    var listRole = await db.Query<RoleInfoDB>(d => d.AccountZoneId == id && !d.IsDeleted);
                    if (listRole.Count > 0)
                    {
                        foreach (RoleInfoDB roleInfoDB in listRole)
                        {
                            gateUser.GetComponent<AccountZoneDB>().AddChild(roleInfoDB);
                        }
                    }
                }
                else
                {
                    gateUser.RemoveComponent<GateUserDisconnectComponent>();
                    gateUser.RemoveComponent<MultiLoginComponent>();
                }

                // 链接到新的session
                gateUser.SessionInstanceId = session.InstanceId;
                session.AddComponent<SessionUserComponent, long>(gateUser.InstanceId);

                if (gateUser.State != Enum_GateUserState.InGate)
                {
                    // 把处于排队或Gate中的前一个玩家踢下线
                    gateUser.AddComponent<MultiLoginComponent>();
                }
            }
        }
    }
}