using ET.Client.TestB;

namespace ET.Client
{
    [ComponentOf(typeof(FUIEntity))]
    [FUIPanel(PanelId.TestBPanel, "TestB", "TestBPanel")]
    public class TestBPanel: Entity, IAwake, IShow, IHide, IBeforeUnload
    {
        public OneComponent Com1 {get; set;}

        public TwoComponent Com2 {get; set;}

        private FUI_TestBPanel _fuiTestBPanel;

        public FUI_TestBPanel FUITestBPanel
        {
            get => _fuiTestBPanel ??= (FUI_TestBPanel)this.GetParent<FUIEntity>().GComponent;
        }
    }
}
