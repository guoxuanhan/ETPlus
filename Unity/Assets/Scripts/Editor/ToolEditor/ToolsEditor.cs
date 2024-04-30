using UnityEditor;

namespace ET
{
    public static class ToolsEditor
    {
        public static void ExcelChecker()
        {
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
            const string gen = "sh gen_check.sh";
#else
            const string gen = "gen_check.bat";
#endif
            ShellHelper.Run($"{gen}", "../Tools/Luban/");
        }
        
        public static void ExcelExporter()
        {
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
            const string tools = "./Tool";
#else
            const string tools = ".\\Tool.exe";
#endif
            ShellHelper.Run($"{tools} --AppType=ExcelExporter --Console=1", "../Bin/");
        }
        
        public static void Proto2CS()
        {
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
            const string tools = "./Tool";
#else
            const string tools = ".\\Tool.exe";
#endif
            ShellHelper.Run($"{tools} --AppType=Proto2CS --Console=1", "../Bin/");
        }
    }
}