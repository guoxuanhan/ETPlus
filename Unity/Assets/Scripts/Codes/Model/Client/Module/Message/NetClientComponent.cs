using System.Net.Sockets;

namespace ET.Client
{
    public struct NetClientComponentOnRead
    {
        public Session Session;
        public object Message;
    }

    [ComponentOf(typeof (Scene))]
    public class NetClientComponent: Entity, IAwake<AddressFamily>, IAwake<NetworkProtocol>, IDestroy
    {
        public int ServiceId;

        public NetworkProtocol NetworkProtocol { get; set; } = NetworkProtocol.KCP;
    }
}