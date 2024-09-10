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
                gObject.asButton.GetChild("txt_name").asTextField.text = roleInfoDB.Name;
                gObject.asButton.GetChild("txt_level").asTextField.text = roleInfoDB.Level.ToString();
                
                gObject.asButton.GetController("button").SetSelectedPage("showcharacter");
                gObject.asButton.GetChild("btn_delete").asButton.AddListnerAsync((context) =>
                {
                    // 禁止向下穿透
                    context.StopPropagation();
                    return self.OnClickDeleteRoleHandler(roleInfoDB.Id);
                });
            }
        }

        private static void OnTouchRoleInfoHandler(this SelectCharacterPanel self, int index, GObject gObject)
        {
            // 重置上次选中的角色显示状态
            if (self.LastSelectRoleInfoIndex != -1 && self.LastSelectRoleInfoIndex != index)
            {
                RoleInfoDB lastRoleInfoDB = self.ClientScene().GetComponent<RoleInfoComponent>().GetRoleInfoByIndex(self.LastSelectRoleInfoIndex);
                if (lastRoleInfoDB != null)
                {
                    var lastGObject = self.FUISelectCharacterPanel.list.GetChildAt(self.LastSelectRoleInfoIndex);
                    lastGObject.asButton.GetController("button").SetSelectedPage("showcharacter");
                }
            }

            RoleInfoDB roleInfoDB = self.ClientScene().GetComponent<RoleInfoComponent>().GetRoleInfoByIndex(index);
            
            if (roleInfoDB == null)
            {
                return;
            }
            
            // 当前选中的角色Id
            self.ClientScene().GetComponent<RoleInfoComponent>().CurrentRoleId = roleInfoDB.Id;
            
            gObject.asButton.GetController("button").SetSelectedPage("selected");

            self.LastSelectRoleInfoIndex = index;
        }

        private static async ETTask OnClickCreateRoleHandler(this SelectCharacterPanel self)
        {
            try
            {
                string nickName = $"角色{RandomGenerator.RandomNumber(0, 100000)}";

                int errorCode = await LoginHelper.CreateRole(self.ClientScene(), nickName);
                if (errorCode != ErrorCode.ERR_Success)
                {
                    return;
                }

                Log.Info($"注册成功：{nickName}");
                self.RefreshItemRenderer();
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
                
                Log.Info($"删除成功：{roleId}");
                self.RefreshItemRenderer();
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

                FUIComponent.Instance.ClosePanel(PanelId.SelectCharacterPanel);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        private static void RefreshItemRenderer(this SelectCharacterPanel self)
        {
            for (int i = 0; i < self.FUISelectCharacterPanel.list.numChildren; i++)
            {
                GObject item = self.FUISelectCharacterPanel.list.GetChildAt(i);
                // 手动调用 itemRenderer 来更新每个项
                self.FUISelectCharacterPanel.list.itemRenderer(i, item);
            }
        }
    }
}