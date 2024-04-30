using UnityEngine;

namespace ET.Client
{
    [Event(SceneType.Process)]
    public class EntryEvent3_InitClient: AEvent<ET.EventType.EntryEvent3>
    {
        protected override async ETTask Run(Scene scene, ET.EventType.EntryEvent3 args)
        {
            Root.Instance.Scene.AddComponent<ResourcesComponent>();

            Root.Instance.Scene.AddComponent<GlobalComponent>();

            Scene clientScene = await SceneFactory.CreateClientScene(1, "Game");
            
            // 开始热更新流程
            await HotUpdateAsync(clientScene);
        }

        private static async ETTask HotUpdateAsync(Scene scene)
        {
            // TODO: 打开热更新界面

            // 更新版本号
            int errorCode = 0;
            errorCode = await ResourcesComponent.Instance.UpdateVersionAsync();
            if (errorCode != ErrorCode.ERR_Success)
            {
                Log.Error($"获取版本号出错...");
                return;
            }

            // 更新资源清单
            errorCode = await ResourcesComponent.Instance.UpdateManifestAsync();
            if (errorCode != ErrorCode.ERR_Success)
            {
                Log.Error($"获取资源清单出错...");
                return;
            }

            // 创建下载器
            ResourcesComponent.Instance.CreateDownloader(10, 3);

            // 下载器不为空 说明有资源需要下载
            if (ResourcesComponent.Instance.Downloader != null)
            {
                await DownloadPatch(scene);
            }
            else
            {
                await EnterGame(scene);
            }
        }

        private static async ETTask DownloadPatch(Scene scene)
        {
            long totalCount = ResourcesComponent.Instance.Downloader.TotalDownloadCount;
            long totalBytes = ResourcesComponent.Instance.Downloader.TotalDownloadBytes;

            Log.Info($"需要下载资源数：{totalCount} 大小：{totalBytes}");

            // 下载资源
            int errorCode = await ResourcesComponent.Instance.DownloadWebFilesAsync(
                // 开始下载回调
                null,

                // 下载进度回调
                (totalDownloadCount, currentDownloadCount, totalDownloadBytes, currentDownloadBytes) =>
                {
                    string currentSizeMB = (currentDownloadBytes / 1048576f).ToString("f1");
                    string totalSizeMB = (totalDownloadBytes / 1048576f).ToString("f1");
                    string text = $"资源下载中：{currentDownloadCount}/{totalDownloadCount} {currentSizeMB}MB/{totalSizeMB}MB";
                    Log.Info(text);

                    // m_updateInfo.text = text;
                    //m_processbar.value = currentDownloadBytes * 1.0f / totalDownloadBytes * 100;

                    // 更新进度条
                    // EventSystem.Instance.Publish(scene, new OnPatchDownloadProgress() { TotalDownloadCount = totalDownloadCount, CurrentDownloadCount = currentDownloadCount, TotalDownloadSizeBytes = totalDownloadBytes, CurrentDownloadSizeBytes = currentDownloadBytes });
                },

                // 下载失败回调
                (fileName, error) =>
                {
                    // 下载失败
                    // EventSystem.Instance.Publish(scene, new OnPatchDownlodFailed() { FileName = fileName, Error = error });
                },

                // 下载完成回调
                null);

            if (errorCode != ErrorCode.ERR_Success)
            {
                // TODO: 弹出错误提示，确定后重试。
                Log.Error($"下载资源失败！{errorCode}");
                return;
            }

            int modelVersion = GlobalConfig.Instance.ModelVersion;
            int hotfixVersion = GlobalConfig.Instance.HotfixVersion;
            await MonoResourcesComponent.Instance.LoadGlobalConfigAsync();

            bool codeChanged = modelVersion != GlobalConfig.Instance.ModelVersion || hotfixVersion != GlobalConfig.Instance.HotfixVersion;
            if (codeChanged)
            {
                // 如果dll文件有更新，则需要重启。
                GameObject.Find("Global").GetComponent<Init>().ReStart().Coroutine();
            }
            else
            {
                await EnterGame(scene);
            }
        }

        private static async ETTask EnterGame(Scene scene)
        {
            // TODO：隐藏热更新界面 展示登录界面
            // scene.GetComponent<UIComponent>().HideWindow(WindowID.WindowID_HotUpdate);
            // scene.GetComponent<UIComponent>().CloseAllWindow();
            // await scene.GetComponent<UIComponent>().ShowWindowAsync(WindowID.WindowID_Login);
            // 只是资源更新就直接进入游戏。
            await EventSystem.Instance.PublishAsync(scene, new ET.EventType.AppStartInitFinish());
        }
    }
}