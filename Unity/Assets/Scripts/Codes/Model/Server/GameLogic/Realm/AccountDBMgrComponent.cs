using System.Collections.Generic;

namespace ET.Server
{
    [ComponentOf(typeof (Scene))]
    public class AccountDBMgrComponent: Entity, IAwake, IDestroy
    {
        public Dictionary<string, AccountDB> DicAccounts = new();
    }
}