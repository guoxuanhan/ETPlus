namespace ET.Client
{
    [Event(SceneType.Client)]
    public class AfterCreateClientScene_AddComponent: AEvent<EventType.AfterCreateClientScene>
    {
        protected override async ETTask Run(Scene scene, EventType.AfterCreateClientScene args)
        {
            scene.AddComponent<UIEventComponent>();
            scene.AddComponent<UIComponent>();
            scene.AddComponent<FUIEventComponent>();
            scene.AddComponent<FUIAssetComponent, bool>(false);
            scene.AddComponent<FUIComponent>();
            await ETTask.CompletedTask;
        }
    }
}