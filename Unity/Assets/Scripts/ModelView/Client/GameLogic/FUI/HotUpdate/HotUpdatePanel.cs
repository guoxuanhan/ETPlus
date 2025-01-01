using ET.Client.HotUpdate;

namespace ET.Client
{
    [ComponentOf(typeof(FUIEntity))]
    [FUIPanel(PanelId.HotUpdatePanel, "HotUpdate", "HotUpdatePanel")]
    public class HotUpdatePanel: Entity, IAwake, IShow, IHide, IBeforeUnload
    {
        private FUI_HotUpdatePanel _fuiHotUpdatePanel;

        public FUI_HotUpdatePanel FUIHotUpdatePanel
        {
            get => _fuiHotUpdatePanel ??= (FUI_HotUpdatePanel)this.GetParent<FUIEntity>().GComponent;
        }
    }
}
