/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ET.Client.pkg_ServerInfo
{
	public partial class FUI_ServerInfoPanel: GComponent
	{
		public GList list;
		public const string URL = "ui://ytvbldaxafn40";

		public static FUI_ServerInfoPanel CreateInstance()
		{
			return (FUI_ServerInfoPanel)UIPackage.CreateObject("pkg_ServerInfo", "ServerInfoPanel");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			list = (GList)GetChildAt(3);
		}
	}
}
