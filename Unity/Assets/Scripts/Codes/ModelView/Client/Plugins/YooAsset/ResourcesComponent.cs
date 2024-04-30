using System.Collections.Generic;
using YooAsset;

namespace ET.Client
{
    [ComponentOf(typeof (Scene))]
    public class ResourcesComponent: Entity, IAwake, IAwake<string>, IDestroy
    {
        [StaticField]
        public static ResourcesComponent Instance;

        public string PackageVersion { get; set; }

        public ResourcePackage Package { get; set; }

        public ResourceDownloaderOperation Downloader { get; set; }

        public Dictionary<string, HandleBase> DicHandlers = new(100);
    }
}