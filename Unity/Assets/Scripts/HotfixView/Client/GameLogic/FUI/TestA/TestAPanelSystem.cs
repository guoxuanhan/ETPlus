namespace ET.Client
{
    [EntitySystemOf(typeof(TestAPanel))]
    [FriendOf(typeof(TestAPanel))]
    public static partial class TestAPanelSystem
    {
        [EntitySystem]
        private static void Awake(this TestAPanel self)
        {
            var fuiCom = self.Root().GetComponent<FUIComponent>();
            self.FUITestAPanel.OpenTestBBtn.AddListner(() => { fuiCom.HideAndShowPanelStackAsync<TestAPanel, TestBPanel>().Coroutine(); });

            self.FUITestAPanel.HideBtn.AddListner(() => { fuiCom.HidePanel(self); });
            Log.Info("<color=#0000FF>测试A界面 Awake</color>");
            
            self.FUITestAPanel.LanguageCombo.items = new string[] {"简体中文", "繁體中文", "English"};
            self.FUITestAPanel.LanguageCombo.selectedIndex = 0;
            self.FUITestAPanel.LanguageCombo.onChanged.Add(() =>
            {
                switch (self.FUITestAPanel.LanguageCombo.selectedIndex)
                {
                    case 0:
                        Log.Info($"点击了index=0，待补充逻辑");
                        break;
                    case 1:
                        Log.Info($"点击了index=1，待补充逻辑");
                        break;
                    case 2:
                        Log.Info($"点击了index=2，待补充逻辑");
                        break;
                }
            });
        }

        [EntitySystem]
        private static void Show(this TestAPanel self)
        {
            Log.Info("<color=#0000FF>测试A界面 Show</color>");
        }

        [EntitySystem]
        private static void Hide(this TestAPanel self)
        {
            Log.Info("<color=#0000FF>测试A界面 Hide</color>");
        }

        [EntitySystem]
        private static void BeforeUnload(this TestAPanel self)
        {
            Log.Info("<color=#0000FF>测试A界面 BeforeUnload</color>");
        }
    }
}