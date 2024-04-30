using UnityEngine.SceneManagement;
using YooAsset;

namespace ET.Client
{
    [ObjectSystem]
    public class ResourcesComponentAwakeSystem: AwakeSystem<ResourcesComponent>
    {
        protected override void Awake(ResourcesComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class ResourcesComponentAwakeSystem1: AwakeSystem<ResourcesComponent, string>
    {
        protected override void Awake(ResourcesComponent self, string packageName)
        {
            self.Awake(packageName);
        }
    }

    [ObjectSystem]
    public class ResourcesComponentDestroySystem: DestroySystem<ResourcesComponent>
    {
        protected override void Destroy(ResourcesComponent self)
        {
            self.Destroy();
        }
    }

    [FriendOfAttribute(typeof (ET.Client.ResourcesComponent))]
    public static class ResourcesComponentSystem
    {
        #region ========== 生命周期 ==========

        public static void Awake(this ResourcesComponent self, string packageName = "DefaultPackage")
        {
            ResourcesComponent.Instance = self;
            self.Package = YooAssets.GetPackage(packageName);
        }

        public static void Destroy(this ResourcesComponent self)
        {
            ResourcesComponent.Instance = null;
            self.PackageVersion = string.Empty;
            self.Downloader = null;

            foreach (var kv in self.DicHandlers)
            {
                self.ReleaseHandler(kv.Value);
            }

            self.DicHandlers.Clear();
            self.ForceUnloadAllAssets();
        }

        #endregion

        #region ========== 同步加载 ==========

        public static T LoadAssetSync<T>(this ResourcesComponent self, string location) where T : UnityEngine.Object
        {
            HandleBase handler;
            if (!self.DicHandlers.TryGetValue(location, out handler))
            {
                handler = self.Package.LoadAssetSync<T>(location);
                self.DicHandlers.Add(location, handler);
            }

            return ((AssetHandle)handler).AssetObject as T;
        }

        public static byte[] LoadRawFileDataSync(this ResourcesComponent self, string location)
        {
            HandleBase handler;
            if (!self.DicHandlers.TryGetValue(location, out handler))
            {
                handler = YooAssets.LoadRawFileSync(location);
                self.DicHandlers.Add(location, handler);
            }

            return (handler as RawFileHandle).GetRawFileData();
        }

        public static string LoadRawFileTextSync(this ResourcesComponent self, string location)
        {
            HandleBase handler;
            if (!self.DicHandlers.TryGetValue(location, out handler))
            {
                handler = YooAssets.LoadRawFileSync(location);
                self.DicHandlers.Add(location, handler);
            }

            return (handler as RawFileHandle).GetRawFileText();
        }

        #endregion

        #region ========== 异步加载 ==========

        public static async ETTask<T> LoadAssetAsync<T>(this ResourcesComponent self, string location) where T : UnityEngine.Object
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ResourcesLoader, location.GetHashCode()))
            {
                HandleBase handler;
                if (!self.DicHandlers.TryGetValue(location, out handler))
                {
                    handler = self.Package.LoadAssetAsync<T>(location);
                    await handler.Task;
                    self.DicHandlers.Add(location, handler);
                }

                return ((AssetHandle)handler).AssetObject as T;
            }
        }

