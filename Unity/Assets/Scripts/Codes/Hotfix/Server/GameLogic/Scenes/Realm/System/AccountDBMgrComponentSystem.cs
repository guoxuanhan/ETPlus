namespace ET.Server
{
    [ObjectSystem]
    public class AccountDBZoneComponentAwakeSystem: AwakeSystem<AccountDBMgrComponent>
    {
        protected override void Awake(AccountDBMgrComponent self)
        {
        }
    }

    [ObjectSystem]
    public class AccountDBZoneComponentDestroySystem: DestroySystem<AccountDBMgrComponent>
    {
        protected override void Destroy(AccountDBMgrComponent self)
        {
            foreach (var entity in self.DicAccounts.Values)
            {
                entity?.Dispose();
            }

            self.DicAccounts.Clear();
        }
    }

    [FriendOfAttribute(typeof (ET.Server.AccountDBMgrComponent))]
    public static class AccountDBMgrComponentSystem
    {
        public static async ETTask<AccountDB> Query(this AccountDBMgrComponent self, string account)
        {
            if (self.DicAccounts.TryGetValue(account, out AccountDB accountDB))
            {
                return accountDB;
            }

            var accountDBList = await self.GetDirectDB().Query<AccountDB>(d => d.Account == account);
            if (accountDBList == null || accountDBList.Count == 0)
            {
                return null;
            }

            accountDB = accountDBList[0];
            self.AddChild(accountDB);
            self.AddContainer(accountDB);

            return accountDB;
        }

        public static void AddContainer(this AccountDBMgrComponent self, AccountDB accountDB)
        {
            self.DicAccounts.Add(accountDB.Account, accountDB);
        }
    }
}