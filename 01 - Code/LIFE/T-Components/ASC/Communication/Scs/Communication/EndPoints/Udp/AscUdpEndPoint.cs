using ASC.Communication.Scs.Client;
using ASC.Communication.Scs.Client.Udp;
using ASC.Communication.Scs.Server;
using ASC.Communication.Scs.Server.Udp;

namespace ASC.Communication.Scs.Communication.EndPoints.Udp {
    public class AscUdpEndPoint : AscEndPoint {
        private IScsServer _ascUdpServer;
        private IScsClient _ascUdpClient;
        public string McastGroup { get; private set; }

        public int UdpListenPort { get; set; }

        public AscUdpEndPoint(int listenPort, string mcastGroup) {
            McastGroup = mcastGroup;
            UdpListenPort = listenPort;
        }


        internal override IScsServer CreateServer() {
            return _ascUdpServer ?? (_ascUdpServer = new AscUdpServer(this));
        }

        internal override IScsClient CreateClient()
        {
            return _ascUdpClient ?? (_ascUdpClient = new AscUdpClient(this));
        }
    }
}