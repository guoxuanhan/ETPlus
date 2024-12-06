namespace ET.Client
{
    [EntitySystemOf(typeof(TestBPanel))]
    [FriendOf(typeof(TestBPanel))]
    public static partial class TestBPanelSystem
    {
        [EntitySystem]
        private static void Awake(this TestBPanel self)
        {
            Log.Info("<color=#FFA500>测试B界面 Awake</color>");
            
            self.FUITestBPanel.OpenTestCBtn.AddListner(() =>
            {
                var fuiCom = self.Root().GetComponent<FUIComponent>();
				
                fuiCom.HideAndShowPanelStackAsync<TestBPanel, TestCPanel>().Coroutine();
            });
			
            self.FUITestBPanel.CloseBtn.AddListner(() =>
            {
                self.Root().GetComponent<FUIComponent>().HidePanel(self);
            });
            
            self.Com1 = self.AddChild<OneComponent, ET.Client.TestB.FUI_OneComponent>(self.FUITestBPanel.Com1, true);
            self.Com2 = self.AddChild<TwoComponent, ET.Client.Common.FUI_TwoComponent>(self.FUITestBPanel.Com2, true);
        }
        
        [EntitySystem]
        private static void Show(this TestBPanel self)
        {
            Log.Info("<color=#FFA500>测试B界面 Show</color>");
            
            self.Com1.OnShow();
            self.Com2.OnShow();
        }

        [EntitySystem]
        private static void Hide(this TestBPanel self)
        {
            Log.Info("<color=#FFA500>测试B界面 Hide</color>");
            
            self.Com1.OnHide();
            self.Com2.OnHide();
        }

        [EntitySystem]
        private static void BeforeUnload(this TestBPanel self)
        {
            Log.Info("<color=#FFA500>测试B界面 BeforeUnload</color>");
            
            self.Com1.BeforeUnload();
            self.Com2.BeforeUnload();
        }
    }
}