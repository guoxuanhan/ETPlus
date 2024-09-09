/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ET.Client.pkg_QueueInfo
{
	public partial class FUI_QueueInfoPanel: GComponent
	{
		public GRichTextField txt_info;
		public ET.Client.pkg_Common.FUI_Button5 btn_close;
		public const string URL = "ui://pe184231jso30";

		public static FUI_QueueInfoPanel CreateInstance()
		{
			return (FUI_QueueInfoPanel)UIPackage.CreateObject("pkg_QueueInfo", "QueueInfoPanel");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			txt_info = (GRichTextField)GetChildAt(3);
			btn_close = (ET.Client.pkg_Common.FUI_Button5)GetChildAt(4);
		}
	}
}
