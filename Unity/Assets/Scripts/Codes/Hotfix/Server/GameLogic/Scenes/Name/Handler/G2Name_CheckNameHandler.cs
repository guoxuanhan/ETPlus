using System.Collections.Generic;

namespace ET.Server
{
    [ActorMessageHandler(SceneType.Name)]
    public class G2Name_CheckNameHandler: AMActorRpcHandler<Scene, G2Name_CheckName, Name2G_CheckName>
    {
        protected override async ETTask Run(Scene scene, G2Name_CheckName request, Name2G_CheckName response)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                response.Error = ErrorCode.ERR_CheckNameNoneName;
                return;
            }

            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.CheckName, request.Name.GetHashCode()))
            {
                DBComponent db = scene.GetDirectDB();
                List<CheckNameLog> list = await db.Query<CheckNameLog>(d => d.Name == request.Name);

                if (list.Count > 0)
                {
                    response.Error = ErrorCode.ERR_CheckNameRepeated;
                    return;
                }

                using (CheckNameLog checkNameLog = scene.GetComponent<TempComponent>().AddChild<CheckNameLog>())
                {
                    checkNameLog.UnitId = request.UnitId;
                    checkNameLog.Name = request.Name;
                    checkNameLog.CreateTimeMs = TimeHelper.ServerNow();

                    await db.Save(checkNameLog);
                }
            }
        }
    }
}