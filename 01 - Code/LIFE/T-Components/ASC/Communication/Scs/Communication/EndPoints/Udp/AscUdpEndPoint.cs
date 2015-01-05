using ASC.Communication.Scs.Client;
using ASC.Communication.Scs.Client.Udp;
using ASC.Communication.Scs.Server;
using ASC.Communication.Scs.Server.Udp;

namespace ASC.Communication.Scs.Communication.EndPoints.Udp {
    public class AscUdpEndPoint : AscEndPoint {
        public string McastGroup { get; private set; }

        public int UdpListenPort { get; set; }

        public AscUdpEndPoint(int listenPort, string mcastGroup) {
            McastGroup = mcastGroup;
            UdpListenPort = listenPort;
        }


        internal override IScsServer CreateServer() {
            return new AscUdpServer(this);
        }

        internal override IScsClient CreateClient()
        {
            return new AscUdpClient(this);
        }
    }
}