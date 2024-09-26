﻿namespace ET.Server
{
    [MessageHandler(SceneType.Gate)]
    public class C2G_EnterMapHandler: AMRpcHandler<C2G_EnterMap, G2C_EnterMap>
    {
        protected override async ETTask Run(Session session, C2G_EnterMap request, G2C_EnterMap response)
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

            long unitId = request.UnitId;
            string account = accountZoneDB.Account;
            long instanceId = accountZoneDB.InstanceId;

            using (await gateUser.GetGateUserLock())
            {
                if (instanceId != accountZoneDB.InstanceId)
                {
                    response.Error = ErrorCode.ERR_GetRoleNoneAccountZone;
                    return;
                }
                
                // 顶号判定
                if (gateUser.GetComponent<MultiLoginComponent>() != null)
                {
                    if (accountZoneDB.LastLoginRoleId != unitId)
                    {
                        await gateUser.Offline(false);
                    }
                    
                    // 等上面下线后再移除顶号状态，防止这时候刚好排队服排到了上一个号
                    gateUser.RemoveComponent<MultiLoginComponent>();

                    if (gateUser.State == Enum_GateUserState.InQueue)
                    {
                        GateQueueComponent gateQueueComponent = gateUser.GetComponent<GateQueueComponent>();
                        response.InQueue = true;
                        response.QueueIndex = gateQueueComponent.QueueIndex;
                        response.QueueCount = gateQueueComponent.QueueCount;
                        return;
                    }

                    if (gateUser.State == Enum_GateUserState.InMap)
                    {
                        // 等待一帧执行的目的是让逻辑返回，类似reply()
                        await TimerComponent.Instance.WaitFrameAsync();
                        gateUser.EnterMap().Coroutine();
                        return;
                    }
                }

                RoleInfoDB targetRoleInfo = accountZoneDB.GetChild<RoleInfoDB>(unitId);

                if (targetRoleInfo == null || targetRoleInfo.IsDeleted)
                {
                    response.Error = ErrorCode.ERR_EnterMapRoleNotExists;
                    return;
                }

                // 正常的选角流程
                accountZoneDB.LastLoginRoleId = unitId;

                Queue2G_Enqueue queue2GEnqueue = (Queue2G_Enqueue)await MessageHelper.CallActor(accountZoneDB.LoginZoneId, SceneType.Queue,
                    new G2Queue_Enqueue() { UnitId = unitId, Account = account, GateActorId = session.DomainScene().InstanceId });
                
                if (queue2GEnqueue.Error != ErrorCode.ERR_Success)
                {
                    response.Error = queue2GEnqueue.Error;
                    return;
                }

                response.InQueue = queue2GEnqueue.InQueue;
                
                if (queue2GEnqueue.InQueue)
                {
                    gateUser.State = Enum_GateUserState.InQueue;

                    GateQueueComponent gateQueueComponent = gateUser.AddOrGetComponent<GateQueueComponent>();
                    gateQueueComponent.UnitId = unitId;
                    gateQueueComponent.QueueIndex = queue2GEnqueue.QueueIndex;
                    gateQueueComponent.QueueCount = queue2GEnqueue.QueueCount;

                    response.QueueIndex = queue2GEnqueue.QueueIndex;
                    response.QueueCount = queue2GEnqueue.QueueCount;

                    Log.Console($"-> 测试 账号{account} 需要排队 {gateQueueComponent.QueueIndex}/{gateQueueComponent.QueueCount}");
                }
                
                // 等待一帧执行的目的是让逻辑返回，类似reply()
                await TimerComponent.Instance.WaitFrameAsync();

                await session.GetDirectDB().Save(accountZoneDB);

                if (!queue2GEnqueue.InQueue)
                {
                    Log.Console($"-> 测试 账号{account} 免排队进入");
                    gateUser.EnterMap().Coroutine();
                }
            }
        }
    }
}