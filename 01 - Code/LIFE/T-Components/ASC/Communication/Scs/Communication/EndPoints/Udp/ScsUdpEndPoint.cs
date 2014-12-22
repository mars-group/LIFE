﻿using System;
using ASC.Communication.Scs.Client;
using ASC.Communication.Scs.Client.Udp;
using ASC.Communication.Scs.Server;
using ASC.Communication.Scs.Server.Udp;

namespace ASC.Communication.Scs.Communication.EndPoints.Udp {
    public class ScsUdpEndPoint : ScsEndPoint {
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

        internal override IScsClient CreateClient()
        {
            return new ScsUdpClient(this);
        }
    }
}