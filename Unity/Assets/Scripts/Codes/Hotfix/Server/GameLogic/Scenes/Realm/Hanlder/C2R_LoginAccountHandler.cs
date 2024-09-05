using System.Collections.Generic;

namespace ET.Server
{
    [MessageHandler(SceneType.Realm)]
    public class C2R_LoginAccountHandler: AMRpcHandler<C2R_LoginAccount, R2C_LoginAccount>
    {
        protected override async ETTask Run(Session session, C2R_LoginAccount request, R2C_LoginAccount response)
        {
            session.RemoveComponent<SessionAcceptTimeoutComponent>();
            
            int modelCount = request.Account.Mode(StartSceneConfigCategory.Instance.Realms.Count);

            if (session.DomainScene().InstanceId != StartSceneConfigCategory.Instance.Realms[modelCount].InstanceId)
            {
                response.Error = ErrorCode.ERR_LoginRealmAddressError;
                session.Disconnect().Coroutine();
                return;
            }

            if (string.IsNullOrEmpty(request.Account) || string.IsNullOrEmpty(request.Password))
            {
                response.Error = ErrorCode.ERR_LoginRealmPasswordWrong;
                return;
            }

            AccountDBRealmComponent accountDBRealmComponent = session.GetComponent<AccountDBRealmComponent>();
            if (accountDBRealmComponent != null)
            {
                response.Error = ErrorCode.ERR_LoginRealmRepeated;
                return;
            }

            string account = request.Account;

            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginAccount, account.GetLongHashCode()))
            {
                AccountDB accountDB = null;
                
                List<AccountDB> accountDBList = await session.GetDirectDB().Query<AccountDB>(d => d.Account == account);
                if (accountDBList.Count > 0)
                {
                    accountDB = accountDBList[0];
                }

                // if (Options.Instance.Develop == 0) 
                // {
                //     if (accountDB == null)
                //     {
                //         response.Error = ErrorCode.ERR_LoginRealmAccountNotExist;
                //         return;
                //     }
                //
                //     if (accountDB.Password != request.Password)
                //     {
                //         response.Error = ErrorCode.ERR_LoginRealmPasswordWrong;
                //         return;
                //     }
                // }
                //else
                {
                    if (accountDB == null)
                    {
                        accountDB = session.AddChild<AccountDB>();
                        accountDB.Account = account;
                        accountDB.Password = request.Password;

                        await session.GetDirectDB().Save(accountDB);
                    }
                }

                accountDBRealmComponent = session.AddComponent<AccountDBRealmComponent>();
                accountDBRealmComponent.AccountDB = accountDB;
                accountDBRealmComponent.AddChild(accountDB);
                
                // 返回服务器列表
                response.ServerInfoList ??= new();

                foreach (var startZoneConfig in StartZoneConfigCategory.Instance.GetAll().Values)
                {
                    if (startZoneConfig.Type != Enum_ServerZoneType.Game.ToInt())
                    {
                        continue;
                    }

                    response.ServerInfoList.Add(new ServerInfoProto() { ZoneId = startZoneConfig.Id, State = Enum_ServerState.Running_通畅.ToInt() });
                }
            }
        }
    }
}