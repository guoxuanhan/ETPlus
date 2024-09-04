/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ET.Client.pkg_Login
{
	public partial class FUI_LoginPanel: GComponent
	{
		public ET.Client.pkg_Common.FUI_Button2 btn_close;
		public GTextField txt_title;
		public GComponent ipt_account;
		public GComponent ipt_password;
		public ET.Client.pkg_Common.FUI_Button3 btn_login;
		public const string URL = "ui://cnapbtb2r1cc0";

		public static FUI_LoginPanel CreateInstance()
		{
			return (FUI_LoginPanel)UIPackage.CreateObject("pkg_Login", "LoginPanel");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			btn_close = (ET.Client.pkg_Common.FUI_Button2)GetChildAt(3);
			txt_title = (GTextField)GetChildAt(4);
			ipt_account = (GComponent)GetChildAt(5);
			ipt_password = (GComponent)GetChildAt(6);
			btn_login = (ET.Client.pkg_Common.FUI_Button3)GetChildAt(7);
		}
	}
}
