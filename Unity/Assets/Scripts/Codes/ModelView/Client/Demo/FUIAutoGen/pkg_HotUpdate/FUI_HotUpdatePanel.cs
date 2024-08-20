/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ET.Client.pkg_HotUpdate
{
	public partial class FUI_HotUpdatePanel: GComponent
	{
		public GTextField n0;
		public GProgressBar ProgressBar;
		public const string URL = "ui://csbreabtr1cc1";

		public static FUI_HotUpdatePanel CreateInstance()
		{
			return (FUI_HotUpdatePanel)UIPackage.CreateObject("pkg_HotUpdate", "HotUpdatePanel");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			n0 = (GTextField)GetChildAt(0);
			ProgressBar = (GProgressBar)GetChildAt(1);
		}
	}
}
