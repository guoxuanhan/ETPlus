using HybridCLR;
using UnityEngine;

namespace ET
{
    public static class HybridCLRHelper
    {
        public static void Load()
        {
            var addressList = MonoResourcesComponent.Instance.GetAddressesByTag("aotdlls");

            foreach (var address in addressList)
            {
                TextAsset textAsset = MonoResourcesComponent.Instance.LoadAssetSync<TextAsset>(address);

                byte[] bytes = textAsset.bytes;

                var errorCode = RuntimeApi.LoadMetadataForAOTAssembly(bytes, HomologousImageMode.Consistent);

                Debug.Log($"LoadMetadataForAOTAssembly. {address} return => {errorCode}");
            }
        }
    }
}