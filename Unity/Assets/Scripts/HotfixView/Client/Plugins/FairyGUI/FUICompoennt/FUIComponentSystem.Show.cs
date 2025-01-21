namespace ET.Client
{
    [FriendOf(typeof(FUIComponent))]
    public static partial class FUIComponentSystem
    {
        public static async ETTask<FUIEntity> ShowPanelAsync(this FUIComponent self, PanelId panelId)
        {
            FUIEntity fuiEntity = await self.InnerShowPanelAsync(panelId);
            FUIEntitySystemSingleton.Instance.Show(fuiEntity.Component);
            return fuiEntity;
        }

        public static async ETTask<FUIEntity> ShowPanelAsync<P1>(this FUIComponent self, PanelId panelId, P1 p1)
                where P1 : Entity, IAwake, IShow<P1>, new()
        {
            FUIEntity fuiEntity = await self.InnerShowPanelAsync(panelId);
            FUIEntitySystemSingleton.Instance.Show(fuiEntity.Component, p1);
            return fuiEntity;
        }

        public static async ETTask<FUIEntity> ShowPanelAsync<P1, P2>(this FUIComponent self, PanelId panelId, P1 p1, P2 p2)
                where P1 : Entity, IAwake, IShow<P1, P2>, new()
        {
            FUIEntity fuiEntity = await self.InnerShowPanelAsync(panelId);
            FUIEntitySystemSingleton.Instance.Show(fuiEntity.Component, p1, p2);
            return fuiEntity;
        }

        public static async ETTask<FUIEntity> ShowPanelAsync<P1, P2, P3>(this FUIComponent self, PanelId panelId, P1 p1, P2 p2, P3 p3)
                where P1 : Entity, IAwake, IShow<P1, P2, P3>, new()
        {
            FUIEntity fuiEntity = await self.InnerShowPanelAsync(panelId);
            FUIEntitySystemSingleton.Instance.Show(fuiEntity.Component, p1, p2, p3);
            return fuiEntity;
        }

        public static async ETTask<FUIEntity> ShowPanelAsync<P1, P2, P3, P4>(this FUIComponent self, PanelId panelId, P1 p1, P2 p2, P3 p3, P4 p4)
                where P1 : Entity, IAwake, IShow<P1, P2, P3, P4>, new()
        {
            FUIEntity fuiEntity = await self.InnerShowPanelAsync(panelId);
            FUIEntitySystemSingleton.Instance.Show(fuiEntity.Component, p1, p2, p3, p4);
            return fuiEntity;
        }

        public static async ETTask<FUIEntity> ShowPanelAsync<P1, P2, P3, P4, P5>(this FUIComponent self, PanelId panelId, P1 p1, P2 p2, P3 p3, P4 p4,
        P5 p5) where P1 : Entity, IAwake, IShow<P1, P2, P3, P4, P5>, new()
        {
            FUIEntity fuiEntity = await self.InnerShowPanelAsync(panelId);
            FUIEntitySystemSingleton.Instance.Show(fuiEntity.Component, p1, p2, p3, p4, p5);
            return fuiEntity;
        }

        private static async ETTask<FUIEntity> InnerShowPanelAsync(this FUIComponent self, PanelId panelId)
        {
            if (self.VisiblePanelList.Contains(panelId))
            {
                return null;
            }

            FUIEntity fuiEntity = self.GetFUIEntity(panelId);

            if (fuiEntity == null)
            {
                fuiEntity = await self.CreatePanelAsync(panelId);
            }

            self.SetPanelVisible(fuiEntity);
            return fuiEntity;
        }

        public static async ETTask<FUIEntity> ShowPanelAsync<T>(this FUIComponent self) where T : Entity, IAwake, IShow, new()
        {
            PanelId panelId = self.GetPanelIdByGeneric<T>();
            FUIEntity fuiEntity = await self.InnerShowPanelAsync<T>(panelId);
            FUIEntitySystemSingleton.Instance.Show(fuiEntity.Component);
            return fuiEntity;
        }

        public static async ETTask<FUIEntity> ShowPanelAsync<T, P1>(this FUIComponent self, P1 p1) where T : Entity, IAwake, IShow<P1>, new()
        {
            PanelId panelId = self.GetPanelIdByGeneric<T>();
            FUIEntity fuiEntity = await self.InnerShowPanelAsync<T>(panelId);
            FUIEntitySystemSingleton.Instance.Show(fuiEntity.Component, p1);
            return fuiEntity;
        }

        public static async ETTask<FUIEntity> ShowPanelAsync<T, P1, P2>(this FUIComponent self, P1 p1, P2 p2)
                where T : Entity, IAwake, IShow<P1, P2>, new()
        {
            PanelId panelId = self.GetPanelIdByGeneric<T>();
            FUIEntity fuiEntity = await self.InnerShowPanelAsync<T>(panelId);
            FUIEntitySystemSingleton.Instance.Show(fuiEntity.Component, p1, p2);
            return fuiEntity;
        }

        public static async ETTask<FUIEntity> ShowPanelAsync<T, P1, P2, P3>(this FUIComponent self, P1 p1, P2 p2, P3 p3)
                where T : Entity, IAwake, IShow<P1, P2, P3>, new()
        {
            PanelId panelId = self.GetPanelIdByGeneric<T>();
            FUIEntity fuiEntity = await self.InnerShowPanelAsync<T>(panelId);
            FUIEntitySystemSingleton.Instance.Show(fuiEntity.Component, p1, p2, p3);
            return fuiEntity;
        }

        public static async ETTask<FUIEntity> ShowPanelAsync<T, P1, P2, P3, P4>(this FUIComponent self, P1 p1, P2 p2, P3 p3, P4 p4)
                where T : Entity, IAwake, IShow<P1, P2, P3, P4>, new()
        {
            PanelId panelId = self.GetPanelIdByGeneric<T>();
            FUIEntity fuiEntity = await self.InnerShowPanelAsync<T>(panelId);
            FUIEntitySystemSingleton.Instance.Show(fuiEntity.Component, p1, p2, p3, p4);
            return fuiEntity;
        }

        public static async ETTask<FUIEntity> ShowPanelAsync<T, P1, P2, P3, P4, P5>(this FUIComponent self, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
                where T : Entity, IAwake, IShow<P1, P2, P3, P4, P5>, new()
        {
            PanelId panelId = self.GetPanelIdByGeneric<T>();
            FUIEntity fuiEntity = await self.InnerShowPanelAsync<T>(panelId);
            FUIEntitySystemSingleton.Instance.Show(fuiEntity.Component, p1, p2, p3, p4, p5);
            return fuiEntity;
        }

        private static async ETTask<FUIEntity> InnerShowPanelAsync<T>(this FUIComponent self, PanelId panelId) where T : Entity, IAwake, new()
        {
            if (self.VisiblePanelList.Contains(panelId))
            {
                return null;
            }

            FUIEntity fuiEntity = self.GetFUIEntity(panelId);

            if (fuiEntity == null)
            {
                fuiEntity = await self.CreatePanelAsync<T>(panelId);
            }

            self.SetPanelVisible(fuiEntity);
            return fuiEntity;
        }
    }
}