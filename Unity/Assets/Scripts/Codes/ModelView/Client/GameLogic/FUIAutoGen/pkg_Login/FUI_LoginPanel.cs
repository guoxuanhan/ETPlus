/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ET.Client.pkg_Login
{
	public partial class FUI_LoginPanel: GComponent
	{
		public ET.Client.pkg_Common.FUI_InputField account;
		public ET.Client.pkg_Common.FUI_InputField password;
		public ET.Client.pkg_Common.FUI_Button1 login;
		public const string URL = "ui://cnapbtb2r1cc0";

		public static FUI_LoginPanel CreateInstance()
		{
			return (FUI_LoginPanel)UIPackage.CreateObject("pkg_Login", "LoginPanel");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			account = (ET.Client.pkg_Common.FUI_InputField)GetChildAt(0);
			password = (ET.Client.pkg_Common.FUI_InputField)GetChildAt(1);
			login = (ET.Client.pkg_Common.FUI_Button1)GetChildAt(2);
		}
	}
}
