using System;
using UnityEngine;

namespace ET.Client
{
    [FriendOf(typeof (LoginPanel))]
    public static class LoginPanelSystem
    {
        public static void Awake(this LoginPanel self)
        {
        }

        public static void RegisterUIEvent(this LoginPanel self)
        {
            self.FUILoginPanel.btn_login.AddListnerAsync(self.OnLoginButtonClickHandler);
        }

        public static void OnShow(this LoginPanel self, Entity contextData = null)
        {
            string content = "";
            if (contextData != null)
            {
                content = (contextData as LoginPanel_ContextData)?.TestData;
            }

            Log.Info($"展示登录界面： {content}");
        }

        public static void OnHide(this LoginPanel self)
        {
        }

        public static void BeforeUnload(this LoginPanel self)
        {
        }

        private static async ETTask OnLoginButtonClickHandler(this LoginPanel self)
        {
            string account = self.FUILoginPanel.ipt_account.asCom.GetChild("input").text;
            string password = self.FUILoginPanel.ipt_password.asCom.GetChild("input").text;

            try
            {
                await LoginHelper.LoginAccount(self.DomainScene(), account, password);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}