namespace ET.Client
{
    [EntitySystemOf(typeof(TestCPanel))]
    [FriendOf(typeof(TestCPanel))]
    public static partial class TestCPanelSystem
    {
        [EntitySystem]
        private static void Awake(this TestCPanel self)
        {
            self.FUITestCPanel.Loader1.url = "ui://Icon1/Icon1";
            self.FUITestCPanel.Loader2.url = "ui://96tfczmnnt9r1";
            self.FUITestCPanel.Loader3.url = "ui://Icon3/IconCom";

            self.FUITestCPanel.CloseBtn.AddListner(() => { self.Root().GetComponent<FUIComponent>().ClosePanel(self); });

            Log.Info("<color=#FFC0CB>测试C界面 Awake</color>");
        }

        [EntitySystem]
        private static void Show(this TestCPanel self)
        {
            Log.Info("<color=#FFC0CB>测试C界面 Show</color>");
        }

        [EntitySystem]
        private static void Hide(this TestCPanel self)
        {
            Log.Info("<color=#FFC0CB>测试C界面 Hide</color>");
        }

        [EntitySystem]
        private static void BeforeUnload(this TestCPanel self)
        {
            Log.Info("<color=#FFC0CB>测试C界面 BeforeUnload</color>");
        }
    }
}