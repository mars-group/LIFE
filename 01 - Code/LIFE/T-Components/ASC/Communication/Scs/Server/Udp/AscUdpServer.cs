using ASC.Communication.Scs.Communication.Channels;
using ASC.Communication.Scs.Communication.Channels.Udp;
using ASC.Communication.Scs.Communication.EndPoints.Udp;

namespace ASC.Communication.Scs.Server.Udp
{
    class AscUdpServer : AscServerBase
    {
        private readonly AscUdpEndPoint _endpoint;
        private IConnectionListener _udpConnectionListener;

        public AscUdpServer(AscUdpEndPoint ascUdpEndPoint) {
            _endpoint = ascUdpEndPoint;
        }

        protected override IConnectionListener CreateConnectionListener() {
            return _udpConnectionListener ?? (_udpConnectionListener = new UdpConnectionListener(_endpoint));
        }
    }
}
