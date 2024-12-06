using ET.Client.TestA;

namespace ET.Client
{
    [ComponentOf(typeof(FUIEntity))]
    [FUIPanel(PanelId.TestAPanel, "TestA", "TestAPanel")]
    public class TestAPanel: Entity, IAwake, IShow, IHide, IBeforeUnload
    {
        private FUI_TestAPanel _fuiTestAPanel;

        public FUI_TestAPanel FUITestAPanel
        {
            get => _fuiTestAPanel ??= (FUI_TestAPanel)this.GetParent<FUIEntity>().GComponent;
        }
    }
}
