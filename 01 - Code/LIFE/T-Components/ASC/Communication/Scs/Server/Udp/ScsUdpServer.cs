using ASC.Communication.Scs.Communication.Channels;
using ASC.Communication.Scs.Communication.Channels.Udp;
using ASC.Communication.Scs.Communication.EndPoints.Udp;

namespace ASC.Communication.Scs.Server.Udp
{
    class ScsUdpServer : ScsServerBase
    {
        private AscUdpEndPoint _endpoint;

        public ScsUdpServer(AscUdpEndPoint ascUdpEndPoint) {
            _endpoint = ascUdpEndPoint;
        }

        protected override IConnectionListener CreateConnectionListener() {
            return new UdpConnectionListener(_endpoint);
        }
    }
}
