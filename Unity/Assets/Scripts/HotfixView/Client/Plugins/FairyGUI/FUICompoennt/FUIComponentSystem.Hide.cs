using System.Linq;

namespace ET.Client
{
    public static partial class FUIComponentSystem
    {
        public static bool IsPanelVisible(this FUIComponent self, PanelId panelId)
        {
            FUIEntity fuiEntity = self.GetFUIEntity(panelId);
            return fuiEntity != null && self.VisiblePanelList.Contains(fuiEntity.PanelId);
        }

        public static bool IsPanelVisible(this FUIComponent self, long id)
        {
            FUIEntity fuiEntity = self.GetFUIEntity(id);
            return fuiEntity != null && self.VisiblePanelList.Contains(fuiEntity.PanelId);
        }

        public static bool IsAnyPanelVisible(this FUIComponent self, UIPanelType panelType)
        {
            return self.VisiblePanelTypeDict[panelType].Count > 0;
        }

        public static int GetVisiblePanelCount(this FUIComponent self)
        {
            return self.VisiblePanelList.Count;
        }

        public static void HideLastPanel(this FUIComponent self)
        {
            if (self.VisiblePanelList.Count <= 0)
            {
                return;
            }

            PanelId panelId = self.VisiblePanelList[self.VisiblePanelList.Count - 1];
            if (!self.IsPanelVisible(panelId))
            {
                return;
            }

            FUIEntity fuiEntity = self.GetFUIEntity(panelId);
            self.InnerHidePanel(fuiEntity);
            FUIEntitySystemSingleton.Instance.Hide(fuiEntity.Component);
        }

        public static void HidePanel(this FUIComponent self, PanelId panelId)
        {
            if (!self.VisiblePanelList.Contains(panelId))
            {
                return;
            }

            FUIEntity fuiEntity = self.GetFUIEntity(panelId);
            self.InnerHidePanel(fuiEntity);
            FUIEntitySystemSingleton.Instance.Hide(fuiEntity.Component);
        }

        public static void HidePanel(this FUIComponent self, long id)
        {
            PanelId panelId = self.AllPanelDict.GetKeyByValue(id);
            self.HidePanel(panelId);
        }

        public static void HidePanel(this FUIComponent self, Entity entity)
        {
            FUIEntity fuiEntity = entity.GetParent<FUIEntity>();
            self.InnerHidePanel(fuiEntity);
            FUIEntitySystemSingleton.Instance.Hide(fuiEntity.Component);
        }

        public static void HidePanel<T>(this FUIComponent self) where T : Entity, IHide, new()
        {
            PanelId panelId = self.GetPanelIdByGeneric<T>();
            self.HidePanel(panelId);
        }

        public static void HideAllPanelsByType(this FUIComponent self, UIPanelType panelType = UIPanelType.PopUp, PanelId ignore = PanelId.Invalid)
        {
            if (!self.IsAnyPanelVisible(panelType))
            {
                return;
            }

            foreach (long id in self.IdEntityDict.Keys.ToArray())
            {
                FUIEntity fuiEntity = self.IdEntityDict[id];

                if (fuiEntity == null || fuiEntity.IsDisposed)
                {
                    continue;
                }

                if (fuiEntity.PanelId != ignore && (fuiEntity.PanelCoreData.panelType & panelType) != 0)
                {
                    self.InnerHidePanel(fuiEntity);
                }
            }
        }

        private static void InnerHidePanel(this FUIComponent self, FUIEntity fuiEntity)
        {
            if (fuiEntity == null || fuiEntity.IsDisposed || !fuiEntity.Visible)
            {
                return;
            }

            self.SetPanelHide(fuiEntity);
        }

        private static void SetPanelVisible(this FUIComponent self, FUIEntity fuiEntity)
        {
            if (fuiEntity == null || fuiEntity.IsDisposed)
            {
                return;
            }

            fuiEntity.Visible = true;

            if (fuiEntity.IsUsingStack)
            {
                self.VisiblePanelList.Remove(fuiEntity.PanelId);
                self.HidePanelList.Remove(fuiEntity.PanelId);
                self.VisiblePanelTypeDict.Add(fuiEntity.PanelCoreData.panelType, fuiEntity.PanelId);
                self.VisiblePanelList.Add(fuiEntity.PanelId);
            }

            Log.Info("<color=magenta>### current Navigation panel visible </color>{0}".Fmt(fuiEntity.PanelId));
        }

        private static void SetPanelHide(this FUIComponent self, FUIEntity fuiEntity)
        {
            if (fuiEntity == null || fuiEntity.IsDisposed || !fuiEntity.Visible)
            {
                return;
            }

            fuiEntity.Visible = false;

            if (fuiEntity.IsUsingStack)
            {
                self.VisiblePanelList.Remove(fuiEntity.PanelId);
                self.HidePanelList.Remove(fuiEntity.PanelId);
                self.VisiblePanelTypeDict.Remove(fuiEntity.PanelCoreData.panelType, fuiEntity.PanelId);
                self.HidePanelList.Add(fuiEntity.PanelId);
            }

            Log.Info("<color=magenta>### current Navigation panel hide </color>{0}".Fmt(fuiEntity.PanelId));
        }
    }
}