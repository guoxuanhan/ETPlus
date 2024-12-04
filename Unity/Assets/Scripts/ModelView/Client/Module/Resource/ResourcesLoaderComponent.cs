using System.Collections.Generic;
using UnityEngine.SceneManagement;
using YooAsset;

namespace ET.Client
{
    [EntitySystemOf(typeof(ResourcesLoaderComponent))]
    [FriendOf(typeof(ResourcesLoaderComponent))]
    public static partial class ResourcesLoaderComponentSystem
    {
        #region ========== 生命周期 ==========

        [EntitySystem]
        private static void Awake(this ResourcesLoaderComponent self)
        {
            self.package = YooAssets.GetPackage("DefaultPackage");
        }

        [EntitySystem]
        private static void Awake(this ResourcesLoaderComponent self, string packageName)
        {
            self.package = YooAssets.GetPackage(packageName);
        }

        [EntitySystem]
        private static void Destroy(this ResourcesLoaderComponent self)
        {
            foreach (var kv in self.handlers)
            {
                self.ReleaseHandler(kv.Value);
            }
            
            self.ForceUnloadAllAssets();
            self.handlers.Clear();
            self.packageVersion = string.Empty;
            self.downloader = null;
        }
        
        #endregion

        #region ========== 同步加载 ==========

        public static T LoadAssetSync<T>(this ResourcesLoaderComponent self, string location) where T : UnityEngine.Object
        {
            HandleBase handler;
            if (!self.handlers.TryGetValue(location, out handler))
            {
                handler = self.package.LoadAssetSync<T>(location);
                self.handlers.Add(location, handler);
            }

            return ((AssetHandle)handler).AssetObject as T;
        }

        public static byte[] LoadRawFileDataSync(this ResourcesLoaderComponent self, string location)
        {
            HandleBase handler;
            if (!self.handlers.TryGetValue(location, out handler))
            {
                handler = YooAssets.LoadRawFileSync(location);
                self.handlers.Add(location, handler);
            }

            return (handler as RawFileHandle).GetRawFileData();
        }

        public static string LoadRawFileTextSync(this ResourcesLoaderComponent self, string location)
        {
            HandleBase handler;
            if (!self.handlers.TryGetValue(location, out handler))
            {
                handler = YooAssets.LoadRawFileSync(location);
                self.handlers.Add(location, handler);
            }

            return (handler as RawFileHandle).GetRawFileText();
        }

        #endregion

        #region ========== 异步加载 ==========

        public static async ETTask<T> LoadAssetAsync<T>(this ResourcesLoaderComponent self, string location) where T : UnityEngine.Object
        {
            using CoroutineLock coroutineLock = await self.Root().GetComponent<CoroutineLockComponent>()
                    .Wait(CoroutineLockType.ResourcesLoader, location.GetHashCode());

            HandleBase handler;
            if (!self.handlers.TryGetValue(location, out handler))
            {
                handler = self.package.LoadAssetAsync<T>(location);
                await handler.Task;
                self.handlers.Add(location, handler);
            }

            return ((AssetHandle)handler).AssetObject as T;
        }

        public static async ETTask LoadSceneAsync(this ResourcesLoaderComponent self, string location, LoadSceneMode loadSceneMode)
        {
            using CoroutineLock coroutineLock = await self.Root().GetComponent<CoroutineLockComponent>()
                    .Wait(CoroutineLockType.ResourcesLoader, location.GetHashCode());

            HandleBase handler;
            if (self.handlers.TryGetValue(location, out handler))
            {
                return;
            }

            handler = self.package.LoadSceneAsync(location);

            await handler.Task;
            self.handlers.Add(location, handler);
        }

        public static async ETTask<byte[]> LoadRawFileDataAsync(this ResourcesLoaderComponent self, string location)
        {
            using CoroutineLock coroutineLock = await self.Root().GetComponent<CoroutineLockComponent>()
                    .Wait(CoroutineLockType.ResourcesLoader, location.GetHashCode());

            HandleBase handler;
            if (!self.handlers.TryGetValue(location, out handler))
            {
                handler = YooAssets.LoadRawFileAsync(location);
                await handler.Task;
                self.handlers.Add(location, handler);
            }

            return (handler as RawFileHandle).GetRawFileData();
        }

        public static async ETTask<string> LoadRawFileTextAsync(this ResourcesLoaderComponent self, string location)
        {
            using CoroutineLock coroutineLock = await self.Root().GetComponent<CoroutineLockComponent>()
                    .Wait(CoroutineLockType.ResourcesLoader, location.GetHashCode());

            HandleBase handler;
            if (!self.handlers.TryGetValue(location, out handler))
            {
                handler = YooAssets.LoadRawFileAsync(location);
                await handler.Task;
                self.handlers.Add(location, handler);
            }

            return (handler as RawFileHandle).GetRawFileText();
        }

