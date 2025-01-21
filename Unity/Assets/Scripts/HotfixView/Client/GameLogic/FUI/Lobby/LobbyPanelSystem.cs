namespace ET.Client
{
    [EntitySystemOf(typeof(LobbyPanel))]
    [FriendOf(typeof(LobbyPanel))]
    public static partial class LobbyPanelSystem
    {
        [EntitySystem]
        private static void Awake(this LobbyPanel self)
        {
            self.FUILobbyPanel.TestABtn.AddListner(() =>
            {
                self.Root().GetComponent<FUIComponent>().HidePanel<LobbyPanel>();
                self.Root().GetComponent<FUIComponent>().ShowPanelAsync<TestAPanel>().Coroutine();
            });

            self.FUILobbyPanel.EnterMap.AddListnerAsync(self.OnEnterMapButtonClick);

            Log.Info("<color=#FFFF00>大厅界面 Awake</color>");
        }

        [EntitySystem]
        private static void Show(this LobbyPanel self)
        {
            Log.Info("<color=#FFFF00>大厅界面 Show</color>");
        }

        [EntitySystem]
        private static void Hide(this LobbyPanel self)
        {
            Log.Info("<color=#FFFF00>大厅界面 Hide</color>");
        }

        [EntitySystem]
        private static void BeforeUnload(this LobbyPanel self)
        {
            Log.Info("<color=#FFFF00>大厅界面 BeforeUnload</color>");
        }

        private static async ETTask OnEnterMapButtonClick(this LobbyPanel self)
        {
            await EnterMapHelper.EnterMapAsync(self.Root());
            self.Root().GetComponent<FUIComponent>().ClosePanel(self);
        }
    }
}