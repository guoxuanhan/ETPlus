namespace ET.Server
{
    [ObjectSystem]
    public class GateQueueComponentDestroySystem: DestroySystem<GateQueueComponent>
    {
        protected override void Destroy(GateQueueComponent self)
        {
            self.UnitId = default;
            self.QueueIndex = default;
            self.QueueCount = default;
        }
    }

    public static class GateQueueComponentSystem
    {
    }
}