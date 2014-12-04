using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hik.Communication.Scs.Communication.EndPoints.Udp;

namespace Hik.Communication.Scs.Communication.Channels.Udp
{
    class UdpConnectionListener : ConnectionListenerBase
    {
        private ScsUdpEndPoint _endPoint;

        public UdpConnectionListener(ScsUdpEndPoint endpoint) {
            _endPoint = endpoint;
        }

        public override void Start() {
            // TODO Implement!
            throw new NotImplementedException();
        }

        public override void Stop() {
            throw new NotImplementedException();
        }
    }
}
