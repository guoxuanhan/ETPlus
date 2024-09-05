namespace ET
{
    [ObjectSystem]
    public class ServerInfoDestroySystem: DestroySystem<ServerInfo>
    {
        protected override void Destroy(ServerInfo self)
        {
            self.ZoneId = default;
            self.State = default;
        }
    }

    public static class ServerInfoSystem
    {
        public static ServerInfoProto ToProto(this ServerInfo self)
        {
            ServerInfoProto proto = new();

            proto.ZoneId = self.ZoneId;
            proto.State = self.State.ToInt();

            return proto;
        }

        public static void FromProto(this ServerInfoProto self, ServerInfo serverInfo)
        {
            serverInfo.ZoneId = self.ZoneId;
            serverInfo.State = (Enum_ServerState)self.State;
        }
    }
}