        public static async ETTask LoadSceneAsync(this ResourcesComponent self, string location, LoadSceneMode loadSceneMode)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ResourcesLoader, location.GetHashCode()))
            {
                HandleBase handler;
                if (self.DicHandlers.TryGetValue(location, out handler))
                {
                    return;
                }

                handler = self.Package.LoadSceneAsync(location, loadSceneMode);

                await handler.Task;
                self.DicHandlers.Add(location, handler);
            }
        }

        public static async ETTask<byte[]> LoadRawFileDataAsync(this ResourcesComponent self, string location)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ResourcesLoader, location.GetHashCode()))
            {
                HandleBase handler;
                if (!self.DicHandlers.TryGetValue(location, out handler))
                {
                    handler = YooAssets.LoadRawFileAsync(location);
                    await handler.Task;
                    self.DicHandlers.Add(location, handler);
                }

                return (handler as RawFileHandle).GetRawFileData();
            }
        }

        public static async ETTask<string> LoadRawFileTextAsync(this ResourcesComponent self, string location)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ResourcesLoader, location.GetHashCode()))
            {
                HandleBase handler;
                if (!self.DicHandlers.TryGetValue(location, out handler))
                {
                    handler = YooAssets.LoadRawFileAsync(location);
                    await handler.Task;
                    self.DicHandlers.Add(location, handler);
                }

                return (handler as RawFileHandle).GetRawFileText();
            }
        }

        #endregion

        #region ========== 资源卸载 ==========

        public static void UnloadUnusedAssets(this ResourcesComponent self)
        {
            self.Package.UnloadUnusedAssets();
        }

        public static void ForceUnloadAllAssets(this ResourcesComponent self)
        {
            self.Package.ForceUnloadAllAssets();
        }

        public static void UnloadAsset(this ResourcesComponent self, string location)
        {
            HandleBase handler;
            if (self.DicHandlers.TryGetValue(location, out handler))
            {
                self.DicHandlers.Remove(location);
                self.ReleaseHandler(handler);
            }
            else
            {
                Log.Error($"卸载的资源{location}不存在!");
            }
        }

        public static void ReleaseHandler(this ResourcesComponent self, HandleBase handleBase)
        {
            switch (handleBase)
            {
                case AssetHandle handle:
                    handle.Release();
                    break;
                case AllAssetsHandle handle:
                    handle.Release();
                    break;
                case SubAssetsHandle handle:
                    handle.Release();
                    break;
                case RawFileHandle handle:
                    handle.Release();
                    break;
                case SceneHandle handle:
                    if (!handle.IsMainScene())
                    {
                        handle.UnloadAsync();
                    }

                    break;
            }
        }

        #endregion

        #region ========== 热更相关 ==========

        /// <summary>
        /// 更新版本号
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static async ETTask<int> UpdateVersionAsync(this ResourcesComponent self)
        {
            var operation = self.Package.UpdatePackageVersionAsync();
            await operation.GetAwaiter();

            if (operation.Status != EOperationStatus.Succeed)
            {
                return ErrorCode.ERR_ResourceUpdateVersionError;
            }

            self.PackageVersion = operation.PackageVersion;
            return ErrorCode.ERR_Success;
        }

        /// <summary>
        /// 更新资源清单 Manifest
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static async ETTask<int> UpdateManifestAsync(this ResourcesComponent self)
        {
            var operation = self.Package.UpdatePackageManifestAsync(self.PackageVersion);
            await operation.GetAwaiter();

            if (operation.Status != EOperationStatus.Succeed)
            {
                return ErrorCode.ERR_ResourceUpdateManifestError;
            }

            return ErrorCode.ERR_Success;
        }

        /// <summary>
        /// 创建资源下载器
        /// </summary>
        /// <param name="self"></param>
        /// <param name="downloadingMaxNum">同时下载的最大文件数</param>
        /// <param name="failedTryAgainCount">下载失败的重试次数</param>
        /// <returns></returns>
        public static void CreateDownloader(this ResourcesComponent self, int downloadingMaxNum, int failedTryAgainCount)
        {
            ResourceDownloaderOperation downloader = YooAssets.CreateResourceDownloader(downloadingMaxNum, failedTryAgainCount);
            if (downloader.TotalDownloadCount == 0)
            {
                Log.Info("YooAsset: 没有发现需要下载的资源...");
            }
            else
            {
                Log.Info($"YooAsset: 一共发现了{downloader.TotalDownloadCount}个资源需要更新下载...");
                self.Downloader = downloader;
            }
        }

        /// <summary>
        /// 使用下载器下载网络资源
        /// </summary>
        /// <param name="self"></param>
        /// <param name="onStartDownloadFileCallback"></param>
        /// <param name="onDownloadProgress"></param>
        /// <param name="onDownloadError"></param>
        /// <param name="onDownloadOver"></param>
        /// <returns></returns>
        public static async ETTask<int> DownloadWebFilesAsync(this ResourcesComponent self,
        DownloaderOperation.OnStartDownloadFile onStartDownloadFileCallback = null,
        DownloaderOperation.OnDownloadProgress onDownloadProgress = null,
        DownloaderOperation.OnDownloadError onDownloadError = null,
        DownloaderOperation.OnDownloadOver onDownloadOver = null)
        {
            if (self.Downloader == null)
            {
                return ErrorCode.ERR_Success;
            }

            // 注册下载回调
            self.Downloader.OnStartDownloadFileCallback = onStartDownloadFileCallback;
            self.Downloader.OnDownloadProgressCallback = onDownloadProgress;
            self.Downloader.OnDownloadErrorCallback = onDownloadError;
            self.Downloader.OnDownloadOverCallback = onDownloadOver;
            self.Downloader.BeginDownload();
            await self.Downloader.GetAwaiter();

            // 检测下载结果
            if (self.Downloader.Status != EOperationStatus.Succeed)
            {
                return ErrorCode.ERR_ResourceUpdateDownloadError;
            }

            return ErrorCode.ERR_Success;
        }

        #endregion
    }
}