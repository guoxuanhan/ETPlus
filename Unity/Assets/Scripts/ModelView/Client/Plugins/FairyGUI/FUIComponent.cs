using System.Collections.Generic;
using FairyGUI;

namespace ET.Client
{
    [ComponentOf(typeof(Scene))]
    public class FUIComponent : Entity, IAwake, IDestroy
    {
        public GComponent GRoot { get; set; }
        public GComponent BottomRoot { get; set; }
        public GComponent NormalGRoot { get; set; }
        public GComponent SecondGRoot { get; set; }
        public GComponent PopUpGRoot { get; set; }
        public GComponent FixedGRoot { get; set; }
        public GComponent OtherGRoot { get; set; }

        /// <summary>
        /// 记录面板类型Id与FUIEntityId的映射
        /// </summary>
        public DoubleMap<PanelId, long> AllPanelDict = new();

        /// <summary>
        /// 记录面板Id与FUIEntity的映射
        /// </summary>
        public Dictionary<long, EntityRef<FUIEntity>> IdEntityDict = new();

        /// <summary>
        /// 记录当前显示中的各个类型的面板类型Id
        /// </summary>
        public MultiMapSet<UIPanelType, PanelId> VisiblePanelTypeDict = new();

        /// <summary>
        /// 显示中的面板栈
        /// </summary>
        public List<PanelId> VisiblePanelList = new();

        /// <summary>
        /// 隐藏中的面板栈
        /// </summary>
        public List<PanelId> HidePanelList = new();

        // public Dictionary<PanelId, List<long>> AllPanelsDict = new();
        //
        // /// 当前打开的各个类型的界面
        // public MultiMapSet<UIPanelType, PanelId> VisiblePanelTypeDict = new();
        //
        // /// 隐藏所有界面时临时的存储
        // public List<EntityRef<FUIEntity>> VisiblePanelCache = new();
        //
        // public Stack<EntityRef<FUIEntity>> HidePanelsStack = new();
        //
        // public Dictionary<long, EntityRef<FUIEntity>> IdToEntity = new();
        //
        // // 记录正在显示的界面，避免多次打开同一种界面
        // public HashSet<PanelId> ShowingPanels = new();
    }
}