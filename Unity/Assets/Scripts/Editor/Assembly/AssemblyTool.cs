using System.IO;
using System.Threading;
using FUIEditor;
using UnityEditor;
using UnityEditor.Build.Player;
using UnityEngine;

namespace ET
{
    public static class AssemblyTool
    {
        /// <summary>
        /// Unity线程的同步上下文
        /// </summary>
        static SynchronizationContext unitySynchronizationContext { get; set; }

        /// <summary>
        /// 程序集名字数组
        /// </summary>
        public static readonly string[] DllNames = { "Unity.Hotfix", "Unity.HotfixView", "Unity.Model", "Unity.ModelView" };

        [InitializeOnLoadMethod]
        static void Initialize()
        {
            unitySynchronizationContext = SynchronizationContext.Current;
        }
        
        #region ==================== ALT+F1 ~ ALT+F6 快捷键调用 ====================

        [MenuItem("ET/导出Proto代码 _&F1", false, ETMenuItemPriority.ExportProto)]
        public static void BuildProtoCodes()
        {
            ToolsEditor.Proto2CS();
        }

        [MenuItem("ET/检查Excel配置 _&F2", false, ETMenuItemPriority.CheckExcel)]
        public static void BuildExcelCheck()
        {
            ToolsEditor.ExcelChecker();
        }

        [MenuItem("ET/导出Excel配置（含代码） _&F3", false, ETMenuItemPriority.ExportExcel)]
        public static void BuildExcelDatas()
        {
            ToolsEditor.ExcelExporter();
            AssetDatabase.Refresh();
        }
        
        [MenuItem("ET/导出FairyGUI代码 _&F4", false, ETMenuItemPriority.ExportFGUICode)]
        public static void BuildFUICodes()
        {
            FUICodeSpawner.FUICodeSpawn();
            AssetDatabase.Refresh();
            Log.Info($"Generage FGUI Code Finish!");
        }

        [MenuItem("ET/编译代码 _&F5", false, ETMenuItemPriority.Compile)]
        public static void F5GenerateProjectFiles()
        {
            // 强制刷新一下，防止关闭auto refresh，文件修改时间不准确
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

            DoCompile();
        }
        
        /// <summary>
        /// 菜单和快捷键热重载按钮
        /// </summary>
        [MenuItem("ET/热重载代码 _&F6", false, ETMenuItemPriority.Reload)]
        static void MenuItemOfReload()
        {
            if (Application.isPlaying)
            {
                CodeLoader.Instance?.Reload();
            }
        }
        
        #endregion

        /// <summary>
        /// 执行编译代码流程
        /// </summary>
        public static void DoCompile()
        {
            // 强制刷新一下，防止关闭auto refresh，编译出老代码
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

            RefreshCodeMode();
            RefreshBuildType();
            RefreshAppType();

            bool isCompileOk = CompileDlls();
            if (!isCompileOk)
            {
                return;
            }

            CopyHotUpdateDlls();
            BuildHelper.ReGenerateProjectFiles();
            
            Log.Info($"Compile Finish!");
        }