        public static async ETTask<Dictionary<string, T>> LoadAllAssetsAsync<T>(this ResourcesLoaderComponent self, string location)
                where T : UnityEngine.Object
        {
            using CoroutineLock coroutineLock = await self.Root().GetComponent<CoroutineLockComponent>()
                    .Wait(CoroutineLockType.ResourcesLoader, location.GetHashCode());

            HandleBase handler;
            if (!self.handlers.TryGetValue(location, out handler))
            {
                handler = self.package.LoadAllAssetsAsync<T>(location);
                await handler.Task;
                self.handlers.Add(location, handler);
            }

            Dictionary<string, T> dictionary = new Dictionary<string, T>();
            foreach (UnityEngine.Object assetObj in ((AllAssetsHandle)handler).AllAssetObjects)
            {
                T t = assetObj as T;
                dictionary.Add(t.name, t);
            }

            return dictionary;
        }

        #endregion

        #region ========== 资源卸载 ==========

        public static void UnloadUnusedAssets(this ResourcesLoaderComponent self)
        {
            self.package.UnloadUnusedAssets();
        }

        public static void ForceUnloadAllAssets(this ResourcesLoaderComponent self)
        {
            self.package.ForceUnloadAllAssets();
        }

        public static void UnloadAsset(this ResourcesLoaderComponent self, string location)
        {
            HandleBase handler;
            if (self.handlers.TryGetValue(location, out handler))
            {
                self.handlers.Remove(location);
                self.ReleaseHandler(handler);
            }
            else
            {
                Log.Error($"卸载的资源{location}不存在!");
            }
        }

        public static void ReleaseHandler(this ResourcesLoaderComponent self, HandleBase handleBase)
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
        public static async ETTask<int> UpdateVersionAsync(this ResourcesLoaderComponent self)
        {
            var operation = self.package.UpdatePackageVersionAsync();
            await operation.GetAwaiter();

            if (operation.Status != EOperationStatus.Succeed)
            {
                return ErrorCode.ERR_ResourceUpdateVersionError;
            }

            self.packageVersion = operation.PackageVersion;
            return ErrorCode.ERR_Success;
        }

        /// <summary>
        /// 更新资源清单 Manifest
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static async ETTask<int> UpdateManifestAsync(this ResourcesLoaderComponent self)
        {
            var operation = self.package.UpdatePackageManifestAsync(self.packageVersion);
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
        public static void CreateDownloader(this ResourcesLoaderComponent self, int downloadingMaxNum, int failedTryAgainCount)
        {
            ResourceDownloaderOperation downloader = YooAssets.CreateResourceDownloader(downloadingMaxNum, failedTryAgainCount);
            if (downloader.TotalDownloadCount == 0)
            {
                Log.Info("YooAsset: 没有发现需要下载的资源...");
            }
            else
            {
                Log.Info($"YooAsset: 一共发现了{downloader.TotalDownloadCount}个资源需要更新下载...");
                self.downloader = downloader;
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
        public static async ETTask<int> DownloadWebFilesAsync(this ResourcesLoaderComponent self,
        DownloaderOperation.OnStartDownloadFile onStartDownloadFileCallback = null,
        DownloaderOperation.OnDownloadProgress onDownloadProgress = null,
        DownloaderOperation.OnDownloadError onDownloadError = null,
        DownloaderOperation.OnDownloadOver onDownloadOver = null)
        {
            if (self.downloader == null)
            {
                return ErrorCode.ERR_Success;
            }

            // 注册下载回调
            self.downloader.OnStartDownloadFileCallback = onStartDownloadFileCallback;
            self.downloader.OnDownloadProgressCallback = onDownloadProgress;
            self.downloader.OnDownloadErrorCallback = onDownloadError;
            self.downloader.OnDownloadOverCallback = onDownloadOver;
            self.downloader.BeginDownload();
            await self.downloader.GetAwaiter();

            // 检测下载结果
            if (self.downloader.Status != EOperationStatus.Succeed)
            {
                return ErrorCode.ERR_ResourceUpdateDownloadError;
            }

            return ErrorCode.ERR_Success;
        }

        #endregion
    }

    /// <summary>
    /// 用来管理资源，生命周期跟随Parent，比如CurrentScene用到的资源应该用CurrentScene的ResourcesLoaderComponent来加载
    /// 这样CurrentScene释放后，它用到的所有资源都释放了
    /// </summary>
    [ComponentOf]
    public class ResourcesLoaderComponent : Entity, IAwake, IAwake<string>, IDestroy
    {
        public string packageVersion;
        public ResourcePackage package;
        public ResourceDownloaderOperation downloader;
        public Dictionary<string, HandleBase> handlers = new();
    }
}