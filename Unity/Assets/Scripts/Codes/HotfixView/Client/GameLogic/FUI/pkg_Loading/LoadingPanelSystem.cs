namespace ET.Client
{
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