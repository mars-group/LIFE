using ASC.Communication.Scs.Client;
using ASC.Communication.Scs.Client.Udp;
using ASC.Communication.Scs.Communication.Channels.Udp;
using ASC.Communication.Scs.Server;
using ASC.Communication.Scs.Server.Udp;

namespace ASC.Communication.Scs.Communication.EndPoints.Udp {
    public class AscUdpEndPoint : AscEndPoint {
        private readonly IScsServer _ascUdpServer;
        private readonly IScsClient _ascUdpClient;
        private readonly UdpCommunicationChannel _udpChannel;
        public string McastGroup { get; private set; }

        public int UdpClientListenPort { get; set; }

        public int UdpServerListenPort { get; set; }

        public AscUdpEndPoint(int clientListenPort, int serverListenPort, string mcastGroup) {
            McastGroup = mcastGroup;
            UdpClientListenPort = clientListenPort;
            UdpServerListenPort = serverListenPort;
            _udpChannel = new UdpCommunicationChannel(this);
            _ascUdpClient = new AscUdpClient(_udpChannel);
            _ascUdpServer = new AscUdpServer(_udpChannel);
        }




        internal override IScsServer CreateServer() {
            return _ascUdpServer;
        }

        internal override IScsClient CreateClient() {
            return _ascUdpClient;
        }
    }
}