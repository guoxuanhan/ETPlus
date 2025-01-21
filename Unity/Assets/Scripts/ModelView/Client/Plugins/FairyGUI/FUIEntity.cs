using FairyGUI;
using UnityEngine;

namespace ET.Client
{
    [ChildOf]
    public class FUIEntity : Entity, IAwake, IDestroy
    {
        public bool Visible
        {
            get
            {
                return this.GComponent.visible;
            }

            set
            {
                this.GComponent.visible = value;
            }
        }
        
        public bool IsPreLoad
        {
            get
            {
                return this.GComponent != null;
            }
        }
        
        public PanelId PanelId
        {
            get
            {
                if (this.panelId == PanelId.Invalid)
                {
                    Log.Error("panel id is " + PanelId.Invalid);
                }
                return this.panelId;
            }
            set { this.panelId = value; }
        }
      
        private PanelId panelId = PanelId.Invalid;

        public GComponent GComponent { get; set; }

        public PanelCoreData PanelCoreData { get; set; }

        public SystemLanguage Language { get; set; }

        /// <summary>
        ///  是否加入显隐栈
        /// </summary>
        public bool IsUsingStack
        {
            get => (this.PanelCoreData.panelType & (UIPanelType.Bottom | UIPanelType.Fixed | UIPanelType.Other)) == 0;
        }

        /// <summary>
        /// 面板逻辑组件
        /// </summary>
        public EntityRef<Entity> Component { get; set; }
    }
}