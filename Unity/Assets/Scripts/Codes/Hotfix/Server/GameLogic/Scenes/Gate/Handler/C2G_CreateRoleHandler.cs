namespace ET.Server
{
    [MessageHandler(SceneType.Gate)]
    public class C2G_CreateRoleHandler: AMRpcHandler<C2G_CreateRole, G2C_CreateRole>
    {
        protected override async ETTask Run(Session session, C2G_CreateRole request, G2C_CreateRole response)
        {
            GateUser gateUser = session.GetComponent<SessionUserComponent>()?.User;
            if (gateUser == null)
            {
                response.Error = ErrorCode.ERR_GetRoleNoneGateUser;
                return;
            }

            AccountZoneDB accountZoneDB = gateUser.GetComponent<AccountZoneDB>();
            if (accountZoneDB == null)
            {
                response.Error = ErrorCode.ERR_GetRoleNoneAccountZone;
                return;
            }

            if (string.IsNullOrEmpty(request.Name))
            {
                response.Error = ErrorCode.ERR_CreateRoleNoneName;
                return;
            }

            long instanceId = accountZoneDB.InstanceId;

            using (await gateUser.GetGateUserLock())
            {
                if (instanceId != accountZoneDB.InstanceId)
                {
                    response.Error = ErrorCode.ERR_GetRoleNoneAccountZone;
                    return;
                }

                long unitId = IdGenerater.Instance.GenerateUnitId(accountZoneDB.LoginZoneId);

                // 名字查重
                Name2G_CheckName name2GCheckName = (Name2G_CheckName)await MessageHelper.CallActor(accountZoneDB.LoginZoneId, SceneType.Name,
                    new G2Name_CheckName() { UnitId = unitId, Name = request.Name });

                if (name2GCheckName.Error != ErrorCode.ERR_Success)
                {
                    response.Error = name2GCheckName.Error;
                    return;
                }
                
                // 正式创建角色
                RoleInfoDB roleInfoDB = accountZoneDB.AddChildWithId<RoleInfoDB>(unitId);
                roleInfoDB.Account = accountZoneDB.Account;
                roleInfoDB.AccountZoneId = accountZoneDB.Id;
                roleInfoDB.LoginZoneId = accountZoneDB.LoginZoneId;
                roleInfoDB.IsDeleted = false;
                roleInfoDB.Name = request.Name;
                roleInfoDB.Level = 1;

                await session.GetDirectDB().Save(roleInfoDB);

                response.Role = roleInfoDB.ToMessage();
            }
        }
    }
}