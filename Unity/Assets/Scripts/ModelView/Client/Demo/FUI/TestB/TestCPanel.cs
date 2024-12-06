using ET.Client.TestB;

namespace ET.Client
{
    [ComponentOf(typeof(FUIEntity))]
    [FUIPanel(PanelId.TestCPanel, "TestB", "TestCPanel")]
    public class TestCPanel: Entity, IAwake, IShow, IHide, IBeforeUnload
    {
        private FUI_TestCPanel _fuiTestCPanel;

        public FUI_TestCPanel FUITestCPanel
        {
            get => _fuiTestCPanel ??= (FUI_TestCPanel)this.GetParent<FUIEntity>().GComponent;
        }
    }
}
