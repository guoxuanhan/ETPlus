using System;
using System.IO;
using System.Reflection;
using FUIEditor;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ET
{
    public enum PlatformType
    {
        None,
        Android,
        IOS,
        Windows,
        MacOS,
        Linux
    }

    public enum BuildType
    {
        Development,
        Release,
    }

    public class BuildEditor: EditorWindow
    {
        private PlatformType activePlatform;
        private PlatformType platformType;
        private bool clearFolder;
        private bool isBuildExe;
        private bool isContainAB;
        private string fairyGUIXMLPath;
        private CodeOptimization codeOptimization = CodeOptimization.Debug;
        private BuildOptions buildOptions;
        private BuildAssetBundleOptions buildAssetBundleOptions = BuildAssetBundleOptions.None;

        private GlobalConfig globalConfig;

        #region ==================== ALT+F1 ~ ALT+F5 快捷键调用 ====================

        [MenuItem("ET/导出Proto代码 _&F1", priority = 196)]
        public static void BuildProtoCodes()
        {
            ToolsEditor.Proto2CS();
        }

        [MenuItem("ET/检查Excel配置 _&F2", priority = 197)]
        public static void BuildExcelCheck()
        {
            ToolsEditor.ExcelChecker();
        }

        [MenuItem("ET/导出Excel配置（含代码） _&F3", priority = 198)]
        public static void BuildExcelDatas()
        {
            ToolsEditor.ExcelExporter();
            AssetDatabase.Refresh();
        }

        [MenuItem("ET/导出FairyGUI代码 _&F4", priority = 199)]
        public static void BuildFUICodes()
        {
            FUICodeSpawner.FUICodeSpawn();
            ShowNotification("FUI代码生成成功！");
        }

        [MenuItem("ET/编译代码 _&F5", false, 200)]
        public static void F5GenerateProjectFiles()
        {
#if ENABLE_CODES
            Debug.LogError("当前开启了ENABLE_CODES 宏, 请关闭后再编译Hotfix.dll 和 Model.dll");
            return;
#endif
            if (Unity.CodeEditor.CodeEditor.CurrentEditor.GetType().Name == "RiderScriptEditor")
            {
                FieldInfo generator = Unity.CodeEditor.CodeEditor.CurrentEditor.GetType()
                        .GetField("m_ProjectGeneration", BindingFlags.Static | BindingFlags.NonPublic);
                var syncMethod = generator.FieldType.GetMethod("Sync");
                syncMethod.Invoke(generator.GetValue(Unity.CodeEditor.CodeEditor.CurrentEditor), null);
            }
            else
            {
                Unity.CodeEditor.CodeEditor.CurrentEditor.SyncAll();
            }

            Debug.Log("ReGenerateProjectFiles finished.");

            var globalConfig = AssetDatabase.LoadAssetAtPath<GlobalConfig>("Assets/Bundles/Config/GlobalConfig/GlobalConfig.asset");
            BuildAssembliesHelper.BuildModel(UnityEditor.Compilation.CodeOptimization.Debug, globalConfig);
            BuildAssembliesHelper.BuildHotfix(UnityEditor.Compilation.CodeOptimization.Debug, globalConfig);

            ShowNotification($"{globalConfig.CodeMode}  Build Model And Hotfix Success!");

            if (Application.isPlaying)
            {
                CodeLoader.Instance.LoadHotfix();
                EventSystem.Instance.Load();
            }
            else
            {
                AssetDatabase.Refresh();
            }
        }

        #endregion

        [MenuItem("ET/Build Tool")]
        public static void ShowWindow()
        {
            GetWindow<BuildEditor>(DockDefine.Types);
        }

        private void OnEnable()
        {
            globalConfig = AssetDatabase.LoadAssetAtPath<GlobalConfig>("Assets/Bundles/Config/GlobalConfig/GlobalConfig.asset");

#if UNITY_ANDROID
            activePlatform = PlatformType.Android;
#elif UNITY_IOS
			activePlatform = PlatformType.IOS;
#elif UNITY_STANDALONE_WIN
			activePlatform = PlatformType.Windows;
#elif UNITY_STANDALONE_OSX
			activePlatform = PlatformType.MacOS;
#elif UNITY_STANDALONE_LINUX
			activePlatform = PlatformType.Linux;
#else
			activePlatform = PlatformType.None;
#endif
            platformType = activePlatform;
        }

        private void OnGUI()
        {
            this.platformType = (PlatformType)EditorGUILayout.EnumPopup(platformType);
            this.clearFolder = EditorGUILayout.Toggle("clean folder? ", clearFolder);
            this.isBuildExe = EditorGUILayout.Toggle("build exe?", this.isBuildExe);
            this.isContainAB = EditorGUILayout.Toggle("contain assetsbundle?", this.isContainAB);
            this.codeOptimization = (CodeOptimization)EditorGUILayout.EnumPopup("CodeOptimization ", this.codeOptimization);
            EditorGUILayout.LabelField("BuildAssetBundleOptions ");
            this.buildAssetBundleOptions = (BuildAssetBundleOptions)EditorGUILayout.EnumFlagsField(this.buildAssetBundleOptions);

            switch (this.codeOptimization)
            {
                case CodeOptimization.None:
                case CodeOptimization.Debug:
                    this.buildOptions = BuildOptions.Development | BuildOptions.ConnectWithProfiler;
                    break;
                case CodeOptimization.Release:
                    this.buildOptions = BuildOptions.None;
                    break;
            }

            GUILayout.Space(5);

            if (GUILayout.Button("BuildPackage"))
            {
                if (this.platformType == PlatformType.None)
                {
                    ShowNotification(new GUIContent("please select platform!"));
                    return;
                }

                if (platformType != activePlatform)
                {
                    switch (EditorUtility.DisplayDialogComplex("Warning!",
                                $"current platform is {activePlatform}, if change to {platformType}, may be take a long time", "change", "cancel",
                                "no change"))
                    {
                        case 0:
                            activePlatform = platformType;
                            break;
                        case 1:
                            return;
                        case 2:
                            platformType = activePlatform;
                            break;
                    }
                }

                BuildHelper.Build(this.platformType, this.buildAssetBundleOptions, this.buildOptions, this.isBuildExe, this.isContainAB,
                    this.clearFolder);
            }

            GUILayout.Label("");
            GUILayout.Label("Code Compile：");
            EditorGUI.BeginChangeCheck();
            this.globalConfig.CodeMode = (CodeMode)EditorGUILayout.EnumPopup("CodeMode: ", this.globalConfig.CodeMode);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(this.globalConfig);
                AssetDatabase.SaveAssetIfDirty(this.globalConfig);
                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("BuildModelAndHotfix"))
            {
                if (Define.EnableCodes)
                {
                    throw new Exception("now in ENABLE_CODES mode, do not need Build!");
                }

                BuildAssembliesHelper.BuildModel(this.codeOptimization, globalConfig);
                BuildAssembliesHelper.BuildHotfix(this.codeOptimization, globalConfig);

                AfterCompiling();

                ShowNotification("Build Model And Hotfix Success!");
            }

            #region Model 和 Hotfix 得同时编译，否则热更会有问题，所以把下面的代码注释掉了。

            // if (GUILayout.Button("BuildModel"))
            // {
            // 	if (Define.EnableCodes)
            // 	{
            // 		throw new Exception("now in ENABLE_CODES mode, do not need Build!");
            // 	}
            // 	BuildAssembliesHelper.BuildModel(this.codeOptimization, globalConfig);
            //
            // 	AfterCompiling();
            // 	
            // 	ShowNotification("Build Model Success!");
            // }
            //
            // if (GUILayout.Button("BuildHotfix"))
            // {
            // 	if (Define.EnableCodes)
            // 	{
            // 		throw new Exception("now in ENABLE_CODES mode, do not need Build!");
            // 	}
            // 	BuildAssembliesHelper.BuildHotfix(this.codeOptimization, globalConfig);
            //
            // 	AfterCompiling();
            // 	
            // 	ShowNotification("Build Hotfix Success!");
            // }

            #endregion

            if (GUILayout.Button("ExcelExporter"))
            {
                //Directory.Delete("Assets/Bundles/Config", true);
                ToolsEditor.ExcelExporter();

                // 设置ab包
                AssetImporter assetImporter = AssetImporter.GetAtPath($"Assets/Bundles/Config");
                assetImporter.assetBundleName = "Config.unity3d";
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("Proto2CS"))
            {
                ToolsEditor.Proto2CS();
            }

            GUILayout.Label("");
            GUILayout.Label("FairyGUI");
            GUIContent guiContent = new GUIContent("FairyGUI语言文件XML路径：", "在 FairyGUI 里生成");
            EditorGUI.BeginChangeCheck();
            string xmlPath = EditorGUILayout.TextField(guiContent, fairyGUIXMLPath);
            if (EditorGUI.EndChangeCheck())
            {
                fairyGUIXMLPath = xmlPath;
            }

            if (GUILayout.Button("导出 FairyGUI 多语言"))
            {
                if (FUICodeSpawner.Localize(fairyGUIXMLPath))
                {
                    ShowNotification("FairyGUI 多语言导出成功！");
                }
                else
                {
                    ShowNotification("FairyGUI 多语言导出失败！");
                }
            }

            GUILayout.Space(5);
            if (GUILayout.Button("FUI代码生成"))
            {
                FUICodeSpawner.FUICodeSpawn();
                ShowNotification("FUI代码生成成功！");
            }
        }

        private static void AfterCompiling()
        {
            Directory.CreateDirectory(BuildAssembliesHelper.CodeDir);

            // 设置ab包
            AssetImporter assetImporter = AssetImporter.GetAtPath("Assets/Bundles/Code");
            assetImporter.assetBundleName = "Code.unity3d";
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("build success!");
        }

        public static void ShowNotification(string tips)
        {
            EditorWindow game = EditorWindow.GetWindow(typeof (EditorWindow).Assembly.GetType("UnityEditor.GameView"));
            game?.ShowNotification(new GUIContent($"{tips}"));
        }
    }
}