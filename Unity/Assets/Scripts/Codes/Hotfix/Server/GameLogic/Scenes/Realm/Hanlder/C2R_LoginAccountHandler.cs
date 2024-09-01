namespace ET.Server
{
    [ActorMessageHandler(SceneType.Realm)]
    public class C2R_LoginAccountHandler: AMRpcHandler<C2R_LoginAccount, R2C_LoginAccount>
    {
        protected override async ETTask Run(Session session, C2R_LoginAccount request, R2C_LoginAccount response)
        {
            int modelCount = request.Account.Mode(StartSceneConfigCategory.Instance.Realms.Count);

            if (session.InstanceId != StartSceneConfigCategory.Instance.Realms[modelCount].InstanceId)
            {
                response.Error = ErrorCode.ERR_LoginRealmAddressError;
                return;
            }

            if (string.IsNullOrEmpty(request.Account) || string.IsNullOrEmpty(request.Password))
            {
                response.Error = ErrorCode.ERR_LoginRealmPasswordWrong;
                return;
            }

            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.AccountLogin, request.Account.GetHashCode()))
            {
                AccountDBZoneComponent accountDBZoneComponent = session.DomainScene().GetComponent<AccountDBZoneComponent>();

                AccountDB accountDB = await accountDBZoneComponent.Query(request.Account);

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
                        accountDB = accountDBZoneComponent.AddChild<AccountDB>();
                        accountDB.Account = request.Account;
                        accountDB.Password = request.Password;

                        accountDBZoneComponent.AddContainer(accountDB);
                        await accountDBZoneComponent.GetDirectDB().Save(accountDB);
                    }
                }
            }

            await ETTask.CompletedTask;
        }
    }
}