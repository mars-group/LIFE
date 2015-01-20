using ASC.Communication.Scs.Communication.Channels;
using ASC.Communication.Scs.Communication.Channels.Udp;

namespace ASC.Communication.Scs.Server.Udp
{
    class AscUdpServer : AscServerBase
    {
        private readonly ICommunicationChannel _udpChannel;
        private IConnectionListener _udpConnectionListener;

        public AscUdpServer(ICommunicationChannel udpChannel)
        {
            _udpChannel = udpChannel;
        }

        protected override IConnectionListener CreateConnectionListener() {
            return _udpConnectionListener ?? (_udpConnectionListener = new UdpConnectionListener(_udpChannel));
        }
    }
}
