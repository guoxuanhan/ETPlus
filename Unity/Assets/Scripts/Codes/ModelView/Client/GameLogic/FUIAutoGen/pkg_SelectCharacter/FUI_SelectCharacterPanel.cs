/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ET.Client.pkg_SelectCharacter
{
	public partial class FUI_SelectCharacterPanel: GComponent
	{
		public GList list;
		public ET.Client.pkg_Common.FUI_Button4 btn_return;
		public ET.Client.pkg_Common.FUI_Button5 btn_login;
		public const string URL = "ui://rwk21bq1lmtf0";

		public static FUI_SelectCharacterPanel CreateInstance()
		{
			return (FUI_SelectCharacterPanel)UIPackage.CreateObject("pkg_SelectCharacter", "SelectCharacterPanel");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			list = (GList)GetChildAt(1);
			btn_return = (ET.Client.pkg_Common.FUI_Button4)GetChildAt(2);
			btn_login = (ET.Client.pkg_Common.FUI_Button5)GetChildAt(3);
		}
	}
}
