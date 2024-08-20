using System.Collections.Generic;
using FairyGUI;
using FairyGUI.Dynamic;
using UnityEngine;

namespace ET.Client
{
    [ObjectSystem]
    public class FUIAssetComponentAwakeSystem: AwakeSystem<FUIAssetComponent, bool>
    {
        protected override void Awake(FUIAssetComponent self, bool unloadUnusedUIPackageImmediately)
        {
            self.Awake(unloadUnusedUIPackageImmediately);
        }
    }

    [ObjectSystem]
    public class FUIAssetComponentDestroySystem: DestroySystem<FUIAssetComponent>
    {
        protected override void Destroy(FUIAssetComponent self)
        {
            self.Destroy();
        }
    }

    [FriendOf(typeof (FUIAssetComponent))]
    public static class FUIAssetComponentSystem
    {
        public static void Awake(this FUIAssetComponent self, bool unloadUnusedUIPackageImmediately)
        {
            self.UnloadUnusedUIPackageImmediately = unloadUnusedUIPackageImmediately;
            
            void LoadUIPackageAsyncHandler(string packageName, LoadUIPackageBytesCallback callback)
            {
                self.LoadUIPackageAsyncInner(packageName, callback).Coroutine();
            }

            void LoadTextureAsyncHandler(string packageName, string assetName, string extension, LoadTextureCallback callback)
            {
                self.LoadTextureAsyncInner(assetName, callback).Coroutine();
            }

            void LoadAudioClipAsyncHandler(string packageName, string assetName, string extension, LoadAudioClipCallback callback)
            {
                self.LoadAudioClipAsyncInner(assetName, callback).Coroutine();
            }

            self.Locations = new Dictionary<int, string>();
            var assetLoader = new DelegateUIAssetLoader();
            assetLoader.LoadUIPackageBytesAsyncHandlerImpl = LoadUIPackageAsyncHandler;
            assetLoader.LoadUIPackageBytesHandlerImpl = self.LoadUIPackageSyncInner;
            assetLoader.LoadTextureAsyncHandlerImpl = LoadTextureAsyncHandler;
            assetLoader.UnloadTextureHandlerImpl = self.UnloadAssetInner;
            assetLoader.LoadAudioClipAsyncHandlerImpl = LoadAudioClipAsyncHandler;
            assetLoader.UnloadAudioClipHandlerImpl = self.UnloadAssetInner;

            self.AssetLoader = assetLoader;
            
            byte[] mappingData = ResourcesComponent.Instance.LoadAssetSync<TextAsset>("UIPackageMapping").bytes;
            self.PackageHelper = new UIPackageMapping(mappingData);

            self.UIAssetManager = new UIAssetManager();
            self.UIAssetManager.Initialize(self);
        }
        
        public static ETTask<GObject> CreateObjectFromURLAsync(this FUIAssetComponent self, string url)
        {
            ETTask<GObject> task = ETTask<GObject>.Create(true);
            UIPackage.CreateObjectFromURLAsync(url, result =>
            {
                task.SetResult(result);
            });
            return task;
        }

        public static ETTask<GObject> CreateObjectAsync(this FUIAssetComponent self, string pkgName, string resName)
        {
            ETTask<GObject> task = ETTask<GObject>.Create(true);
            UIPackage.CreateObjectAsync(pkgName, resName, result =>
            {
                task.SetResult(result);
            });
            return task;
        }
        
        public static GObject CreateObject(this FUIAssetComponent self, string pkgName, string resName)
        {
            return UIPackage.CreateObject(pkgName, resName);
        }
        
        public static void UnloadUnusedUIPackages(this FUIAssetComponent self)
        {
            UIPackage.RemoveUnusedPackages();
        }

        public static void Destroy(this FUIAssetComponent self)
        {
            self.UIAssetManager.Dispose();
            self.UIAssetManager = null;
            self.AssetLoader = null;

            if (ResourcesComponent.Instance != null && !ResourcesComponent.Instance.IsDisposed)
            {
                foreach (string location in self.Locations.Values)
                {
                    ResourcesComponent.Instance.UnloadAsset(location);
                }
            }

            self.Locations.Clear();
        }

        private static void LoadUIPackageSyncInner(this FUIAssetComponent self, string packageName, out byte[] bytes, out string assetNamePrefix)
        {
            string location = "{0}{1}".Fmt(packageName, "_fui");
            byte[] descData = ResourcesComponent.Instance.LoadAssetSync<TextAsset>(location).bytes;
            ResourcesComponent.Instance.UnloadAsset(location);
            
            bytes = descData;
            assetNamePrefix = packageName;
        }

        private static async ETTask LoadUIPackageAsyncInner(this FUIAssetComponent self, string packageName, LoadUIPackageBytesCallback callback)
        {
            string location = "{0}{1}".Fmt(packageName, "_fui");
            var textAsset = await ResourcesComponent.Instance.LoadAssetAsync<TextAsset>(location);
            if (textAsset != null)
            {
                byte[] descData = textAsset.bytes;
                ResourcesComponent.Instance.UnloadAsset(location);

                callback(descData, packageName);
            }
        }

        private static async ETTask LoadTextureAsyncInner(this FUIAssetComponent self, string assetName, LoadTextureCallback callback)
        {
            Texture res = await ResourcesComponent.Instance.LoadAssetAsync<Texture>(assetName);

            if (res != null)
                self.Locations[res.GetInstanceID()] = assetName;

            callback(res);
        }

        private static async ETTask LoadAudioClipAsyncInner(this FUIAssetComponent self, string assetName, LoadAudioClipCallback callback)
        {
            AudioClip res = await ResourcesComponent.Instance.LoadAssetAsync<AudioClip>(assetName);

            if (res != null)
                self.Locations[res.GetInstanceID()] = assetName;

            callback(res);
        }

        private static void UnloadAssetInner(this FUIAssetComponent self, UnityEngine.Object obj)
        {
            if (obj == null)
                return;

            int instanceId = obj.GetInstanceID();
            if (!self.Locations.TryGetValue(instanceId, out string location))
                return;

            self.Locations.Remove(instanceId);

            if (ResourcesComponent.Instance != null && !ResourcesComponent.Instance.IsDisposed)
            {
                ResourcesComponent.Instance.UnloadAsset(location);
            }
        }
    }
}