using System.Linq;

namespace ET.Client
{
    public static partial class FUIComponentSystem
    {
        public static void ClosePanel(this FUIComponent self, PanelId panelId)
        {
            FUIEntity fuiEntity = self.GetFUIEntity(panelId);
            self.ClosePanel(fuiEntity);
        }

        public static void ClosePanel(this FUIComponent self, long id)
        {
            FUIEntity fuiEntity = self.GetFUIEntity(id);
            self.ClosePanel(fuiEntity);
        }

        public static void ClosePanel<T>(this FUIComponent self) where T : Entity
        {
            PanelId panelId = self.GetPanelIdByGeneric<T>();
            self.ClosePanel(panelId);
        }

        public static void ClosePanel(this FUIComponent self, Entity entity)
        {
            FUIEntity fuiEntity = entity.GetParent<FUIEntity>();
            self.ClosePanel(fuiEntity);
        }

        public static void CloseLastPanel(this FUIComponent self)
        {
            if (self.VisiblePanelList.Count <= 0)
            {
                return;
            }

            PanelId panelId = self.VisiblePanelList[self.VisiblePanelList.Count - 1];
            self.ClosePanel(panelId);
        }

        public static void CloseAllPanels(this FUIComponent self, UIPanelType ignore = UIPanelType.Bottom | UIPanelType.Fixed | UIPanelType.Other)
        {
            if (self.IdEntityDict.Count <= 0)
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

                if ((fuiEntity.PanelCoreData.panelType & ignore) != 0)
                {
                    continue;
                }

                self.ClosePanel(fuiEntity);
            }
        }

        public static void ForceCloseAllPanels(this FUIComponent self)
        {
            if (self.IdEntityDict.Count <= 0)
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

                self.ClosePanel(fuiEntity);
            }

            self.AllPanelDict.Clear();
            self.IdEntityDict.Clear();
            self.VisiblePanelList.Clear();
            self.HidePanelList.Clear();
            self.VisiblePanelTypeDict.Clear();
        }

        public static void CloseAllPanelsByType(this FUIComponent self, UIPanelType panelType = UIPanelType.PopUp, PanelId ignore = PanelId.Invalid)
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
                    self.ClosePanel(fuiEntity);
                }
            }
        }

        private static void ClosePanel(this FUIComponent self, FUIEntity fuiEntity)
        {
            if (self.SetPanelClose(fuiEntity))
            {
                self.UnloadPanel(fuiEntity);
            }
        }

        private static bool SetPanelClose(this FUIComponent self, FUIEntity fuiEntity)
        {
            if (fuiEntity == null || fuiEntity.IsDisposed)
            {
                return false;
            }

            fuiEntity.Visible = false;

            self.VisiblePanelList.Remove(fuiEntity.PanelId);
            self.HidePanelList.Remove(fuiEntity.PanelId);
            self.VisiblePanelTypeDict.Remove(fuiEntity.PanelCoreData.panelType, fuiEntity.PanelId);

            Log.Info("<color=magenta>### close panel without Pop </color>{0}".Fmt(fuiEntity.PanelId));
            return true;
        }

        private static void UnloadPanel(this FUIComponent self, FUIEntity fuiEntity, bool isDespose = true)
        {
            if (fuiEntity == null)
            {
                return;
            }

            FUIEntitySystemSingleton.Instance.BeforeUnload(fuiEntity.Component);

            if (fuiEntity.IsPreLoad)
            {
                fuiEntity.GComponent.Dispose();
                fuiEntity.GComponent = null;
            }

            if (isDespose)
            {
                self.AllPanelDict.RemoveByValue(fuiEntity.Id);
                self.IdEntityDict.Remove(fuiEntity.Id);
                fuiEntity.Dispose();
            }
        }
    }
}