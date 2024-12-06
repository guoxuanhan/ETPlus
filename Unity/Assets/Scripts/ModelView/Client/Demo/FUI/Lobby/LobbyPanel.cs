using ET.Client.Lobby;

namespace ET.Client
{
    [ComponentOf(typeof(FUIEntity))]
    [FUIPanel(PanelId.LobbyPanel, "Lobby", "LobbyPanel")]
    public class LobbyPanel: Entity, IAwake, IShow, IHide, IBeforeUnload
    {
        private FUI_LobbyPanel _fuiLobbyPanel;

        public FUI_LobbyPanel FUILobbyPanel
        {
            get => _fuiLobbyPanel ??= (FUI_LobbyPanel)this.GetParent<FUIEntity>().GComponent;
        }
    }
}
