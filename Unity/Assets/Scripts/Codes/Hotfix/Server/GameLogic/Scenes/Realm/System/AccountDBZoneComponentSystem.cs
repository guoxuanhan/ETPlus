namespace ET.Server
{
    [ObjectSystem]
    public class AccountDBZoneComponentAwakeSystem: AwakeSystem<AccountDBZoneComponent>
    {
        protected override void Awake(AccountDBZoneComponent self)
        {
        }
    }

    [ObjectSystem]
    public class AccountDBZoneComponentDestroySystem: DestroySystem<AccountDBZoneComponent>
    {
        protected override void Destroy(AccountDBZoneComponent self)
        {
            foreach (var entity in self.DicAccounts.Values)
            {
                entity?.Dispose();
            }

            self.DicAccounts.Clear();
        }
    }

    [FriendOfAttribute(typeof (ET.Server.AccountDBZoneComponent))]
    public static class AccountDBZoneComponentSystem
    {
        public static async ETTask<AccountDB> Query(this AccountDBZoneComponent self, string account)
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

        public static void AddContainer(this AccountDBZoneComponent self, AccountDB accountDB)
        {
            self.DicAccounts.Add(accountDB.Account, accountDB);
        }
    }
}