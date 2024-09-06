namespace ET
{
    [ObjectSystem]
    public class RoleInfoComponentDestroySystem: DestroySystem<RoleInfoComponent>
    {
        protected override void Destroy(RoleInfoComponent self)
        {
            self.ClearRoleInfos();
        }
    }

    [FriendOfAttribute(typeof (ET.RoleInfoComponent))]
    public static class RoleInfoComponentSystem
    {
        public static void ClearRoleInfos(this RoleInfoComponent self)
        {
            foreach (RoleInfoDB roleInfoDB in self.RoleInfosList)
            {
                roleInfoDB.Dispose();
            }

            self.RoleInfosList.Clear();
        }

        public static RoleInfoDB GetRoleInfoByIndex(this RoleInfoComponent self, int index)
        {
            if (index < 0 || index >= self.RoleInfosList.Count)
            {
                return null;
            }

            return self.RoleInfosList[index];
        }

        public static RoleInfoDB GetRoleInfoById(this RoleInfoComponent self, long roleId)
        {
            foreach (RoleInfoDB roleInfoDB in self.RoleInfosList)
            {
                if (roleInfoDB.Id == roleId)
                {
                    return roleInfoDB;
                }
            }

            return null;
        }

        public static void AddRoleInfo(this RoleInfoComponent self, RoleInfoProto roleInfoProto)
        {
            RoleInfoDB roleInfoDB = self.AddChildWithId<RoleInfoDB>(roleInfoProto.UnitId);

            roleInfoDB.FromProto(roleInfoProto);

            self.RoleInfosList.Add(roleInfoDB);
        }

        public static void RemoveRoleInfo(this RoleInfoComponent self, long roleId)
        {
            RoleInfoDB roleInfoDB = self.GetRoleInfoById(roleId);

            if (roleInfoDB == null)
            {
                return;
            }

            self.RoleInfosList.Remove(roleInfoDB);
            roleInfoDB.Dispose();
        }

        public static bool IsCurrentRoleExist(this RoleInfoComponent self)
        {
            if (self.CurrentRoleId == 0)
            {
                return false;
            }

            RoleInfoDB roleInfoDB = self.GetRoleInfoById(self.CurrentRoleId);
            if (roleInfoDB == null)
            {
                return false;
            }

            return true;
        }
    }
}