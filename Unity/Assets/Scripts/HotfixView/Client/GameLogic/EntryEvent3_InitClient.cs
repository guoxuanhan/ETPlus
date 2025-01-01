using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ET.Client
{
    [Event(SceneType.Main)]
    public class EntryEvent3_InitClient : AEvent<Scene, EntryEvent3>
    {
        protected override async ETTask Run(Scene root, EntryEvent3 args)
        {
            GlobalComponent globalComponent = root.AddComponent<GlobalComponent>();
            root.AddComponent<UIGlobalComponent>();
            root.AddComponent<UIComponent>();
            root.AddComponent<ResourcesLoaderComponent>();
            root.AddComponent<PlayerComponent>();
            root.AddComponent<CurrentScenesComponent>();

            root.AddComponent<FUIAssetComponent, bool>(false);
            root.AddComponent<FUIComponent>();
            
            // 根据配置修改掉Main Fiber的SceneType
            SceneType sceneType = EnumHelper.FromString<SceneType>(globalComponent.GlobalConfig.AppType.ToString());
            root.SceneType = sceneType;
            
            await EventSystem.Instance.PublishAsync(root, new AppStartInitFinish());
        }

        private static async ETTask HotUpdateAsync(Scene root)
        {
            // 打开热更界面
            await root.GetComponent<FUIComponent>().ShowPanelAsync<HotUpdatePanel>();

            int errorCode = ErrorCode.ERR_Success;

            // 更新版本号
            errorCode = await root.GetComponent<ResourcesLoaderComponent>().UpdateVersionAsync();
            if (errorCode != ErrorCode.ERR_Success)
            {
                Log.Error($"获取版本号出错...");
                return;
            }

            // 更新Manifest
            errorCode = await root.GetComponent<ResourcesLoaderComponent>().UpdateManifestAsync();
            if (errorCode != ErrorCode.ERR_Success)
            {
                Log.Error($"获取资源清单出错...");
                return;
            }

            // 创建下载器
            root.GetComponent<ResourcesLoaderComponent>().CreateDownloader(10, 3);

            // 下载器不为空 说明有资源需要下载
            if (root.GetComponent<ResourcesLoaderComponent>().GetDownloader() != null)
            {
                await DownloadPatch(root);
            }
            else
            {
                await EnterGame(root);
            }
        }

        private static async ETTask DownloadPatch(Scene root)
        {
            var downloader = root.GetComponent<ResourcesLoaderComponent>().GetDownloader();
            long totalCount = downloader.TotalDownloadCount;
            long totalBytes = downloader.TotalDownloadBytes;

            Log.Info($"需要下载资源数：{totalCount} 大小：{totalBytes}");

            // 下载资源
            int errorCode = await root.GetComponent<ResourcesLoaderComponent>().DownloadWebFilesAsync(
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
            
            // 代码更新可能要restart暂定，只是资源更新就直接进入游戏。
            await EnterGame(root);
        }

        private static async ETTask EnterGame(Scene root)
        {
            // 重启UI组件
            root.GetComponent<FUIComponent>().Restart();

            // 关闭热更新界面
            root.GetComponent<FUIComponent>().ClosePanel(PanelId.HotUpdatePanel);

            // 展示登录界面
            await root.GetComponent<FUIComponent>().ShowPanelAsync<LoginPanel>();

            await EventSystem.Instance.PublishAsync(root, new AppStartInitFinish());
        }
    }
}