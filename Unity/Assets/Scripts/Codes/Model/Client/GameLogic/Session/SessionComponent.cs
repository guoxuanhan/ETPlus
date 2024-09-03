namespace ET.Client
{
    [ComponentOf(typeof (Scene))]
    public class SessionComponent: Entity, IAwake, IAwake<NetworkProtocol>, IDestroy
    {
        public NetworkProtocol NetworkProtocol;

        private EntityRef<Session> session;

        public Session Session
        {
            get
            {
                return this.session;
            }
            set
            {
                this.session = value;
            }
        }
    }
}