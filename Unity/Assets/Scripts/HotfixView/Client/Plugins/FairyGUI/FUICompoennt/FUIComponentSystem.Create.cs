using System;
using FairyGUI;

namespace ET.Client
{
    [FriendOf(typeof(FUIComponent))]
    public static partial class FUIComponentSystem
    {
        public static async ETTask<FUIEntity> CreatePanelAsync(this FUIComponent self, PanelId panelId, long id = 0)
        {
            FUIEntity fuiEntity = await self.CreateFUIEntityAsync(panelId, id);
            return fuiEntity;
        }

        private static async ETTask<FUIEntity> CreateFUIEntityAsync(this FUIComponent self, PanelId panelId, long id = 0)
        {
            CoroutineLock coroutineLock = null;

            try
            {
                coroutineLock = await self.Root().GetComponent<CoroutineLockComponent>().Wait(CoroutineLockType.LoadingPanels, id);

                FUIEntity fuiEntity = null;

                if (id != 0)
                {
                    fuiEntity = self.AddChildWithId<FUIEntity>(id, true);
                }
                else
                {
                    fuiEntity = self.AddChild<FUIEntity>(true);
                }

                fuiEntity.PanelId = panelId;

                bool success = await self.LoadFUIEntityAsync(fuiEntity);
                if (success)
                {
                    return fuiEntity;
                }

                fuiEntity?.Dispose();
                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                coroutineLock?.Dispose();
            }
        }

        private static async ETTask<bool> LoadFUIEntityAsync(this FUIComponent self, FUIEntity fuiEntity)
        {
            if (!FUIEventComponent.Instance.TryGetPanelInfo(fuiEntity.PanelId, out PanelInfo panelInfo))
            {
                return false;
            }

            // 创建组件
            fuiEntity.GComponent = await self.CreateObjectAsync(panelInfo.PackageName, panelInfo.ComponentName);
            if (fuiEntity.GComponent == null)
            {
                return false;
            }

            // 设置全屏
            fuiEntity.GComponent.MakeFullScreen();

            // 记录FUIEntity信息
            self.AllPanelDict.Add(fuiEntity.PanelId, fuiEntity.Id);
            self.IdEntityDict.Add(fuiEntity.Id, fuiEntity);

            // 添加逻辑组件
            Type type = CodeTypes.Instance.GetType("ET.Client" + panelInfo.ComponentName);
            fuiEntity.Component = fuiEntity.AddComponent(type);

            // 设置根节点
            fuiEntity.SetRoot(self.GetTargetRoot(fuiEntity.PanelCoreData.panelType));

            return true;
        }

        public static async ETTask<FUIEntity> CreatePanelAsync<T>(this FUIComponent self, PanelId panelId, long id = 0) where T : Entity, IAwake, new()
        {
            FUIEntity fuiEntity = await self.CreateFUIEntityAsync<T>(panelId, id);
            return fuiEntity;
        }

        private static async ETTask<FUIEntity> CreateFUIEntityAsync<T>(this FUIComponent self, PanelId panelId, long id = 0) where T : Entity, IAwake, new()
        {
            CoroutineLock coroutineLock = null;

            try
            {
                coroutineLock = await self.Root().GetComponent<CoroutineLockComponent>().Wait(CoroutineLockType.LoadingPanels, id);

                FUIEntity fuiEntity = null;

                if (id != 0)
                {
                    fuiEntity = self.AddChildWithId<FUIEntity>(id, true);
                }
                else
                {
                    fuiEntity = self.AddChild<FUIEntity>(true);
                }

                fuiEntity.PanelId = panelId;

                bool isSuccess = await self.LoadFUIEntityAsync<T>(fuiEntity);
                if (isSuccess)
                {
                    return fuiEntity;
                }

                fuiEntity?.Dispose();
                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                coroutineLock?.Dispose();
            }
        }

        private static async ETTask<bool> LoadFUIEntityAsync<T>(this FUIComponent self, FUIEntity fuiEntity) where T : Entity, IAwake, new()
        {
            if (!FUIEventComponent.Instance.TryGetPanelInfo<T>(out PanelInfo panelInfo))
            {
                return false;
            }

            // 创建组件
            fuiEntity.GComponent = await self.CreateObjectAsync(panelInfo.PackageName, panelInfo.ComponentName);
            if (fuiEntity.GComponent == null)
            {
                return false;
            }

            // 设置全屏
            fuiEntity.GComponent.MakeFullScreen();

            // 记录FUIEntity信息
            self.AllPanelDict.Add(fuiEntity.PanelId, fuiEntity.Id);
            self.IdEntityDict.Add(fuiEntity.Id, fuiEntity);

            // 添加逻辑组件
            Entity component = fuiEntity.AddComponent<T>();
            fuiEntity.Component = component;

            // 设置根节点
            fuiEntity.SetRoot(self.GetTargetRoot(fuiEntity.PanelCoreData.panelType));

            return true;
        }

        public static async ETTask<GComponent> CreateObjectAsync(this FUIComponent self, string packageName, string componentName)
        {
            return (await self.Scene().GetComponent<FUIAssetComponent>().CreateObjectAsync(packageName, componentName)).asCom;
        }

        private static GComponent GetTargetRoot(this FUIComponent self, UIPanelType panelType)
        {
            return panelType switch
            {
                UIPanelType.Bottom => self.BottomRoot,
                UIPanelType.Normal => self.NormalGRoot,
                UIPanelType.Second => self.SecondGRoot,
                UIPanelType.PopUp => self.PopUpGRoot,
                UIPanelType.Fixed => self.FixedGRoot,
                UIPanelType.Other => self.OtherGRoot,
                _ => self.NormalGRoot,
            };
        }
    }
}