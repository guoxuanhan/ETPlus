namespace ET
{
    namespace EventType
    {
        public struct SceneChangeStart
        {
        }
        
        public struct SceneChangeFinish
        {
        }
        
        public struct AfterCreateClientScene
        {
        }
        
        public struct AfterCreateCurrentScene
        {
        }

        public struct AppStartInitFinish
        {
        }

        public struct LoginFinish
        {
        }

        public struct EnterMapFinish
        {
        }

        public struct AfterUnitCreate
        {
            public Unit Unit;
        }

        /// <summary>
        /// 登录排队事件
        /// </summary>
        public struct UpdateLoginQueueInfo
        {
            public int QueueIndex { get; set; }
            
            public int QueueCount { get; set; }
        }
    }
}