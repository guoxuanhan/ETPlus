using Unity.Mathematics;

namespace ET.Server
{
    [ActorMessageHandler(SceneType.Map)]
    public class G2M_SecondLoginHandler: AMActorLocationRpcHandler<Unit, G2M_SecondLogin, M2G_SecondLogin>
    {
        protected override async ETTask Run(Unit unit, G2M_SecondLogin request, M2G_SecondLogin response)
        {
            Scene scene = unit.DomainScene();

            // 通知客户端开始切场景
            M2C_StartSceneChange m2CStartSceneChange = new M2C_StartSceneChange() { SceneInstanceId = scene.InstanceId, SceneName = scene.Name };
            MessageHelper.SendToClient(unit, m2CStartSceneChange);

            // 通知客户端创建My Unit
            M2C_CreateMyUnit m2CCreateUnits = new M2C_CreateMyUnit();
            m2CCreateUnits.Unit = UnitHelper.CreateUnitInfo(unit);
            MessageHelper.SendToClient(unit, m2CCreateUnits);

            // 加入aoi
            unit.RemoveComponent<AOIEntity>();
            unit.AddComponent<AOIEntity, int, float3>(9 * 1000, unit.Position);

            await ETTask.CompletedTask;
        }
    }
}