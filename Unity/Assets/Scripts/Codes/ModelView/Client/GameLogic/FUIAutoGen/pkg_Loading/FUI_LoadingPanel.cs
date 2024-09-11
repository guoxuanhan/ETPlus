/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ET.Client.pkg_Loading
{
	public partial class FUI_LoadingPanel: GComponent
	{
		public GProgressBar sdr_progressbar;
		public GMovieClip anm_movieclip;
		public const string URL = "ui://tdkwgdekfn5k0";

		public static FUI_LoadingPanel CreateInstance()
		{
			return (FUI_LoadingPanel)UIPackage.CreateObject("pkg_Loading", "LoadingPanel");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			sdr_progressbar = (GProgressBar)GetChildAt(1);
			anm_movieclip = (GMovieClip)GetChildAt(2);
		}
	}
}
