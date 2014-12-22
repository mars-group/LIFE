using ASC.Communication.Scs.Communication.Channels;
using ASC.Communication.Scs.Communication.Channels.Udp;
using ASC.Communication.Scs.Communication.EndPoints.Udp;

namespace ASC.Communication.Scs.Server.Udp
{
    class ScsUdpServer : ScsServerBase
    {
        private ScsUdpEndPoint _endpoint;

        public ScsUdpServer(ScsUdpEndPoint scsUdpEndPoint) {
            _endpoint = scsUdpEndPoint;
        }

        protected override IConnectionListener CreateConnectionListener() {
            return new UdpConnectionListener(_endpoint);
        }
    }
}
