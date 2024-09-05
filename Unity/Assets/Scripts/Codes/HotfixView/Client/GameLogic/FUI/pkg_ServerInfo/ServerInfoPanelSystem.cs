using System;
using FairyGUI;
using UnityEngine;

namespace ET.Client
{
    [FriendOf(typeof(ServerInfoPanel))]
    public static class ServerInfoPanelSystem
    {
        public static void Awake(this ServerInfoPanel self)
        {
        }

        public static void RegisterUIEvent(this ServerInfoPanel self)
        {
            int count = self.ClientScene().GetComponent<ServerInfoComponent>().GetServerInfoCount();
            self.FUIServerInfoPanel.list.Init(self.OnItemRefreshHandler, self.OnItemClickHandler, count);
        }

        public static void OnShow(this ServerInfoPanel self, Entity contextData = null)
        {
        }

        public static void OnHide(this ServerInfoPanel self)
        {
        }

        public static void BeforeUnload(this ServerInfoPanel self)
        {
        }

        private static void OnItemRefreshHandler(this ServerInfoPanel self, int index, GObject gObject)
        {
            ServerInfo serverInfo = self.ClientScene().GetComponent<ServerInfoComponent>().GetServerInfoByIndex(index);

            Color color = Color.white;
            switch (serverInfo.State)
            {
                case  Enum_ServerState.Close:
                    color = Color.gray;
                    break;
                case  Enum_ServerState.Running_通畅:
                    color = Color.green;
                    break;
                case  Enum_ServerState.Running_拥挤:
                    color = Color.red;
                    break;
                case  Enum_ServerState.Running_火爆:
                    color = Color.red;
                    break;
                case  Enum_ServerState.Running_爆满:
                    color = Color.red;
                    break;
            }

            gObject.asButton.GetChild("n0").asImage.color = color;
            gObject.asButton.GetChild("title").text = $"服务器: {serverInfo.ZoneId}";
        }

        private static void OnItemClickHandler(this ServerInfoPanel self, int index, GObject gObject)
        {
            ServerInfo serverInfo = self.ClientScene().GetComponent<ServerInfoComponent>().GetServerInfoByIndex(index);

            try
            {
                LoginHelper.LoginZone(self.ClientScene(), serverInfo.ZoneId).Coroutine();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}