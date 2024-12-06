namespace ET.Client
{
    [EntitySystemOf(typeof(TwoComponent))]
    [FriendOf(typeof(TwoComponent))]
    public static partial class TwoComponentSystem
    {
        [EntitySystem]
        private static void Awake(this TwoComponent self, ET.Client.Common.FUI_TwoComponent fuiTwoComponent)
        {
            self.FUITwoComponent = fuiTwoComponent;
            
            self.FUITwoComponent.OneBtn.AddListner(() =>
            {
                Log.Info("TwoComponentSystem OneBtn Click");
            });
            
            Log.Info("<color=#000000>组件2 Awake</color>");
        }

        public static void OnShow(this TwoComponent self, Entity contextData = null)
        {
            Log.Info("<color=#000000>组件2 OnShow</color>");
        }

        public static void OnHide(this TwoComponent self)
        {
            Log.Info("<color=#000000>组件2 OnHide</color>");
        }

        public static void BeforeUnload(this TwoComponent self)
        {
            Log.Info("<color=#000000>组件2 BeforeUnload</color>");
        }
    }
}