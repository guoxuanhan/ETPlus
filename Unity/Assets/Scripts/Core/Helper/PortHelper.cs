using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace ET
{
    public static class PortHelper
    {
        /// <summary>
        /// 清理Unity的残留进程
        /// </summary>
        public static void ClearUnityProcess()
        {
#if UNITY_EDITOR
            Process[] allProcess = Process.GetProcesses();
            string fileName = Process.GetCurrentProcess().MainModule.FileName;

            List<Process> unityProcesses = new List<Process>();
            foreach (var process in allProcess)
            {
                if (process?.HasExited == false)
                {
                    if (process.ProcessName == "Unity" && process.MainModule.FileName == fileName)
                    {
                        unityProcesses.Add(process);
                    }
                    else if (process.ProcessName.Contains("UnityCrashHandler"))
                    {
                        process.Kill();
                        //UnityEngine.Debug.LogError("杀掉了多余的Unity奔溃进程");
                    }
                }
            }

            if (unityProcesses.Count > 1)
            {
                //kill 掉后启动的所有同Editor路径process
                unityProcesses.Sort((a, b) => a.StartTime < b.StartTime ? 0 : 1);

                for (int i = 1; i < unityProcesses.Count; i++)
                {
                    var process = unityProcesses[i];
                    if (process?.HasExited == false) process.Kill();
                    //Log.Info($"Kill Process {process.ProcessName}  PID:" + process.Id);
                }
            }
#endif
        }

#if UNITY_EDITOR
        /// <summary>
        /// 杀掉使用指定端口的进程
        /// </summary>
        /// <param name="port"></param>
        public static void KillByPort(int port)
        {
            Process pro = new Process();

            // 设置命令行、参数
            pro.StartInfo.FileName = "cmd.exe";
            pro.StartInfo.UseShellExecute = false;
            pro.StartInfo.RedirectStandardInput = true;
            pro.StartInfo.RedirectStandardOutput = true;
            pro.StartInfo.RedirectStandardError = true;
            pro.StartInfo.CreateNoWindow = true;
            // 启动CMD
            pro.Start();
            // 运行端口检查命令
            pro.StandardInput.WriteLine("netstat -ano");
            pro.StandardInput.WriteLine("exit");

            // 获取结果
            Regex reg = new Regex(@"\s ", RegexOptions.Compiled);
            string line = null;
            while ((line = pro.StandardOutput.ReadLine()) != null)
            {
                line = line.Trim();
                if (line.StartsWith("TCP", StringComparison.OrdinalIgnoreCase))
                {
                    line = reg.Replace(line, ",");

                    //Log.Error("测试kil process line:" + line);
                    string[] arr = line.Split(',');
                    if (arr[2].EndsWith(":" + port))
                    {
                        UnityEngine.Debug.Log($"占用端口的进程ID：{arr.Last()}");
                        KillProcess(int.Parse(arr.Last()));
                    }
                }
            }
        }

        public static void KillProcess(int processName) //调用方法，传参
        {
            if (processName == 0)
            {
                return;
            }

            try
            {
                // Process[] thisproc = Process.GetProcessesByName(processName);
                Process thisproc = Process.GetProcessById(processName);

                UnityEngine.Debug.Log($"进程名字为：" + thisproc.ProcessName);

                if (!thisproc.CloseMainWindow()) //尝试关闭进程 释放资源
                {
                    thisproc.Kill(); //强制关闭
                }

                UnityEngine.Debug.Log($"进程 {processName}关闭成功");
            }
            catch //出现异常，表明 kill 进程失败
            {
                UnityEngine.Debug.LogError($"结束进程{processName}出错！");
            }
            finally
            {
            }
        }
#endif
    }
}