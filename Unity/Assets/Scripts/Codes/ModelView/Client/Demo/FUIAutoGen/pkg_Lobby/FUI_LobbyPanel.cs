/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ET.Client.pkg_Lobby
{
	public partial class FUI_LobbyPanel: GComponent
	{
		public ET.Client.pkg_Common.FUI_Button1 enter;
		public const string URL = "ui://cv6e72zir1cc0";

		public static FUI_LobbyPanel CreateInstance()
		{
			return (FUI_LobbyPanel)UIPackage.CreateObject("pkg_Lobby", "LobbyPanel");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			enter = (ET.Client.pkg_Common.FUI_Button1)GetChildAt(0);
		}
	}
}
