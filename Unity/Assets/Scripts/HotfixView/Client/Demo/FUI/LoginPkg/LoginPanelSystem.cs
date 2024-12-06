namespace ET.Client
{
    [EntitySystemOf(typeof(LoginPanel))]
    [FriendOf(typeof(LoginPanel))]
    public static partial class LoginPanelSystem
    {
        [EntitySystem]
        private static void Awake(this LoginPanel self)
        {
            self.FUILoginPanel.LoginBtn.AddListner(self.OnLoginButtonClick);
            Log.Info("<color=#FF0000>登录界面 Awake</color>");
        }

        [EntitySystem]
        private static void Show(this LoginPanel self)
        {
            Log.Info("<color=#FF0000>登录界面 Show</color>");
        }

        [EntitySystem]
        private static void Hide(this LoginPanel self)
        {
            Log.Info("<color=#FF0000>登录界面 Hide</color>");
        }

        [EntitySystem]
        private static void BeforeUnload(this LoginPanel self)
        {
            Log.Info("<color=#FF0000>登录界面 BeforeUnload</color>");
        }

        private static void OnLoginButtonClick(this LoginPanel self)
        {
            string account = self.FUILoginPanel.AccountInput.text ?? "";
            string password = self.FUILoginPanel.PasswordInput.text ?? "";
            LoginHelper.Login(self.Root(), account, password).Coroutine();
        }
    }
}