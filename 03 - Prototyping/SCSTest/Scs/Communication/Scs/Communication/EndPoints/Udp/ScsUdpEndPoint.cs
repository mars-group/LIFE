using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hik.Communication.Scs.Client;
using Hik.Communication.Scs.Server;
using Hik.Communication.Scs.Server.Udp;

namespace Hik.Communication.Scs.Communication.EndPoints.Udp
{
    public class ScsUdpEndPoint : ScsEndPoint
    {
        public string IpAddress { get; private set; }

        public int UdpPort { get; set; }

        public ScsUdpEndPoint(string address) {
            var splittedAddress = address.Trim().Split(':');
            IpAddress = splittedAddress[0].Trim();
            UdpPort = Convert.ToInt32(splittedAddress[1].Trim());
        }




        internal override IScsServer CreateServer() {
            return new ScsUdpServer(this);
        }

        internal override IScsClient CreateClient() {
            throw new NotImplementedException();
        }
    }
}
