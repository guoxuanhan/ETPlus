using FairyGUI;
using UnityEngine;

namespace ET.Client
{
    [EnableClass]
    public class FUIGLoaderExternal : GLoader
    {
        [StaticField]
        private static EntityRef<Scene> rootScene;

        public static Scene RootScene
        {
            get
            {
                return rootScene;
            }
            set
            {
                rootScene = value;
            }
        }

        /*
            开始外部载入，地址在url属性
            载入完成后调用OnExternalLoadSuccess
            载入失败调用OnExternalLoadFailed

            注意：如果是外部载入，在载入结束后，调用OnExternalLoadSuccess或OnExternalLoadFailed前，
            比较严谨的做法是先检查url属性是否已经和这个载入的内容不相符。
            如果不相符，表示loader已经被修改了。
            这种情况下应该放弃调用OnExternalLoadSuccess或OnExternalLoadFailed。
        */
        protected override void LoadExternal()
        {
            string nativeUrl = this.url;
            Texture2D texture2D = RootScene.GetComponent<ResourcesLoaderComponent>().LoadAssetSync<Texture2D>(this.url);
            if (texture2D != null)
            {
                if (nativeUrl == this.url)
                {
                    onExternalLoadSuccess(new NTexture(texture2D));
                }
                else
                {
                    this.onExternalLoadFailed();
                }
            }
            else
            {
                this.onExternalLoadFailed();
            }
        }

        protected override void FreeExternal(NTexture texture)
        {
            RootScene.GetComponent<ResourcesLoaderComponent>().UnloadAsset(this.url);
        }
    }
}