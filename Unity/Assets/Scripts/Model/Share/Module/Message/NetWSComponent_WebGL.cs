using System.Collections.Generic;

namespace ET
{
    [ComponentOf(typeof(Scene))]
    public class NetWSComponent: Entity, IAwake<IEnumerable<string>>, IAwake, IDestroy, IUpdate
    {
        public AService AService;
    }
}