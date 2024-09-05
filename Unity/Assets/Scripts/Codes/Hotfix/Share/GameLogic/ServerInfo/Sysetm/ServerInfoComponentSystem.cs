namespace ET
{
    [ObjectSystem]
    public class ServerInfoComponentDestroySystem: DestroySystem<ServerInfoComponent>
    {
        protected override void Destroy(ServerInfoComponent self)
        {
            self.ClearServerInfos();
        }
    }

    [FriendOfAttribute(typeof (ET.ServerInfoComponent))]
    public static class ServerInfoComponentSystem
    {
        /// <summary>
        /// 获取服务器信息数量
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static int GetServerInfoCount(this ServerInfoComponent self)
        {
            return self.ServerInfosList.Count;
        }

        /// <summary>
        /// 通过列表下标获取服务器信息
        /// </summary>
        /// <param name="self"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static ServerInfo GetServerInfoByIndex(this ServerInfoComponent self, int index)
        {
            if (index < 0 || index >= self.ServerInfosList.Count)
            {
                return null;
            }

            return self.ServerInfosList[index];
        }

        /// <summary>
        /// 通过服务器Id获取服务器信息
        /// </summary>
        /// <param name="self"></param>
        /// <param name="zoneId"></param>
        /// <returns></returns>
        public static ServerInfo GetServerInfoById(this ServerInfoComponent self, int zoneId)
        {
            int index = self.ServerInfosList.FindIndex(d => d.ZoneId == zoneId);
            return self.GetServerInfoByIndex(index);
        }

        /// <summary>
        /// 删除所有服务器信息
        /// </summary>
        /// <param name="self"></param>
        public static void ClearServerInfos(this ServerInfoComponent self)
        {
            foreach (ServerInfo serverInfo in self.ServerInfosList)
            {
                serverInfo.Dispose();
            }

            self.ServerInfosList.Clear();
        }

        /// <summary>
        /// 添加服务器信息
        /// </summary>
        /// <param name="self"></param>
        /// <param name="serverInfoProto"></param>
        public static void AddServerInfo(this ServerInfoComponent self, ServerInfoProto serverInfoProto)
        {
            ServerInfo serverInfo = self.AddChild<ServerInfo>();

            serverInfoProto.FromProto(serverInfo);
            self.ServerInfosList.Add(serverInfo);
        }
    }
}