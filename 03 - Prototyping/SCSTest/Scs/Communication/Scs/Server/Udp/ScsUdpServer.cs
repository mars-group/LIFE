using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hik.Communication.Scs.Communication.Channels;
using Hik.Communication.Scs.Communication.Channels.Udp;
using Hik.Communication.Scs.Communication.EndPoints.Udp;

namespace Hik.Communication.Scs.Server.Udp
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
