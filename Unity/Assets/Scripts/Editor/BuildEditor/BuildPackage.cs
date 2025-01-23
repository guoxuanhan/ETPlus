using System;
using System.Collections.Generic;
using HybridCLR.Editor.Commands;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using YooAsset.Editor;

namespace ET
{
    public static class BuildPackage
    {
        private static readonly int ScreenWidth = 1080;
        private static readonly int ScreenHeight = 1920;
        private static readonly string PackageName = "DefaultPackage";
        private static readonly string GlobalConfigFullPath = "Assets/Bundles/Config/GlobalConfig/GlobalConfig.asset";
        private static readonly string BuildPackageFullPath = Application.dataPath + "/../../BuildPackage";

        [MenuItem("ET/BuildPackage/一键打包Windows", false, ETMenuItemPriority.BuildPackage)]
        public static void AutomationBuild()
        {
            if (!ValidateBuildEnvironment()) return;

            string packageVersion = GetBuildPackageVersion();
            BuildYooAssetBundles(BuildTarget.StandaloneWindows64, packageVersion);
            BuildImp(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64,
                $"{BuildPackageFullPath}/Windows/Windows-{packageVersion}/ETPlus.exe");
        }

        [MenuItem("ET/BuildPackage/一键打包Android", false, ETMenuItemPriority.BuildPackage)]
        public static void AutomationBuildAndroid()
        {
            if (!ValidateBuildEnvironment()) return;

            string packageVersion = GetBuildPackageVersion();
            BuildYooAssetBundles(BuildTarget.Android, packageVersion);
            BuildImp(BuildTargetGroup.Android, BuildTarget.Android, $"{BuildPackageFullPath}/Android/Android-{packageVersion}.apk");
        }

        [MenuItem("ET/BuildPackage/一键打包IOS", false, ETMenuItemPriority.BuildPackage)]
        public static void AutomationBuildIOS()
        {
            if (!ValidateBuildEnvironment()) return;

            string packageVersion = GetBuildPackageVersion();
            BuildYooAssetBundles(BuildTarget.iOS, packageVersion);
            BuildImp(BuildTargetGroup.iOS, BuildTarget.iOS, $"{BuildPackageFullPath}/IOS/XCode_Project");
        }

        [MenuItem("ET/BuildPackage/一键打包WebGL", false, ETMenuItemPriority.BuildPackage)]
        public static void AutomationBuildWebGL()
        {
            if (!ValidateBuildEnvironment()) return;

            string packageVersion = GetBuildPackageVersion();
            BuildYooAssetBundles(BuildTarget.WebGL, packageVersion);
            BuildImp(BuildTargetGroup.WebGL, BuildTarget.WebGL, $"{BuildPackageFullPath}/WebGL/WebGL-{packageVersion}");
        }

        private static bool ValidateBuildEnvironment()
        {
            GlobalConfig globalConfig = AssetDatabase.LoadAssetAtPath<GlobalConfig>(GlobalConfigFullPath);
            if (globalConfig.CodeMode != CodeMode.Client)
            {
                Debug.LogError($"Current CodeMode: {globalConfig.CodeMode}");
                return false;
            }

            if (!Define.EnableIL2CPP)
            {
                Debug.LogError($"Current not enable il2cpp");
                return false;
            }

            AssemblyTool.DoCompile();
            CompileDllCommand.CompileDllActiveBuildTarget();
            PrebuildCommand.GenerateAll();
            HybridCLREditor.CopyAotDll();
            AssetDatabase.Refresh();
            return true;
        }

        private static string GetBuildPackageVersion()
        {
            int totalMinutes = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
            return DateTime.Now.ToString("yyyy-MM-dd") + "-" + totalMinutes;
        }

        private static void BuildYooAssetBundles(BuildTarget buildTarget, string packageVersion = "1.0")
        {
            Debug.Log($"开始构建 : {buildTarget}");

            // 构建参数
            ScriptableBuildParameters buildParameters = new ScriptableBuildParameters
            {
                BuildOutputRoot = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot(),
                BuildinFileRoot = AssetBundleBuilderHelper.GetStreamingAssetsRoot(),
                BuildPipeline = EBuildPipeline.ScriptableBuildPipeline.ToString(),
                BuildTarget = buildTarget,
                BuildMode = EBuildMode.IncrementalBuild,
                PackageName = PackageName,
                PackageVersion = packageVersion,
                EnableSharePackRule = true,
                VerifyBuildingResult = true,
                FileNameStyle = AssetBundleBuilderSetting.GetPackageFileNameStyle(PackageName, EBuildPipeline.ScriptableBuildPipeline),
                BuildinFileCopyOption = EBuildinFileCopyOption.ClearAndCopyAll,
                BuildinFileCopyParams =
                        AssetBundleBuilderSetting.GetPackageBuildinFileCopyParams(PackageName, EBuildPipeline.ScriptableBuildPipeline),
                CompressOption = ECompressOption.LZMA
            };

            // 执行构建
            ScriptableBuildPipeline pipeline = new ScriptableBuildPipeline();
            var buildResult = pipeline.Run(buildParameters, true);
            if (buildResult.Success)
            {
                Debug.Log($"构建成功 : {buildResult.OutputPackageDirectory}");
            }
            else
            {
                Debug.LogError($"构建失败 : {buildResult.ErrorInfo}");
            }
        }

        private static void BuildImp(BuildTargetGroup buildTargetGroup, BuildTarget buildTarget, string locationPathName)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(buildTargetGroup, BuildTarget.StandaloneWindows64);
            AssetDatabase.Refresh();
            var scenes = new List<string>();
            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                {
                    scenes.Add(scene.path);
                }
            }

            if (scenes.Count == 0)
            {
                Debug.LogError("打包异常，尚未添加Scene");
                return;
            }

            PlayerSettings.defaultScreenWidth = ScreenWidth;
            PlayerSettings.defaultScreenHeight = ScreenHeight;

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = scenes.ToArray(),
                locationPathName = locationPathName,
                targetGroup = buildTargetGroup,
                target = buildTarget,
                options = BuildOptions.None
            };
            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;
            if (summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                Debug.Log($"构建成功: {summary.totalSize / 1024 / 1024} MB");
                EditorUtility.RevealInFinder(locationPathName);
            }
            else
            {
                Debug.LogError($"构建失败: {summary.result}");
            }
        }
    }
}