namespace ET.Server
{
    [MessageHandler(SceneType.Realm)]
    public class C2R_LoginZoneHandler: AMRpcHandler<C2R_LoginZone, R2C_LoginZone>
    {
        protected override async ETTask Run(Session session, C2R_LoginZone request, R2C_LoginZone response)
        {
            AccountDBRealmComponent accountDBRealmComponent = session.GetComponent<AccountDBRealmComponent>();
            if (accountDBRealmComponent == null || accountDBRealmComponent.AccountDB == null)
            {
                response.Error = ErrorCode.ERR_LoginZoneAccountNotLogin;
                return;
            }

            StartZoneConfig startZoneConfig = StartZoneConfigCategory.Instance.GetOrDefault(request.ZoneId);
            if (startZoneConfig == null)
            {
                response.Error = ErrorCode.ERR_LoginZoneZoneNotExists;
                return;
            }

            string account = accountDBRealmComponent.AccountDB.Account;

            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginZone, account.GetHashCode()))
            {
                StartSceneConfig startSceneConfig = RealmGateAddressHelper.GetGate(request.ZoneId, account);

                G2R_GetLoginKey g2RGetLoginKey = (G2R_GetLoginKey)await MessageHelper.CallActor(startSceneConfig.InstanceId,
                            new R2G_GetLoginKey() { Info = new LoginGateInfo() { Account = account, LoginZoneId = request.ZoneId } });
                
                if (g2RGetLoginKey.Error != ErrorCode.ERR_Success)
                {
                    response.Error = g2RGetLoginKey.Error;
                    return;
                }

                response.GateKey = g2RGetLoginKey.GateKey;
                response.GateAddress = startSceneConfig.InnerIPOutPort.ToString();

                session.Disconnect().Coroutine();
            }
        }
    }
}