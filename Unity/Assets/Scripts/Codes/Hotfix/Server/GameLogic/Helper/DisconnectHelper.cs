namespace ET.Server
{
    public static class DisconnectHelper
    {
        public static async ETTask Disconnect(this Session self)
        {
            if (self == null)
            {
                return;
            }

            long instanceId = self.InstanceId;

            await TimerComponent.Instance.WaitAsync(1000);

            if (self.InstanceId != instanceId)
            {
                return;
            }

            self.Dispose();
        }
    }
}