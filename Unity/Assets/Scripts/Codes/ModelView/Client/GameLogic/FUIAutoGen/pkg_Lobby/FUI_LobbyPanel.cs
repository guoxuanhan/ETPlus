/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ET.Client.pkg_Lobby
{
	public partial class FUI_LobbyPanel: GComponent
	{
		public GImage joystick_center;
		public GButton joystick;
		public GGraph joystick_touch;
		public GTextField n8;
		public GTextField n9;
		public const string URL = "ui://cv6e72zir1cc0";

		public static FUI_LobbyPanel CreateInstance()
		{
			return (FUI_LobbyPanel)UIPackage.CreateObject("pkg_Lobby", "LobbyPanel");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			joystick_center = (GImage)GetChildAt(0);
			joystick = (GButton)GetChildAt(1);
			joystick_touch = (GGraph)GetChildAt(2);
			n8 = (GTextField)GetChildAt(3);
			n9 = (GTextField)GetChildAt(4);
		}
	}
}
