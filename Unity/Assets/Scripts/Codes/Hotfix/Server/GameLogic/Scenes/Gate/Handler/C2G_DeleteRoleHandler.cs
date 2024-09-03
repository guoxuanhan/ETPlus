namespace ET.Server
{
    [MessageHandler(SceneType.Gate)]
    public class C2G_DeleteRoleHandler: AMRpcHandler<C2G_DeleteRole, G2C_DeleteRole>
    {
        protected override async ETTask Run(Session session, C2G_DeleteRole request, G2C_DeleteRole response)
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

            if (!accountZoneDB.Children.ContainsKey(request.RoleId))
            {
                response.Error = ErrorCode.ERR_DeleteRoleNoneRole;
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

                RoleInfoDB roleInfoDB = accountZoneDB.GetChild<RoleInfoDB>(request.RoleId);

                if (roleInfoDB == null)
                {
                    response.Error = ErrorCode.ERR_DeleteRoleNoneRole;
                    return;
                }

                roleInfoDB.IsDeleted = true;

                await session.GetDirectDB().Save(roleInfoDB);

                response.RoleId = roleInfoDB.Id;
                roleInfoDB.Dispose();
            }
        }
    }
}