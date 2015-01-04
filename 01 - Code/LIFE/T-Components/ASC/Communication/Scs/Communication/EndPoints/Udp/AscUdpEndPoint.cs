using System;
using ASC.Communication.Scs.Client;
using ASC.Communication.Scs.Client.Udp;
using ASC.Communication.Scs.Server;
using ASC.Communication.Scs.Server.Udp;

namespace ASC.Communication.Scs.Communication.EndPoints.Udp {
    public class AscUdpEndPoint : AscEndPoint {
        public string McastGroup { get; private set; }
        public string IpAddress { get; private set; }

        public int UdpPort { get; set; }

        public AscUdpEndPoint(string address, string mcastGroup) {
            McastGroup = mcastGroup;
            var splittedAddress = address.Trim().Split(':');
            IpAddress = splittedAddress[0].Trim();
            UdpPort = Convert.ToInt32(splittedAddress[1].Trim());
        }


        internal override IScsServer CreateServer() {
            return new AscUdpServer(this);
        }

        internal override IScsClient CreateClient()
        {
            // make sure only one AscUdpClient ist present
            return new AscUdpClient(this);
        }
    }
}