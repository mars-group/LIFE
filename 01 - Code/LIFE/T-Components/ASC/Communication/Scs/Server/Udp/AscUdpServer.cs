using ASC.Communication.Scs.Communication.Channels;
using ASC.Communication.Scs.Communication.Channels.Udp;
using ASC.Communication.Scs.Communication.Messengers;

namespace ASC.Communication.Scs.Server.Udp
{
    internal class AscUdpServer : AscServerBase {
        private readonly ICommunicationChannel _udpChannel;
        private IConnectionListener _udpConnectionListener;

        public AscUdpServer(ICommunicationChannel udpChannel) {
            _udpChannel = udpChannel;
            _udpChannel.WireProtocol = WireProtocolFactory.CreateWireProtocol();
        }

        public override IMessenger GetMessenger() {
            return _udpChannel ;
        }


        protected override IConnectionListener CreateConnectionListener() {
            return _udpConnectionListener ?? (_udpConnectionListener = new UdpConnectionListener(_udpChannel));
        }
    }
}
