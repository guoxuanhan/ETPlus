using System.Net;

namespace ET.Server
{
    public struct NetServerComponentOnRead
    {
        public Session Session;
        public object Message;
    }

    [ComponentOf(typeof (Scene))]
    public class NetServerComponent: Entity, IAwake<IPEndPoint>, IAwake<IPEndPoint, NetworkProtocol>, IDestroy
    {
        public int ServiceId;

        public NetworkProtocol NetworkProtocol { get; set; } = NetworkProtocol.KCP;
    }
}