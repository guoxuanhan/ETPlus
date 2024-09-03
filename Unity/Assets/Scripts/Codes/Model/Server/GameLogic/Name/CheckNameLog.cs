namespace ET.Server
{
    [ChildOf(typeof (TempComponent))]
    public class CheckNameLog: Entity, IAwake, IDestroy
    {
        public long UnitId { get; set; }

        public string Name { get; set; }

        public long CreateTimeMs { get; set; }
    }
}