namespace ET.Client
{
    [EntitySystemOf(typeof(OneComponent))]
    [FriendOf(typeof(OneComponent))]
    public static partial class OneComponentSystem
    {
        [EntitySystem]
        private static void Awake(this OneComponent self, ET.Client.TestB.FUI_OneComponent fuiOneComponent)
        {
            self.FUIOneComponent = fuiOneComponent;
            self.TwoCom = self.AddChild<TwoComponent, ET.Client.Common.FUI_TwoComponent>(self.FUIOneComponent.TwoCom, true);
            
            Log.Info("<color=#808080>组件1 Awake</color>");
        }
        
        public static void OnShow(this OneComponent self, Entity contextData = null)
        {
            Log.Info("<color=#808080>组件1 OnShow</color>");
            self.TwoCom.OnShow();
        }

        public static void OnHide(this OneComponent self)
        {
            Log.Info("<color=#808080>组件1 OnHide</color>");
            self.TwoCom.OnHide();
        }

        public static void BeforeUnload(this OneComponent self)
        {
            Log.Info("<color=#808080>组件1 BeforeUnload</color>");
            self.TwoCom.BeforeUnload();
        }
    }
}