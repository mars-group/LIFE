//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using ASC.Communication.Scs.Client;
using ASC.Communication.Scs.Client.Udp;
using ASC.Communication.Scs.Communication.Channels.Udp;
using ASC.Communication.Scs.Server;
using ASC.Communication.Scs.Server.Udp;

namespace ASC.Communication.Scs.Communication.EndPoints.Udp {
    public class AscUdpEndPoint : AscEndPoint {
        private readonly IAscServer _ascUdpServer;
        private readonly IScsClient _ascUdpClient;
        public string McastGroup { get; private set; }

        public int UdpPort { get; set; }

        public AscUdpEndPoint(int port, string mcastGroup) {
            McastGroup = mcastGroup;
            UdpPort = port;
            var udpChannel = new UdpCommunicationChannel(this);
            udpChannel.Start();
            _ascUdpClient = new AscUdpClient(udpChannel);
            _ascUdpServer = new AscUdpServer(udpChannel);
        }




        internal override IAscServer CreateServer() {
            return _ascUdpServer;
        }

        internal override IScsClient CreateClient() {
            return _ascUdpClient;
        }
    }
}