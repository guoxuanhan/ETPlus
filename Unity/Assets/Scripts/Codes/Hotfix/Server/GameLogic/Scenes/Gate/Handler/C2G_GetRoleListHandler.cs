namespace ET.Server
{
    [MessageHandler(SceneType.Gate)]
    public class C2G_GetRoleListHandler: AMRpcHandler<C2G_GetRoleList, G2C_GetRoleList>
    {
        protected override async ETTask Run(Session session, C2G_GetRoleList request, G2C_GetRoleList response)
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

            if (accountZoneDB.Children.Count > 0)
            {
                response.RoleInfos ??= new();
                
                foreach (Entity entity in accountZoneDB.Children.Values)
                {
                    if (entity is RoleInfoDB roleInfoDB)
                    {
                        response.RoleInfos.Add(roleInfoDB.ToProto());
                    }
                }
            }

            await ETTask.CompletedTask;
        }
    }
}