namespace ET.Server
{
    [ComponentOf(typeof (GateUser))]
    public class GateMapComponent: Entity, IAwake
    {
        public Scene Scene { get; set; }
    }
}