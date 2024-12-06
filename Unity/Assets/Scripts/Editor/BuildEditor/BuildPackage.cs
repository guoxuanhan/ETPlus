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
        [MenuItem("ET/BuildPackage/一键打包Windows", false, ETMenuItemPriority.BuildPackage)]
        public static void AutomationBuild()
        {
            GlobalConfig globalConfig = AssetDatabase.LoadAssetAtPath<GlobalConfig>("Assets/Resources/GlobalConfig.asset");
            if (globalConfig.CodeMode != CodeMode.Client)
            {
                Debug.LogError($"Current CodeMode: {globalConfig.CodeMode}");
                return;
            }

            if (!Define.EnableIL2CPP)
            {
                Debug.LogError($"Current not enable il2cpp");
                return;
            }

            AssemblyTool.DoCompile();
            CompileDllCommand.CompileDllActiveBuildTarget();
            PrebuildCommand.GenerateAll();
            HybridCLREditor.CopyAotDll();
            AssetDatabase.Refresh();

            string packageVersion = GetBuildPackageVersion();
            BuildInternal(BuildTarget.StandaloneWindows64, Application.dataPath + "/../BuildPackage/Windows", packageVersion);
            AssetDatabase.Refresh();
            BuildImp(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64,
                $"{Application.dataPath}/../BuildPackage/Windows/Windows-{packageVersion}/ETPlus.exe");
        }

        [MenuItem("ET/BuildPackage/一键打包Android", false, ETMenuItemPriority.BuildPackage)]
        public static void AutomationBuildAndroid()
        {
            GlobalConfig globalConfig = AssetDatabase.LoadAssetAtPath<GlobalConfig>("Assets/Resources/GlobalConfig.asset");
            if (globalConfig.CodeMode != CodeMode.Client)
            {
                Debug.LogError($"Current CodeMode: {globalConfig.CodeMode}");
                return;
            }

            if (!Define.EnableIL2CPP)
            {
                Debug.LogError($"Current not enable il2cpp");
                return;
            }

            AssemblyTool.DoCompile();
            CompileDllCommand.CompileDllActiveBuildTarget();
            PrebuildCommand.GenerateAll();
            HybridCLREditor.CopyAotDll();
            AssetDatabase.Refresh();

            string packageVersion = GetBuildPackageVersion();
            BuildInternal(BuildTarget.Android, outputRoot: Application.dataPath + "/../BuildPackage/Android", packageVersion);
            AssetDatabase.Refresh();
            BuildImp(BuildTargetGroup.Android, BuildTarget.Android, $"{Application.dataPath}/../BuildPackage/Android/Android-{packageVersion}.apk");
        }

        [MenuItem("ET/BuildPackage/一键打包IOS", false, ETMenuItemPriority.BuildPackage)]
        public static void AutomationBuildIOS()
        {
            GlobalConfig globalConfig = AssetDatabase.LoadAssetAtPath<GlobalConfig>("Assets/Resources/GlobalConfig.asset");
            if (globalConfig.CodeMode != CodeMode.Client)
            {
                Debug.LogError($"Current CodeMode: {globalConfig.CodeMode}");
                return;
            }

            if (!Define.EnableIL2CPP)
            {
                Debug.LogError($"Current not enable il2cpp");
                return;
            }

            AssemblyTool.DoCompile();
            CompileDllCommand.CompileDllActiveBuildTarget();
            PrebuildCommand.GenerateAll();
            HybridCLREditor.CopyAotDll();
            AssetDatabase.Refresh();

            string packageVersion = GetBuildPackageVersion();
            BuildInternal(BuildTarget.iOS, outputRoot: Application.dataPath + "/../BuildPackage/IOS", packageVersion);
            AssetDatabase.Refresh();
            BuildImp(BuildTargetGroup.iOS, BuildTarget.iOS, $"{Application.dataPath}/../BuildPackage/IOS/XCode_Project");
        }

        private static void BuildInternal(BuildTarget buildTarget, string outputRoot, string packageVersion = "1.0")
        {
            Debug.Log($"开始构建 : {buildTarget}");
            string packageName = "DefaultPackage";

            // 构建参数
            ScriptableBuildParameters buildParameters = new ScriptableBuildParameters();
            // BuiltinBuildParameters buildParameters = new BuiltinBuildParameters();
            buildParameters.BuildOutputRoot = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot();
            buildParameters.BuildinFileRoot = AssetBundleBuilderHelper.GetStreamingAssetsRoot();
            buildParameters.BuildPipeline = EBuildPipeline.ScriptableBuildPipeline.ToString();
            // buildParameters.BuildPipeline = EBuildPipeline.BuiltinBuildPipeline.ToString();
            buildParameters.BuildTarget = buildTarget;
            buildParameters.BuildMode = EBuildMode.IncrementalBuild;
            buildParameters.PackageName = packageName;
            buildParameters.PackageVersion = packageVersion;
            buildParameters.EnableSharePackRule = true;
            buildParameters.VerifyBuildingResult = true;
            buildParameters.FileNameStyle = AssetBundleBuilderSetting.GetPackageFileNameStyle(packageName, EBuildPipeline.ScriptableBuildPipeline);
            buildParameters.BuildinFileCopyOption = EBuildinFileCopyOption.ClearAndCopyAll;
            buildParameters.BuildinFileCopyParams =
                    AssetBundleBuilderSetting.GetPackageBuildinFileCopyParams(packageName, EBuildPipeline.ScriptableBuildPipeline);
            // buildParameters.EncryptionServices = CreateEncryptionInstance();
            buildParameters.CompressOption = ECompressOption.LZMA;

            // 执行构建
            ScriptableBuildPipeline pipeline = new ScriptableBuildPipeline();
            // BuiltinBuildPipeline pipeline = new BuiltinBuildPipeline();
            var buildResult = pipeline.Run(buildParameters, true);
            if (buildResult.Success)
            {
                Debug.Log($"构建成功 : {buildResult.OutputPackageDirectory}");
                // EditorUtility.RevealInFinder(buildResult.OutputPackageDirectory);
            }
            else
            {
                Debug.LogError($"构建失败 : {buildResult.ErrorInfo}");
            }
        }

        // 构建版本相关
        private static string GetBuildPackageVersion()
        {
            int totalMinutes = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
            return DateTime.Now.ToString("yyyy-MM-dd") + "-" + totalMinutes;
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
                Debug.Log("打包异常，尚未添加Scene");
                return;
            }

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                // scenes = new[] { "Assets/Scenes/Init.unity" },
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
                Debug.Log($"Build success: {summary.totalSize / 1024 / 1024} MB");
                EditorUtility.RevealInFinder(locationPathName);
            }
            else
            {
                Debug.Log($"Build Failed" + summary.result);
            }
        }
    }
}