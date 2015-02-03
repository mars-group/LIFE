using ASC.Communication.Scs.Client;
using ASC.Communication.Scs.Client.Udp;
using ASC.Communication.Scs.Communication.Channels.Udp;
using ASC.Communication.Scs.Server;
using ASC.Communication.Scs.Server.Udp;

namespace ASC.Communication.Scs.Communication.EndPoints.Udp {
    public class AscUdpEndPoint : AscEndPoint {
        private readonly IAscServer _ascUdpServer;
        private readonly IScsClient _ascUdpClient;
        private readonly UdpCommunicationChannel _udpChannel;
        public string McastGroup { get; private set; }

        public int UdpPort { get; set; }

        public AscUdpEndPoint(int port, string mcastGroup) {
            McastGroup = mcastGroup;
            UdpPort = port;
            _udpChannel = new UdpCommunicationChannel(this);
            _ascUdpClient = new AscUdpClient(_udpChannel);
            _ascUdpServer = new AscUdpServer(_udpChannel);
        }




        internal override IAscServer CreateServer() {
            return _ascUdpServer;
        }

        internal override IScsClient CreateClient() {
            return _ascUdpClient;
        }
    }
}