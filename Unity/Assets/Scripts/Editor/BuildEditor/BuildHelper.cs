using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ET
{
    public static class BuildHelper
    {
        private const string relativeDirPrefix = "../Release";

        public static string BuildFolder = "../Release/{0}/StreamingAssets/";
        
#if ENABLE_CODES
        [InitializeOnLoadMethod]
        public static void ReGenerateProjectFiles()
        {
            if (Unity.CodeEditor.CodeEditor.CurrentEditor.GetType().Name== "RiderScriptEditor")
            {
                FieldInfo generator = Unity.CodeEditor.CodeEditor.CurrentEditor.GetType().GetField("m_ProjectGeneration", BindingFlags.Static | BindingFlags.NonPublic);
                var syncMethod = generator.FieldType.GetMethod("Sync");
                syncMethod.Invoke(generator.GetValue(Unity.CodeEditor.CodeEditor.CurrentEditor), null);
            }
            else
            {
                Unity.CodeEditor.CodeEditor.CurrentEditor.SyncAll();
            }
            
            Debug.Log("ReGenerateProjectFiles finished.");
        }
#endif
        
#if ENABLE_AUTO_COMPILE
#if !ENABLE_CODES
        [InitializeOnLoadMethod]
        public static void ReGenerateProjectFiles()
        {
            BuildEditor.F5GenerateProjectFiles();
        }
#endif
        [MenuItem("ET/ChangeDefine/Remove ENABLE_AUTO_COMPILE")]
        public static void RemoveEnableAutoCodes()
        {
            EnableDefineSymbols("ENABLE_AUTO_COMPILE", false);
        }
#else
        [MenuItem("ET/ChangeDefine/Add ENABLE_AUTO_COMPILE")]
        public static void AddEnableAutoCodes()
        {
            EnableDefineSymbols("ENABLE_AUTO_COMPILE", true);
        }
#endif

#if ENABLE_RIDER
        [MenuItem("ET/ChangeDefine/Remove ENABLE_RIDER")]
        public static void RemoveEnableRider()
        {
            EnableDefineSymbols("ENABLE_RIDER", false);
        }
#else
        [MenuItem("ET/ChangeDefine/Add ENABLE_RIDER")]
        public static void AddEnableRider()
        {
            EnableDefineSymbols("ENABLE_RIDER", true);
        }
#endif
        
#if ENABLE_CODES
        [MenuItem("ET/ChangeDefine/Remove ENABLE_CODES")]
        public static void RemoveEnableCodes()
        {
            EnableDefineSymbols("ENABLE_CODES", false);
        }
#else
        [MenuItem("ET/ChangeDefine/Add ENABLE_CODES")]
        public static void AddEnableCodes()
        {
            EnableDefineSymbols("ENABLE_CODES", true);
        }
#endif
        
#if ENABLE_VIEW
        [MenuItem("ET/ChangeDefine/Remove ENABLE_VIEW")]
        public static void RemoveEnableView()
        {
            EnableDefineSymbols("ENABLE_VIEW", false);
        }
#else
        [MenuItem("ET/ChangeDefine/Add ENABLE_VIEW")]
        public static void AddEnableView()
        {
            EnableDefineSymbols("ENABLE_VIEW", true);
        }
#endif
        
        private static void EnableDefineSymbols(string symbols, bool enable)
        {
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            var ss = defines.Split(';').ToList();
            if (enable)
            {
                if (ss.Contains(symbols))
                {
                    return;
                }
                ss.Add(symbols);
            }
            else
            {
                if (!ss.Contains(symbols))
                {
                    return;
                }
                ss.Remove(symbols);
            }
            
            BuildEditor.ShowNotification($"EnableDefineSymbols {symbols} {enable}");
            defines = string.Join(";", ss);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines);
            AssetDatabase.SaveAssets();
        }

        public static void Build(PlatformType type, BuildAssetBundleOptions buildAssetBundleOptions, BuildOptions buildOptions, bool isBuildExe, bool isContainAB, bool clearFolder)
        {
            BuildTarget buildTarget = BuildTarget.StandaloneWindows;
            string programName = "ET";
            string exeName = programName;
            switch (type)
            {
                case PlatformType.Windows:
                    buildTarget = BuildTarget.StandaloneWindows64;
                    exeName += ".exe";
                    break;
                case PlatformType.Android:
                    buildTarget = BuildTarget.Android;
                    exeName += ".apk";
                    break;
                case PlatformType.IOS:
                    buildTarget = BuildTarget.iOS;
                    break;
                case PlatformType.MacOS:
                    buildTarget = BuildTarget.StandaloneOSX;
                    break;
                
                case PlatformType.Linux:
                    buildTarget = BuildTarget.StandaloneLinux64;
                    break;
            }

            string fold = string.Format(BuildFolder, type);

            if (clearFolder && Directory.Exists(fold))
            {
                Directory.Delete(fold, true);
            }
            Directory.CreateDirectory(fold);

            UnityEngine.Debug.Log("start build assetbundle");
            BuildPipeline.BuildAssetBundles(fold, buildAssetBundleOptions, buildTarget);

            UnityEngine.Debug.Log("finish build assetbundle");

            if (isContainAB)
            {
                FileHelper.CleanDirectory("Assets/StreamingAssets/");
                FileHelper.CopyDirectory(fold, "Assets/StreamingAssets/");
            }

            if (isBuildExe)
            {
                AssetDatabase.Refresh();
                string[] levels = {
                    "Assets/Init.unity",
                };
                UnityEngine.Debug.Log("start build exe");
                BuildPipeline.BuildPlayer(levels, $"{relativeDirPrefix}/{exeName}", buildTarget, buildOptions);
                UnityEngine.Debug.Log("finish build exe");
            }
            else
            {
                if (isContainAB && type == PlatformType.Windows)
                {
                    string targetPath = Path.Combine(relativeDirPrefix, $"{programName}_Data/StreamingAssets/");
                    FileHelper.CleanDirectory(targetPath);
                    Debug.Log($"src dir: {fold}    target: {targetPath}");
                    FileHelper.CopyDirectory(fold, targetPath);
                }
            }
        }
    }
}
