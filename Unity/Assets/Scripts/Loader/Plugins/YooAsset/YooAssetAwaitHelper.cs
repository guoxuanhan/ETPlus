using YooAsset;

namespace ET
{
    public static class YooAssetAwaitHelper
    {
        public static ETTask GetAwaiter(this AsyncOperationBase asyncOperationBase)
        {
            ETTask task = ETTask.Create(true);
            asyncOperationBase.Completed += _ => { task.SetResult(); };
            return task;
        }

        public static ETTask GetAwaiter(this AssetHandle assetOperationHandle)
        {
            ETTask task = ETTask.Create(true);
            assetOperationHandle.Completed += _ => { task.SetResult(); };
            return task;
        }

        public static ETTask GetAwaiter(this SubAssetsHandle subAssetsOperationHandle)
        {
            ETTask task = ETTask.Create(true);
            subAssetsOperationHandle.Completed += _ => { task.SetResult(); };
            return task;
        }

        public static ETTask GetAwaiter(this SceneHandle sceneOperationHandle)
        {
            ETTask task = ETTask.Create(true);
            sceneOperationHandle.Completed += _ => { task.SetResult(); };
            return task;
        }

        public static ETTask GetAwaiter(this RawFileHandle assetOperationHandle)
        {
            ETTask task = ETTask.Create(true);
            assetOperationHandle.Completed += _ => { task.SetResult(); };
            return task;
        }
    }
}