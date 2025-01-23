using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using YooAsset;

namespace ET
{
    /// <summary>
    /// 主要用来加载dll config aotdll，因为这时候纤程还没创建，无法使用ResourcesLoaderComponent。
    /// 游戏中的资源应该使用ResourcesLoaderComponent来加载
    /// </summary>
    public class ResourcesComponent : Singleton<ResourcesComponent>, ISingletonAwake
    {
        private ResourcePackage defaultPackage { get; set; }
        
        public void Awake()
        {
            YooAssets.Initialize();
#if !UNITY_WEBGL
            YooAssets.SetCacheSystemDisableCacheOnWebGL();
#endif
        }

        protected override void Destroy()
        {
            YooAssets.Destroy();
        }

        public void DestroyPackage(string packageName)
        {
            ResourcePackage package = YooAssets.GetPackage(packageName);
            package.UnloadUnusedAssets();
        }

        public async ETTask CreatePackageAsync(EPlayMode ePlayMode, string packageName, bool isDefault = false)
        {
            defaultPackage = YooAssets.TryGetPackage(packageName);
            if (defaultPackage == null)
            {
                defaultPackage = YooAssets.CreatePackage(packageName);
            }
            
            if (isDefault)
            {
                YooAssets.SetDefaultPackage(defaultPackage);
            }
            
            InitializationOperation initializationOperation = null;

            // 编辑器下的模拟模式
            switch (ePlayMode)
            {
                case EPlayMode.EditorSimulateMode:
                {
                    EditorSimulateModeParameters createParameters = new();
                    createParameters.SimulateManifestFilePath =
                            EditorSimulateModeHelper.SimulateBuild(EDefaultBuildPipeline.ScriptableBuildPipeline, packageName);
                    initializationOperation = defaultPackage.InitializeAsync(createParameters);
                    break;
                }
                case EPlayMode.OfflinePlayMode:
                {
                    OfflinePlayModeParameters createParameters = new();
                    createParameters.DecryptionServices = new FileOffsetDecryption();
                    initializationOperation = defaultPackage.InitializeAsync(createParameters);
                    break;
                }
                case EPlayMode.HostPlayMode:
                {
                    string defaultHostServer = GetHostServerURL(defaultPackage.PackageName);
                    string fallbackHostServer = GetHostServerURL(defaultPackage.PackageName);
                    HostPlayModeParameters createParameters = new();
                    createParameters.DecryptionServices = new FileStreamDecryption();
                    createParameters.BuildinQueryServices = new GameQueryServices();
                    createParameters.RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
                    initializationOperation = defaultPackage.InitializeAsync(createParameters);
                    break;
                }
                case EPlayMode.WebPlayMode:
                {
                    string defaultHostServer = GetHostServerURL(defaultPackage.PackageName);
                    string fallbackHostServer = GetHostServerURL(defaultPackage.PackageName);
                    WebPlayModeParameters createParameters = new();
                    createParameters.DecryptionServices = new FileStreamDecryption();
                    createParameters.BuildinQueryServices = new WebGLGameQueryServices();
                    createParameters.RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
                    initializationOperation = defaultPackage.InitializeAsync(createParameters);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            await initializationOperation.Task;

            // 如果初始化失败弹出提示界面
            if (initializationOperation.Status != EOperationStatus.Succeed)
            {
                Log.Error($"YooAsset资源包初始化失败!");
            }

            await LoadGlobalConfigAsync();
        }
        
        /// <summary>
        /// 获取资源服务器地址
        /// </summary>
        /// <param name="pacakgeName"></param>
        /// <returns></returns>
        private string GetHostServerURL(string pacakgeName)
        {
            //string hostServerIP = "http://10.0.2.2"; //安卓模拟器地址
            string hostServerIP = "http://127.0.0.1";
            string appVersion = "v1.0";

#if UNITY_EDITOR
            switch (UnityEditor.EditorUserBuildSettings.activeBuildTarget)
            {
                case UnityEditor.BuildTarget.Android:
                    return $"{hostServerIP}/CDN/Android/{appVersion}";
                case UnityEditor.BuildTarget.iOS:
                    return $"{hostServerIP}/CDN/IPhone/{appVersion}";
                case UnityEditor.BuildTarget.WebGL:
                    return $"{hostServerIP}/webgl/StreamingAssets/Bundles/{pacakgeName}";
                default:
                    return $"{hostServerIP}/CDN/PC/{appVersion}";
            }
#else
		    switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    return $"{hostServerIP}/CDN/Android/{appVersion}";
                case RuntimePlatform.IPhonePlayer:
                    return $"{hostServerIP}/CDN/IPhone/{appVersion}";
                case RuntimePlatform.WebGLPlayer:
                    return $"{hostServerIP}/webgl/StreamingAssets/Bundles/{pacakgeName}";
                default:
                    return $"{hostServerIP}/CDN/PC/{appVersion}";
            }
#endif
        }
        
        /// <summary>
        /// 远端资源地址查询服务类
        /// </summary>
        private class RemoteServices : IRemoteServices
        {
            private readonly string _defaultHostServer;
            private readonly string _fallbackHostServer;

            public RemoteServices(string defaultHostServer, string fallbackHostServer)
            {
                _defaultHostServer = defaultHostServer;
                _fallbackHostServer = fallbackHostServer;
            }

            string IRemoteServices.GetRemoteMainURL(string fileName)
            {
                return $"{_defaultHostServer}/{fileName}";
            }

            string IRemoteServices.GetRemoteFallbackURL(string fileName)
            {
                return $"{_fallbackHostServer}/{fileName}";
            }
        }

        /// <summary>
        /// 资源文件流加载解密类
        /// </summary>
        private class FileStreamDecryption : IDecryptionServices
        {
            /// <summary>
            /// 同步方式获取解密的资源包对象
            /// 注意：加载流对象在资源包对象释放的时候会自动释放
            /// </summary>
            AssetBundle IDecryptionServices.LoadAssetBundle(DecryptFileInfo fileInfo, out Stream managedStream)
            {
                BundleStream bundleStream = new BundleStream(fileInfo.FileLoadPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                managedStream = bundleStream;
                return AssetBundle.LoadFromStream(bundleStream, fileInfo.ConentCRC, GetManagedReadBufferSize());
            }

            /// <summary>
            /// 异步方式获取解密的资源包对象
            /// 注意：加载流对象在资源包对象释放的时候会自动释放
            /// </summary>
            AssetBundleCreateRequest IDecryptionServices.LoadAssetBundleAsync(DecryptFileInfo fileInfo, out Stream managedStream)
            {
                BundleStream bundleStream = new BundleStream(fileInfo.FileLoadPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                managedStream = bundleStream;
                return AssetBundle.LoadFromStreamAsync(bundleStream, fileInfo.ConentCRC, GetManagedReadBufferSize());
            }

            private static uint GetManagedReadBufferSize()
            {
                return 1024;
            }
        }

        /// <summary>
        /// 资源文件偏移加载解密类
        /// </summary>
        private class FileOffsetDecryption : IDecryptionServices
        {
            /// <summary>
            /// 同步方式获取解密的资源包对象
            /// 注意：加载流对象在资源包对象释放的时候会自动释放
            /// </summary>
            AssetBundle IDecryptionServices.LoadAssetBundle(DecryptFileInfo fileInfo, out Stream managedStream)
            {
                managedStream = null;
                return AssetBundle.LoadFromFile(fileInfo.FileLoadPath, fileInfo.ConentCRC, GetFileOffset());
            }

            /// <summary>
            /// 异步方式获取解密的资源包对象
            /// 注意：加载流对象在资源包对象释放的时候会自动释放
            /// </summary>
            AssetBundleCreateRequest IDecryptionServices.LoadAssetBundleAsync(DecryptFileInfo fileInfo, out Stream managedStream)
            {
                managedStream = null;
                return AssetBundle.LoadFromFileAsync(fileInfo.FileLoadPath, fileInfo.ConentCRC, GetFileOffset());
            }

            private static ulong GetFileOffset()
            {
                return 32;
            }
        }

        public T LoadAssetSync<T>(string location) where T : UnityEngine.Object
        {
            AssetHandle handle = YooAssets.LoadAssetSync<T>(location);
            T t = handle.AssetObject as T;
            handle.Release();
            return t;
        }

        public async ETTask<T> LoadAssetAsync<T>(string location) where T : UnityEngine.Object
        {
            AssetHandle handle = YooAssets.LoadAssetAsync<T>(location);
            await handle.Task;
            T t = (T)handle.AssetObject;
            handle.Release();
            return t;
        }

        public async ETTask<Dictionary<string, T>> LoadAllAssetsAsync<T>(string location) where T : UnityEngine.Object
        {
            AllAssetsHandle allAssetsOperationHandle = YooAssets.LoadAllAssetsAsync<T>(location);
            await allAssetsOperationHandle.Task;
            Dictionary<string, T> dictionary = new Dictionary<string, T>();
            foreach (UnityEngine.Object assetObj in allAssetsOperationHandle.AllAssetObjects)
            {
                T t = assetObj as T;
                dictionary.Add(t.name, t);
            }

            allAssetsOperationHandle.Release();
            return dictionary;
        }
        
        public async ETTask LoadGlobalConfigAsync()
        {
            AssetHandle handler = YooAssets.LoadAssetAsync<GlobalConfig>("GlobalConfig");
            await handler.Task;
            GlobalConfig.Instance = handler.AssetObject as GlobalConfig;
            handler.Release();
            defaultPackage.UnloadUnusedAssets();
        }
        
        public List<string> GetAddressesByTag(string tag)
        {
            AssetInfo[] assetInfos = YooAssets.GetAssetInfos(tag);

            List<string> result = new(assetInfos.Length);

            foreach (var assetInfo in assetInfos)
            {
                result.Add(assetInfo.Address);
            }

            return result;
        }
    }
}