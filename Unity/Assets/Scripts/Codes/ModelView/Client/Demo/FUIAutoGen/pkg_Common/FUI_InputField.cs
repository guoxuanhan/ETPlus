/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ET.Client.pkg_Common
{
	public partial class FUI_InputField: GComponent
	{
		public GTextInput input;
		public const string URL = "ui://c3ni7iglr1cc0";

		public static FUI_InputField CreateInstance()
		{
			return (FUI_InputField)UIPackage.CreateObject("pkg_Common", "InputField");
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			input = (GTextInput)GetChildAt(1);
		}
	}
}
