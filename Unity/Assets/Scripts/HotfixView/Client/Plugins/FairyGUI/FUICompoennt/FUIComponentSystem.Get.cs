namespace ET.Client
{
    public static partial class FUIComponentSystem
    {
        public static bool IsPanelExist(this FUIComponent self, PanelId panelId)
        {
            return self.AllPanelDict.ContainsKey(panelId);
        }

        public static bool IsPanelExist(this FUIComponent self, long id)
        {
            return self.AllPanelDict.ContainsValue(id);
        }

        public static FUIEntity GetFUIEntity(this FUIComponent self, PanelId panelId)
        {
            if (!self.IsPanelExist(panelId))
            {
                return null;
            }

            long id = self.AllPanelDict.GetValueByKey(panelId);
            return self.GetFUIEntity(id);
        }

        public static FUIEntity GetFUIEntity(this FUIComponent self, long id)
        {
            if (!self.IsPanelExist(id))
            {
                return null;
            }

            if (!self.IdEntityDict.TryGetValue(id, out EntityRef<FUIEntity> fuiEntity))
            {
                return null;
            }

            return fuiEntity;
        }

        private static PanelId GetPanelIdByGeneric<T>(this FUIComponent self) where T : Entity
        {
            if (FUIEventComponent.Instance.TryGetPanelInfo<T>(out PanelInfo panelInfo))
            {
                return panelInfo.PanelId;
            }

            return PanelId.Invalid;
        }

        public static T GetPanelLogic<T>(this FUIComponent self, bool needVisible = false) where T : Entity
        {
            PanelId panelId = self.GetPanelIdByGeneric<T>();
            FUIEntity fuiEntity = self.GetFUIEntity(panelId);
            return self.InnerGetPanelLogic<T>(fuiEntity, needVisible);
        }

        private static T InnerGetPanelLogic<T>(this FUIComponent self, FUIEntity fuiEntity, bool needVisible = false) where T : Entity
        {
            if (fuiEntity == null)
            {
                Log.Error("GetPanelLogic: fuiEntity is null");
                return null;
            }

            if (!fuiEntity.IsPreLoad)
            {
                Log.Error($"GetPanelLogic: {fuiEntity.PanelId} is not loaded!");
                return null;
            }

            if (needVisible && !fuiEntity.Visible)
            {
                Log.Warning($"GetPanelLogic: {fuiEntity.PanelId} is need visible state!");
                return null;
            }

            return fuiEntity.GetComponent<T>();
        }
    }
}