namespace ET.Server
{
    [MessageHandler(SceneType.Realm)]
    public class C2R_LoginAccountHandler: AMRpcHandler<C2R_LoginAccount, R2C_LoginAccount>
    {
        protected override async ETTask Run(Session session, C2R_LoginAccount request, R2C_LoginAccount response)
        {
            session.RemoveComponent<SessionAcceptTimeoutComponent>();
            
            int modelCount = request.Account.Mode(StartSceneConfigCategory.Instance.Realms.Count);

            if (session.InstanceId != StartSceneConfigCategory.Instance.Realms[modelCount].InstanceId)
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

            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginAccount, request.Account.GetLongHashCode()))
            {
                AccountDBMgrComponent accountDBMgrComponent = session.DomainScene().GetComponent<AccountDBMgrComponent>();

                AccountDB accountDB = await accountDBMgrComponent.Query(request.Account);

                if (Options.Instance.Develop == 0)
                {
                    if (accountDB == null)
                    {
                        response.Error = ErrorCode.ERR_LoginRealmAccountNotExist;
                        return;
                    }

                    if (accountDB.Password != request.Password)
                    {
                        response.Error = ErrorCode.ERR_LoginRealmPasswordWrong;
                        return;
                    }
                }
                else
                {
                    if (accountDB == null)
                    {
                        accountDB = accountDBMgrComponent.AddChild<AccountDB>();
                        accountDB.Account = request.Account;
                        accountDB.Password = request.Password;

                        accountDBMgrComponent.AddContainer(accountDB);
                        await accountDBMgrComponent.GetDirectDB().Save(accountDB);
                    }
                }

                accountDBRealmComponent = session.AddComponent<AccountDBRealmComponent>();
                accountDBRealmComponent.AccountDB = accountDB;
            }
        }
    }
}