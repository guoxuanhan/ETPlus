using System.Collections.Generic;

namespace ET
{
    [ComponentOf(typeof (Scene))]
    public class ServerInfoComponent: Entity, IAwake, IDestroy
    {
        public List<ServerInfo> ServerInfosList = new();
    }
}