        /// <summary>
        /// 刷新代码模式
        /// </summary>
        static void RefreshCodeMode()
        {
            CodeMode codeMode = CodeMode.ClientServer;
            GlobalConfig globalConfig = AssetDatabase.LoadAssetAtPath<GlobalConfig>("Assets/Bundles/Config/GlobalConfig/GlobalConfig.asset");
            if (globalConfig)
            {
                codeMode = globalConfig.CodeMode;
            }

            switch (codeMode)
            {
                case CodeMode.Client:
                {
                    EnableUnityClient();
                    break;
                }
                case CodeMode.Server:
                {
                    EnableUnityServer();
                    break;
                }
                case CodeMode.ClientServer:
                {
                    EnableUnityClientServer();
                    break;
                }
            }

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 刷新构建类型
        /// </summary>
        static void RefreshBuildType()
        {
            BuildType buildType = BuildType.Release;
            GlobalConfig globalConfig = AssetDatabase.LoadAssetAtPath<GlobalConfig>("Assets/Bundles/Config/GlobalConfig/GlobalConfig.asset");
            if (globalConfig)
            {
                buildType = globalConfig.BuildType;
            }

            EditorUserBuildSettings.development = buildType == BuildType.Debug;
        }

        /// <summary>
        /// 刷新应用类型
        /// </summary>
        static void RefreshAppType()
        {
            AppType appType = AppType.Demo;
            GlobalConfig globalConfig = AssetDatabase.LoadAssetAtPath<GlobalConfig>("Assets/Bundles/Config/GlobalConfig/GlobalConfig.asset");
            if (globalConfig)
            {
                appType = globalConfig.AppType;
            }

            switch (appType)
            {
                case AppType.Demo:
                    EnableAppTypeDemo();
                    DotNetCSProjHelper.ExcludeFolderRef(new[] { "GameLogic" });
                    break;
                case AppType.LockStep:
                    EnableAppTypeLockStep();
                    DotNetCSProjHelper.ExcludeFolderRef(new[] { "GameLogic" });
                    break;
                case AppType.GameLogic:
                    EnableAppTypeGameLogic();
                    DotNetCSProjHelper.ExcludeFolderRef(new[] { "Demo", "LockStep" });
                    break;
            }
            
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 编译成dll
        /// </summary>
        static bool CompileDlls()
        {
            // 运行时编译需要先设置为UnitySynchronizationContext, 编译完再还原为CurrentContext
            SynchronizationContext lastSynchronizationContext = Application.isPlaying ? SynchronizationContext.Current : null;
            SynchronizationContext.SetSynchronizationContext(unitySynchronizationContext);

            bool isCompileOk = false;

            try
            {
                Directory.CreateDirectory(Define.BuildOutputDir);
                BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
                BuildTargetGroup group = BuildPipeline.GetBuildTargetGroup(target);
                ScriptCompilationSettings scriptCompilationSettings = new()
                {
                    group = group,
                    target = target,
                    extraScriptingDefines = new[] { "UNITY_COMPILE" },
                    options = EditorUserBuildSettings.development ? ScriptCompilationOptions.DevelopmentBuild : ScriptCompilationOptions.None
                };
                ScriptCompilationResult result = PlayerBuildInterface.CompilePlayerScripts(scriptCompilationSettings, Define.BuildOutputDir);
                isCompileOk = result.assemblies.Count > 0;
                EditorUtility.ClearProgressBar();
            }
            finally
            {
                if (lastSynchronizationContext != null)
                {
                    SynchronizationContext.SetSynchronizationContext(lastSynchronizationContext);
                }
            }

            return isCompileOk;
        }

        /// <summary>
        /// 将dll文件复制到加载目录
        /// </summary>
        static void CopyHotUpdateDlls()
        {
            FileHelper.CleanDirectory(Define.CodeDir);
            
            GlobalConfig globalConfig = AssetDatabase.LoadAssetAtPath<GlobalConfig>("Assets/Bundles/Config/GlobalConfig/GlobalConfig.asset");
            ++globalConfig.CodeVersion;
            EditorUtility.SetDirty(globalConfig);
            AssetDatabase.SaveAssets();
            
            foreach (string dllName in DllNames)
            {
                string sourceDll = $"{Define.BuildOutputDir}/{dllName}.dll";
                string sourcePdb = $"{Define.BuildOutputDir}/{dllName}.pdb";
                File.Copy(sourceDll, $"{Define.CodeDir}/{dllName}_{globalConfig.CodeVersion}.dll.bytes", true);
                File.Copy(sourcePdb, $"{Define.CodeDir}/{dllName}_{globalConfig.CodeVersion}.pdb.bytes", true);
            }

            AssetDatabase.Refresh();
        }

        #region ========== CodeMode 程序集 ==========
        
        /// <summary>
        /// 启用纯客户端模式
        /// </summary>
        static void EnableUnityClient()
        {
            DisableAsmdef("Assets/Scripts/Model/Generate/Client/Ignore.asmdef");
            EnableAsmdef("Assets/Scripts/Model/Generate/Server/Ignore.asmdef");
            EnableAsmdef("Assets/Scripts/Model/Generate/ClientServer/Ignore.asmdef");

            DisableAsmdef("Assets/Scripts/Model/Client/Ignore.asmdef");
            EnableAsmdef("Assets/Scripts/Model/Server/Ignore.asmdef");

            DisableAsmdef("Assets/Scripts/Hotfix/Client/Ignore.asmdef");
            EnableAsmdef("Assets/Scripts/Hotfix/Server/Ignore.asmdef");

            DisableAsmdef("Assets/Scripts/ModelView/Client/Ignore.asmdef");
            DisableAsmdef("Assets/Scripts/HotfixView/Client/Ignore.asmdef");
        }

        /// <summary>
        /// 启用纯服务端模式
        /// </summary>
        static void EnableUnityServer()
        {
            EnableAsmdef("Assets/Scripts/Model/Generate/Client/Ignore.asmdef");
            EnableAsmdef("Assets/Scripts/Model/Generate/Server/Ignore.asmdef");
            DisableAsmdef("Assets/Scripts/Model/Generate/ClientServer/Ignore.asmdef");

            DisableAsmdef("Assets/Scripts/Model/Client/Ignore.asmdef");
            DisableAsmdef("Assets/Scripts/Model/Server/Ignore.asmdef");

            DisableAsmdef("Assets/Scripts/Hotfix/Client/Ignore.asmdef");
            DisableAsmdef("Assets/Scripts/Hotfix/Server/Ignore.asmdef");

            EnableAsmdef("Assets/Scripts/HotfixView/Client/Ignore.asmdef");
            EnableAsmdef("Assets/Scripts/ModelView/Client/Ignore.asmdef");
        }

        /// <summary>
        /// 启用双端模式
        /// </summary>
        static void EnableUnityClientServer()
        {
            EnableAsmdef("Assets/Scripts/Model/Generate/Client/Ignore.asmdef");
            EnableAsmdef("Assets/Scripts/Model/Generate/Server/Ignore.asmdef");
            DisableAsmdef("Assets/Scripts/Model/Generate/ClientServer/Ignore.asmdef");

            DisableAsmdef("Assets/Scripts/Model/Client/Ignore.asmdef");
            DisableAsmdef("Assets/Scripts/Model/Server/Ignore.asmdef");

            DisableAsmdef("Assets/Scripts/Hotfix/Client/Ignore.asmdef");
            DisableAsmdef("Assets/Scripts/Hotfix/Server/Ignore.asmdef");

            DisableAsmdef("Assets/Scripts/HotfixView/Client/Ignore.asmdef");
            DisableAsmdef("Assets/Scripts/ModelView/Client/Ignore.asmdef");
        }

        #endregion
        
        #region ========== AppType 程序集 ==========

        static void EnableAppTypeDemo()
        {
            // 《Enable》
            // Hotfix-Client-GameLogic
            EnableAsmdef("Assets/Scripts/Hotfix/Client/GameLogic/Ignore.asmdef");
            // Hotfix-Server-GameLogic
            EnableAsmdef("Assets/Scripts/Hotfix/Server/GameLogic/Ignore.asmdef");
            // Hotfix-Share-GameLogic
            EnableAsmdef("Assets/Scripts/Hotfix/Share/GameLogic/Ignore.asmdef");
            
            // HotfixView-Client-GameLogic
            EnableAsmdef("Assets/Scripts/HotfixView/Client/GameLogic/Ignore.asmdef");
            
            // Model-Client-GameLogic
            EnableAsmdef("Assets/Scripts/Model/Client/GameLogic/Ignore.asmdef");
            // Model-Server-GameLogic
            EnableAsmdef("Assets/Scripts/Model/Server/GameLogic/Ignore.asmdef");
            // Model-Share-GameLogic
            EnableAsmdef("Assets/Scripts/Model/Share/GameLogic/Ignore.asmdef");
            
            // ModelView-Client-GameLogic
            EnableAsmdef("Assets/Scripts/ModelView/Client/GameLogic/Ignore.asmdef");
            
            // FairyGUI
            EnableAsmdef("Assets/Scripts/HotfixView/Client/Plugins/FairyGUI/Ignore.asmdef");
            EnableAsmdef("Assets/Scripts/ModelView/Client/Plugins/FairyGUI/Ignore.asmdef");
            
            
            // 《Disable》
            // Hotfix-Client-Demo&LockStep
            DisableAsmdef("Assets/Scripts/Hotfix/Client/Demo/Ignore.asmdef");
            DisableAsmdef("Assets/Scripts/Hotfix/Client/LockStep/Ignore.asmdef");
            // Hotfix-Server-Demo&LockStep
            DisableAsmdef("Assets/Scripts/Hotfix/Server/Demo/Ignore.asmdef");
            DisableAsmdef("Assets/Scripts/Hotfix/Server/LockStep/Ignore.asmdef");
            // Hotfix-Share-Demo&LockStep
            DisableAsmdef("Assets/Scripts/Hotfix/Share/Demo/Ignore.asmdef");
            DisableAsmdef("Assets/Scripts/Hotfix/Share/LockStep/Ignore.asmdef");
            
            // HotfixView-Client-Demo&LockStep
            DisableAsmdef("Assets/Scripts/HotfixView/Client/Demo/Ignore.asmdef");
            DisableAsmdef("Assets/Scripts/HotfixView/Client/LockStep/Ignore.asmdef");

            
            // Model-Client-Demo&LockStep
            DisableAsmdef("Assets/Scripts/Model/Client/Demo/Ignore.asmdef");
            DisableAsmdef("Assets/Scripts/Model/Client/LockStep/Ignore.asmdef");
            // Model-Server-Demo&LockStep
            DisableAsmdef("Assets/Scripts/Model/Server/Demo/Ignore.asmdef");
            DisableAsmdef("Assets/Scripts/Model/Server/LockStep/Ignore.asmdef");
            // Model-Share-Demo&LockStep
            DisableAsmdef("Assets/Scripts/Model/Share/Demo/Ignore.asmdef");
            DisableAsmdef("Assets/Scripts/Model/Share/LockStep/Ignore.asmdef");
            
            // ModelView-Client-Demo&LockStep
            DisableAsmdef("Assets/Scripts/ModelView/Client/Demo/Ignore.asmdef");
            DisableAsmdef("Assets/Scripts/ModelView/Client/LockStep/Ignore.asmdef");
        }

        static void EnableAppTypeLockStep()
        {
            // 《Enable》
            // Hotfix-Client-GameLogic
            EnableAsmdef("Assets/Scripts/Hotfix/Client/GameLogic/Ignore.asmdef");
            // Hotfix-Server-GameLogic
            EnableAsmdef("Assets/Scripts/Hotfix/Server/GameLogic/Ignore.asmdef");
            // Hotfix-Share-GameLogic
            EnableAsmdef("Assets/Scripts/Hotfix/Share/GameLogic/Ignore.asmdef");
            
            // HotfixView-Client-GameLogic
            EnableAsmdef("Assets/Scripts/HotfixView/Client/GameLogic/Ignore.asmdef");
            
            // Model-Client-GameLogic
            EnableAsmdef("Assets/Scripts/Model/Client/GameLogic/Ignore.asmdef");
            // Model-Server-GameLogic
            EnableAsmdef("Assets/Scripts/Model/Server/GameLogic/Ignore.asmdef");
            // Model-Share-GameLogic
            EnableAsmdef("Assets/Scripts/Model/Share/GameLogic/Ignore.asmdef");
            
            // ModelView-Client-GameLogic
            EnableAsmdef("Assets/Scripts/ModelView/Client/GameLogic/Ignore.asmdef");
            
            // FairyGUI
            EnableAsmdef("Assets/Scripts/HotfixView/Client/Plugins/FairyGUI/Ignore.asmdef");
            EnableAsmdef("Assets/Scripts/ModelView/Client/Plugins/FairyGUI/Ignore.asmdef");
            
            
            // 《Disable》
            // Hotfix-Client-Demo&LockStep
            DisableAsmdef("Assets/Scripts/Hotfix/Client/Demo/Ignore.asmdef");
            DisableAsmdef("Assets/Scripts/Hotfix/Client/LockStep/Ignore.asmdef");
            // Hotfix-Server-Demo&LockStep
            DisableAsmdef("Assets/Scripts/Hotfix/Server/Demo/Ignore.asmdef");
            DisableAsmdef("Assets/Scripts/Hotfix/Server/LockStep/Ignore.asmdef");
            // Hotfix-Share-Demo&LockStep
            DisableAsmdef("Assets/Scripts/Hotfix/Share/Demo/Ignore.asmdef");
            DisableAsmdef("Assets/Scripts/Hotfix/Share/LockStep/Ignore.asmdef");
            
            // HotfixView-Client-Demo&LockStep
            DisableAsmdef("Assets/Scripts/HotfixView/Client/Demo/Ignore.asmdef");
            DisableAsmdef("Assets/Scripts/HotfixView/Client/LockStep/Ignore.asmdef");

            
            // Model-Client-Demo&LockStep
            DisableAsmdef("Assets/Scripts/Model/Client/Demo/Ignore.asmdef");
            DisableAsmdef("Assets/Scripts/Model/Client/LockStep/Ignore.asmdef");
            // Model-Server-Demo&LockStep
            DisableAsmdef("Assets/Scripts/Model/Server/Demo/Ignore.asmdef");
            DisableAsmdef("Assets/Scripts/Model/Server/LockStep/Ignore.asmdef");
            // Model-Share-Demo&LockStep
            DisableAsmdef("Assets/Scripts/Model/Share/Demo/Ignore.asmdef");
            DisableAsmdef("Assets/Scripts/Model/Share/LockStep/Ignore.asmdef");
            
            // ModelView-Client-Demo&LockStep
            DisableAsmdef("Assets/Scripts/ModelView/Client/Demo/Ignore.asmdef");
            DisableAsmdef("Assets/Scripts/ModelView/Client/LockStep/Ignore.asmdef");
        }

        static void EnableAppTypeGameLogic()
        {
            // 《Enable》
            // Hotfix-Client-Demo&LockStep
            EnableAsmdef("Assets/Scripts/Hotfix/Client/Demo/Ignore.asmdef");
            EnableAsmdef("Assets/Scripts/Hotfix/Client/LockStep/Ignore.asmdef");
            // Hotfix-Server-Demo&LockStep
            EnableAsmdef("Assets/Scripts/Hotfix/Server/Demo/Ignore.asmdef");
            EnableAsmdef("Assets/Scripts/Hotfix/Server/LockStep/Ignore.asmdef");
            // Hotfix-Share-Demo&LockStep
            EnableAsmdef("Assets/Scripts/Hotfix/Share/Demo/Ignore.asmdef");
            EnableAsmdef("Assets/Scripts/Hotfix/Share/LockStep/Ignore.asmdef");
            
            // HotfixView-Client-Demo&LockStep
            EnableAsmdef("Assets/Scripts/HotfixView/Client/Demo/Ignore.asmdef");
            EnableAsmdef("Assets/Scripts/HotfixView/Client/LockStep/Ignore.asmdef");

            
            // Model-Client-Demo&LockStep
            EnableAsmdef("Assets/Scripts/Model/Client/Demo/Ignore.asmdef");
            EnableAsmdef("Assets/Scripts/Model/Client/LockStep/Ignore.asmdef");
            // Model-Server-Demo&LockStep
            EnableAsmdef("Assets/Scripts/Model/Server/Demo/Ignore.asmdef");
            EnableAsmdef("Assets/Scripts/Model/Server/LockStep/Ignore.asmdef");
            // Model-Share-Demo&LockStep
            EnableAsmdef("Assets/Scripts/Model/Share/Demo/Ignore.asmdef");
            EnableAsmdef("Assets/Scripts/Model/Share/LockStep/Ignore.asmdef");
            
            // ModelView-Client-Demo&LockStep
            EnableAsmdef("Assets/Scripts/ModelView/Client/Demo/Ignore.asmdef");
            EnableAsmdef("Assets/Scripts/ModelView/Client/LockStep/Ignore.asmdef");
            
            // 《Disable》
            // Hotfix-Client-GameLogic
            DisableAsmdef("Assets/Scripts/Hotfix/Client/GameLogic/Ignore.asmdef");
            // Hotfix-Server-GameLogic
            DisableAsmdef("Assets/Scripts/Hotfix/Server/GameLogic/Ignore.asmdef");
            // Hotfix-Share-GameLogic
            DisableAsmdef("Assets/Scripts/Hotfix/Share/GameLogic/Ignore.asmdef");
            
            // HotfixView-Client-GameLogic
            DisableAsmdef("Assets/Scripts/HotfixView/Client/GameLogic/Ignore.asmdef");
            
            // Model-Client-GameLogic
            DisableAsmdef("Assets/Scripts/Model/Client/GameLogic/Ignore.asmdef");
            // Model-Server-GameLogic
            DisableAsmdef("Assets/Scripts/Model/Server/GameLogic/Ignore.asmdef");
            // Model-Share-GameLogic
            DisableAsmdef("Assets/Scripts/Model/Share/GameLogic/Ignore.asmdef");
            
            // ModelView-Client-GameLogic
            DisableAsmdef("Assets/Scripts/ModelView/Client/GameLogic/Ignore.asmdef");
            
            // FairyGUI
            DisableAsmdef("Assets/Scripts/HotfixView/Client/Plugins/FairyGUI/Ignore.asmdef");
            DisableAsmdef("Assets/Scripts/ModelView/Client/Plugins/FairyGUI/Ignore.asmdef");
        }
        
        #endregion

        /// <summary>
        /// 启用指定的程序集定义文件
        /// </summary>
        static void EnableAsmdef(string asmdefFile)
        {
            string asmdefDisableFile = $"{asmdefFile}.DISABLED";
            string srcFilePath = asmdefDisableFile.Replace("Assets/Scripts/", "Assets/Settings/IgnoreAsmdef/");

            if (!File.Exists(srcFilePath))
            {
                Debug.LogError($"忽略编译配置的原文件不存在, 请检查项目文件完整性:{srcFilePath}");
                return;
            }

            if (File.Exists(asmdefFile) && new FileInfo(srcFilePath).LastWriteTime == new FileInfo(asmdefFile).LastWriteTime)
            {
                return;
            }

            File.Copy(srcFilePath, asmdefFile, true);
        }

        /// <summary>
        /// 删除指定的程序集定义文件
        /// </summary>
        static void DisableAsmdef(string asmdefFile)
        {
            File.Delete(asmdefFile);
            File.Delete($"{asmdefFile}.meta");
        }
    }
}