using System.Collections.Generic;

namespace ET.Server
{
    [ComponentOf(typeof (Scene))]
    public class AccountDBZoneComponent: Entity, IAwake, IDestroy
    {
        public Dictionary<string, AccountDB> DicAccounts = new();
    }
}