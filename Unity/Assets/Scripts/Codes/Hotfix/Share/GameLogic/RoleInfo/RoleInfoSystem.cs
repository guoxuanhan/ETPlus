namespace ET
{
    public static class RoleInfoSystem
    {
        public static RoleInfoProto ToProto(this RoleInfoDB self)
        {
            RoleInfoProto message = new();

            message.UnitId = self.Id;
            message.Name = self.Name;
            message.Level = self.Level;

            return message;
        }

        public static void FromProto(this RoleInfoDB self, RoleInfoProto roleInfoProto)
        {
            self.Name = roleInfoProto.Name;
            self.Level = roleInfoProto.Level;
        }
    }
}