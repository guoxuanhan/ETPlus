namespace ET.Server
{
    public static class RoleInfoDBSystem
    {
        public static GateRoleInfo ToMessage(this RoleInfoDB self)
        {
            GateRoleInfo message = new();

            message.UnitId = self.Id;
            message.Name = self.Name;
            message.Level = self.Level;

            return message;
        }
    }
}