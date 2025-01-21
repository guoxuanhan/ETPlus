using FairyGUI;

namespace ET.Client
{
    [EntitySystemOf(typeof(FUIComponent))]
    [FriendOf(typeof(FUIComponent))]
    public static partial class FUIComponentSystem
    {
        [EntitySystem]
        public static void Awake(this FUIComponent self)
        {
            // 设置分辨率
            GRoot.inst.SetContentScaleFactor(1080, 1920, UIContentScaler.ScreenMatchMode.MatchWidthOrHeight);
            
            self.GRoot = GRoot.inst;

            self.BottomRoot = new GComponent();
            self.BottomRoot.gameObjectName = "BottomRoot";
            GRoot.inst.AddChild(self.BottomRoot);
            
            self.NormalGRoot = new GComponent();
            self.NormalGRoot.gameObjectName = "NormalGRoot";
            GRoot.inst.AddChild(self.NormalGRoot);
            
            self.SecondGRoot = new GComponent();
            self.SecondGRoot.gameObjectName = "SecondGRoot";
            GRoot.inst.AddChild(self.SecondGRoot);
            
            self.PopUpGRoot = new GComponent();
            self.PopUpGRoot.gameObjectName = "PopUpGRoot";
            GRoot.inst.AddChild(self.PopUpGRoot);
            
            self.FixedGRoot = new GComponent();
            self.FixedGRoot.gameObjectName = "FixedGRoot";
            GRoot.inst.AddChild(self.FixedGRoot);
            
            self.OtherGRoot = new GComponent();
            self.OtherGRoot.gameObjectName = "OtherGRoot";
            GRoot.inst.AddChild(self.OtherGRoot);

            self.SetLoaderExtension();
            FUIBinder.BindAll();
        }
        
        [EntitySystem]
        public static void Destroy(this FUIComponent self)
        {
            self.ForceCloseAllPanels();
        }
        
        public static void Restart(this FUIComponent self)
        {
            self.CloseAllPanels();
            
            FUIBinder.BindAll();
        }

        /// <summary>
        /// 添加GLoader扩展类处理
        /// </summary>
        /// <param name="self"></param>
        private static void SetLoaderExtension(this FUIComponent self)
        {
            FUIGLoaderExternal.RootScene = self.Root();
            UIObjectFactory.SetLoaderExtension(typeof(FUIGLoaderExternal));
        }
    }
}