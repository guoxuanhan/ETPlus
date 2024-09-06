using ET.Client.pkg_SelectCharacter;

namespace ET.Client
{
	[ComponentOf(typeof(FUIEntity))]
	public class SelectCharacterPanel: Entity, IAwake
	{
		private FUI_SelectCharacterPanel _fuiSelectCharacterPanel;

		public FUI_SelectCharacterPanel FUISelectCharacterPanel
		{
			get => _fuiSelectCharacterPanel ??= (FUI_SelectCharacterPanel)this.GetParent<FUIEntity>().GComponent;
		}
	}
}
