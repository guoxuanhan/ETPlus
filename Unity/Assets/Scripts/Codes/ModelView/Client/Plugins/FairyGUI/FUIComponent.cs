using System.Collections.Generic;
using FairyGUI;

namespace ET.Client
{
    [ComponentOf(typeof (Scene))]
    public class FUIComponent: Entity, IAwake<int, int>, IDestroy
    {
        [StaticField]
        public static FUIComponent Instance;

        public List<PanelId> VisiblePanelsQueue = new List<PanelId>(10);

        public Dictionary<int, FUIEntity> AllPanelsDic = new Dictionary<int, FUIEntity>(10);

        public List<PanelId> FUIEntitylistCached = new List<PanelId>(10);

        public Dictionary<int, FUIEntity> VisiblePanelsDic = new Dictionary<int, FUIEntity>(10);

        public Stack<PanelId> HidePanelsStack = new Stack<PanelId>(10);
    }
}