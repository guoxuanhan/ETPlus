using System;

namespace ET.Client
{
    [Flags]
    public enum UIPanelType
    {
        Bottom = 0,         // 底层界面，可作为背景（永不销毁/关闭，不会加入显隐栈）
        Normal = 1 << 0,    // 普通主界面
        Second = 1 << 1,    // 二级界面
        PopUp = 1 << 2,     // 弹出窗口
        Fixed = 1 << 3,     // 固定窗口（永不销毁/关闭，不会加入显隐栈）
        Other = 1 << 4,     // 其他窗口（永不销毁/关闭，不会加入显隐栈）
    }

    [ChildOf(typeof(FUIEntity))]
    public class PanelCoreData: Entity, IAwake
    {
        public UIPanelType panelType { get; set; } = UIPanelType.Normal;
    }
}