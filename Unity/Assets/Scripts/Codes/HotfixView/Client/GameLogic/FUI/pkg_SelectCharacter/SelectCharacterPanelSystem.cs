using System;
using FairyGUI;

namespace ET.Client
{
    [FriendOf(typeof (SelectCharacterPanel))]
    public static class SelectCharacterPanelSystem
    {
        public static void Awake(this SelectCharacterPanel self)
        {
        }

        public static void RegisterUIEvent(this SelectCharacterPanel self)
        {
            self.FUISelectCharacterPanel.btn_return.AddListnerAsync(self.OnClickReturnHandler);
            self.FUISelectCharacterPanel.btn_login.AddListnerAsync(self.OnClickStartGameHandler);

            self.FUISelectCharacterPanel.list.SetVirtual();
            self.FUISelectCharacterPanel.list.Init(self.OnRefreshRoleInfoHanlder, self.OnTouchRoleInfoHandler, 4);
        }

        public static void OnShow(this SelectCharacterPanel self, Entity contextData = null)
        {
        }

        public static void OnHide(this SelectCharacterPanel self)
        {
        }

        public static void BeforeUnload(this SelectCharacterPanel self)
        {
        }

        private static void OnRefreshRoleInfoHanlder(this SelectCharacterPanel self, int index, GObject gObject)
        {
            RoleInfoDB roleInfoDB = self.ClientScene().GetComponent<RoleInfoComponent>().GetRoleInfoByIndex(index);

            if (roleInfoDB == null)
            {
                gObject.asButton.GetController("button").SetSelectedPage("nocharacter");
                gObject.asButton.GetChild("btn_add").asButton.AddListnerAsync(self.OnClickCreateRoleHandler);
            }
            else
            {
                gObject.asButton.GetController("button").SetSelectedPage("showcharacter");
                gObject.asButton.GetChild("btn_delete").asButton.AddListnerAsync(() => { return self.OnClickDeleteRoleHandler(roleInfoDB.Id); });
            }
        }

        private static void OnTouchRoleInfoHandler(this SelectCharacterPanel self, int index, GObject gObject)
        {
            RoleInfoDB roleInfoDB = self.ClientScene().GetComponent<RoleInfoComponent>().GetRoleInfoByIndex(index);

            if (roleInfoDB == null)
            {
                return;
            }

            // 当前选中的角色Id
            self.ClientScene().GetComponent<RoleInfoComponent>().CurrentRoleId = roleInfoDB.Id;

            gObject.asButton.GetController("button").SetSelectedPage("selected");
        }

        private static async ETTask OnClickCreateRoleHandler(this SelectCharacterPanel self)
        {
            try
            {
                string nickName = $"角色{RandomGenerator.RandomNumber(0, 100)}";

                int errorCode = await LoginHelper.CreateRole(self.ClientScene(), nickName);
                if (errorCode != ErrorCode.ERR_Success)
                {
                    return;
                }

                self.FUISelectCharacterPanel.list.RefreshVirtualList();
                Log.Info($"注册成功：{nickName}");
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        private static async ETTask OnClickDeleteRoleHandler(this SelectCharacterPanel self, long roleId)
        {
            try
            {
                int errorCode = await LoginHelper.DeleteRole(self.ClientScene(), roleId);
                if (errorCode != ErrorCode.ERR_Success)
                {
                    return;
                }
                
                self.FUISelectCharacterPanel.list.RefreshVirtualList();
                Log.Info($"删除成功：{roleId}");
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
        
        private static async ETTask OnClickReturnHandler(this SelectCharacterPanel self)
        {
            await FUIComponent.Instance.ShowPanelAsync(PanelId.ServerInfoPanel);
            FUIComponent.Instance.ClosePanel(PanelId.SelectCharacterPanel);
        }

        private static async ETTask OnClickStartGameHandler(this SelectCharacterPanel self)
        {
            try
            {
                int errorCode = await LoginHelper.EnterMap(self.ClientScene());
                if (errorCode != ErrorCode.ERR_Success)
                {
                    return;
                }

                await FUIComponent.Instance.ShowPanelAsync(PanelId.LobbyPanel);
                FUIComponent.Instance.ClosePanel(PanelId.SelectCharacterPanel);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}