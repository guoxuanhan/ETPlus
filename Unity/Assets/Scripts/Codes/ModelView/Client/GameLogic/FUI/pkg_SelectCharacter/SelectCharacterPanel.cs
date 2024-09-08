using ET.Client.pkg_SelectCharacter;

namespace ET.Client
{
	[ComponentOf(typeof(FUIEntity))]
	public class SelectCharacterPanel: Entity, IAwake
	{
		/// <summary>
		/// 上次选中的角色下标
		/// </summary>
		public int LastSelectRoleInfoIndex = -1;
		
		private FUI_SelectCharacterPanel _fuiSelectCharacterPanel;

		public FUI_SelectCharacterPanel FUISelectCharacterPanel
		{
			get => _fuiSelectCharacterPanel ??= (FUI_SelectCharacterPanel)this.GetParent<FUIEntity>().GComponent;
		}
	}
}
