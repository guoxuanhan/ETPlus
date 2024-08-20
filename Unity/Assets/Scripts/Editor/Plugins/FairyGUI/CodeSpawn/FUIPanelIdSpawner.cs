using System.IO;
using System.Text;
using ET;

namespace FUIEditor
{
    public static class FUIPanelIdSpawner
    {
        public static void SpawnPanelId()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/");
            sb.AppendLine();
            sb.AppendFormat("namespace {0}", FUICodeSpawner.NameSpace);
            sb.AppendLine();
            sb.AppendLine("{");
            sb.AppendLine("\tpublic enum PanelId");
            sb.AppendLine("\t{");

            sb.AppendLine("\t\tInvalid = 0,");

            foreach (PackageInfo packageInfo in FUICodeSpawner.PackageInfos.Values)
            {
                string pkgName = packageInfo.Name.Replace("pkg_", "");
                string panelName = "{0}Panel.xml".Fmt(pkgName);
                if (packageInfo.PackageComponentInfos.ContainsKey(panelName))
                {
                    sb.AppendLine("\t\t{0}Panel,".Fmt(pkgName));
                }
            }
            
            sb.AppendLine("\t}"); 
            sb.AppendLine("}");
            
            string filePath = "{0}/PanelId.cs".Fmt(FUICodeSpawner.FUIAutoGenDir);
            using FileStream fs = new FileStream(filePath, FileMode.Create);
            using StreamWriter sw = new StreamWriter(fs);
            sw.Write(sb.ToString());
        }
    }
}