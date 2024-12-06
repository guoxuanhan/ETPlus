namespace ET.Client
{
    [EntitySystemOf(typeof(HotUpdatePanel))]
    [FriendOf(typeof(HotUpdatePanel))]
    public static partial class HotUpdatePanelSystem
    {
        [EntitySystem]
        private static void Awake(this HotUpdatePanel self)
        {
            Log.Info("<color=#FFFFFF>热更新 Awake</color>");
        }

        [EntitySystem]
        private static void Show(this HotUpdatePanel self)
        {
            Log.Info("<color=#FFFFFF>热更新 Show</color>");
        }

        [EntitySystem]
        private static void Hide(this HotUpdatePanel self)
        {
            Log.Info("<color=#FFFFFF>热更新 Hide</color>");
        }

        [EntitySystem]
        private static void BeforeUnload(this HotUpdatePanel self)
        {
            Log.Info("<color=#FFFFFF>热更新 BeforeUnload</color>");
        }
    }
}