namespace ET.Server
{
    [ObjectSystem]
    public class GateUserMgrComponentDestroySystem: DestroySystem<GateUserMgrComponent>
    {
        protected override void Destroy(GateUserMgrComponent self)
        {
            foreach (GateUser entity in self.DicGateUsers.Values)
            {
                entity.Dispose();
            }

            self.DicGateUsers.Clear();
        }
    }

    [FriendOfAttribute(typeof (ET.Server.GateUserMgrComponent))]
    public static class GateUserMgrComponentSystem
    {
        public static GateUser Get(this GateUserMgrComponent self, string account)
        {
            self.DicGateUsers.TryGetValue(account, out var gateUser);
            return gateUser;
        }

        public static void Remove(this GateUserMgrComponent self, string account)
        {
            GateUser gateUser = self.Get(account);
            if (gateUser == null)
            {
                return;
            }

            self.DicGateUsers.Remove(account);
            gateUser.Dispose();
        }

        public static GateUser Create(this GateUserMgrComponent self, string account, int zone)
        {
            GateUser gateUser = self.AddChild<GateUser>();
            gateUser.AddComponent<MailBoxComponent>();
            // gateUser.AddComponent<MailBoxComponent, MailboxType>(MailboxType.GateSession);

            AccountZoneDB accountZoneDB = gateUser.AddComponent<AccountZoneDB>();
            accountZoneDB.Account = account;
            accountZoneDB.LoginZoneId = zone;
            self.GetDirectDB().Save(accountZoneDB).Coroutine();

            self.DicGateUsers.Add(account, gateUser);
            return gateUser;
        }

        public static GateUser Create(this GateUserMgrComponent self, AccountZoneDB accountZoneDB)
        {
            GateUser gateUser = self.AddChild<GateUser>();
            gateUser.AddComponent<MailBoxComponent>();

            gateUser.AddComponent(accountZoneDB);

            self.DicGateUsers.Add(accountZoneDB.Account, gateUser);
            return gateUser;
        }
    }
}