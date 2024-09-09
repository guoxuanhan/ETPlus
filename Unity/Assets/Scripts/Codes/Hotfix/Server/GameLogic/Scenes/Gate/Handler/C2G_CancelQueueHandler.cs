namespace ET.Server
{
    [MessageHandler(SceneType.Gate)]
    public class C2G_CancelQueueHandler: AMRpcHandler<C2G_CancelQueue, G2C_CancelQueue>
    {
        protected override async ETTask Run(Session session, C2G_CancelQueue request, G2C_CancelQueue response)
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

            long instanceId = gateUser.InstanceId;

            using (await gateUser.GetGateUserLock())
            {
                if (instanceId != accountZoneDB.InstanceId)
                {
                    response.Error = ErrorCode.ERR_GetRoleNoneAccountZone;
                    return;
                }

                if (gateUser.State == Enum_GateUserState.InMap)
                {
                    response.Error = ErrorCode.ERR_CancelQueueRoleInMap;
                    return;
                }

                gateUser.RemoveComponent<GateQueueComponent>();

                // 向排队服发送主动断开的消息（主动断开不需要断线保护）
                MessageHelper.SendActor(session.DomainZone(), SceneType.Queue,
                    new G2Queue_Disconnect() { UnitId = accountZoneDB.LastLoginRoleId, IsProtect = false });
            }
        }
    }
}