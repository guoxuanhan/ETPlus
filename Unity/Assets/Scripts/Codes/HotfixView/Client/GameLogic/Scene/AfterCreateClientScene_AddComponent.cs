namespace ET.Client
{
    [Event(SceneType.Client)]
    public class AfterCreateClientScene_AddComponent: AEvent<EventType.AfterCreateClientScene>
    {
        protected override async ETTask Run(Scene scene, EventType.AfterCreateClientScene args)
        {
            scene.AddComponent<FUIEventComponent>();
            scene.AddComponent<FUIAssetComponent, bool>(false);
            scene.AddComponent<FUIComponent, int, int>(1920, 1080);
            await ETTask.CompletedTask;
        }
    }
}