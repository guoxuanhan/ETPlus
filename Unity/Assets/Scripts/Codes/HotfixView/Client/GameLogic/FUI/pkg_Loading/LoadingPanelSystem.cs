namespace ET.Client
{
    [Event(SceneType.Client)]
    public class LoadingPanel_SceneChangeStart: AEvent<EventType.SceneChangeStart>
    {
        protected override async ETTask Run(Scene scene, EventType.SceneChangeStart args)
        {
            await FUIComponent.Instance.ShowPanelAsync(PanelId.LoadingPanel);
        }
    }

    [Event(SceneType.Client)]
    public class LoadingPanel_SceneChangeFinish: AEvent<EventType.SceneChangeFinish>
    {
        protected override async ETTask Run(Scene scene, EventType.SceneChangeFinish args)
        {
            FUIComponent.Instance.HidePanel(PanelId.LoadingPanel);
            await ETTask.CompletedTask;
        }
    }

    [FriendOf(typeof (LoadingPanel))]
    public static class LoadingPanelSystem
    {
        public static void Awake(this LoadingPanel self)
        {
        }

        public static void RegisterUIEvent(this LoadingPanel self)
        {
        }

        public static void OnShow(this LoadingPanel self, Entity contextData = null)
        {
            self.FUILoadingPanel.sdr_progressbar.TweenValue(100, RandomGenerator.RandFloat01()).OnComplete(d =>
            {
                FUIComponent.Instance.HidePanel(PanelId.LoadingPanel);
            });
        }

        public static void OnHide(this LoadingPanel self)
        {
        }

        public static void BeforeUnload(this LoadingPanel self)
        {
        }
    }
}