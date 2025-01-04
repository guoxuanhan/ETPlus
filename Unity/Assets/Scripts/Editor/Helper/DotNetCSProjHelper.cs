using System;
using System.Linq;
using System.Xml.Linq;
using UnityEngine.Windows;

namespace ET
{
    public static class DotNetCSProjHelper
    {
        private const string ExcludeCsprojPath = "../DotNet/{0}/DotNet.{1}.csproj";
        private const string ExcludePatternServer = @"..\..\Unity\Assets\Scripts\{0}\Server\{1}\**";
        private const string ExcludePatternClient = @"..\..\Unity\Assets\Scripts\{0}\Client\{1}\**";
        private const string ExcludePatternShare = @"..\..\Unity\Assets\Scripts\{0}\Share\{1}\**";

        public static void ExcludeFolderRef(string[] appTypes)
        {
            foreach (var appType in appTypes)
            {
                if (!Enum.IsDefined(typeof(AppType), appType))
                {
                    Log.Error($"AppType未定义：{appType}!");
                    return;
                }
            }
            
            ModifyCsproj("Hotfix", appTypes);
            ModifyCsproj("Model", appTypes);
        }

        private static void ModifyCsproj(string type, string[] appTypes)
        {
            string path = string.Format(ExcludeCsprojPath, type, type);
            if (!File.Exists(path))
            {
                Log.Error($"DotNet工程{type}.csproj不存在，请检查项目性！");
                return;
            }
            
            XDocument document = XDocument.Load(path);
            
            // 删除旧的 ItemGroup 节点
            var existingNodes = document.Descendants("ItemGroup")
                    .Where(group => group.Elements("Compile").Any(e => e.Attribute("Remove") != null))
                    .ToList();

            foreach (var node in existingNodes)
            {
                node.Remove();
            }
            
            foreach (var appType in appTypes)
            {
                string patternServer = string.Format(ExcludePatternServer, type, appType);
                string patternClient = string.Format(ExcludePatternClient, type, appType);
                string patternShare = string.Format(ExcludePatternShare, type, appType);

                // 添加新的 ItemGroup 节点
                XElement newItem = new XElement("ItemGroup",
                    new XElement("Compile", new XAttribute("Remove", patternServer)),
                    new XElement("Compile", new XAttribute("Remove", patternClient)),
                    new XElement("Compile", new XAttribute("Remove", patternShare)));

                document.Root.Add(newItem);
                document.Save(path);
            }
        }
    }
}