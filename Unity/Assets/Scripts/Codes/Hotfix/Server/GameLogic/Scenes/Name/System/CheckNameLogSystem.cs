namespace ET.Server
{
    [ObjectSystem]
    public class CheckNameLogDestroySystem: DestroySystem<CheckNameLog>
    {
        protected override void Destroy(CheckNameLog self)
        {
            self.UnitId = default;
            self.Name = default;
            self.CreateTimeMs = default;
        }
    }

    public static class CheckNameLogSystem
    {
    }
}