//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using ASC.Communication.Scs.Communication.Channels;
using ASC.Communication.Scs.Communication.Channels.Udp;
using ASC.Communication.Scs.Communication.Messengers;

namespace ASC.Communication.Scs.Server.Udp
{
    internal class AscUdpServer : AscServerBase {
        private readonly ICommunicationChannel _udpChannel;
        private IConnectionListener _udpConnectionListener;

        public AscUdpServer(ICommunicationChannel udpChannel) {
            _udpChannel = udpChannel;
            _udpChannel.WireProtocol = WireProtocolFactory.CreateWireProtocol();
        }

        public override IMessenger GetMessenger() {
            return _udpChannel ;
        }


        protected override IConnectionListener CreateConnectionListener() {
            return _udpConnectionListener ?? (_udpConnectionListener = new UdpConnectionListener(_udpChannel));
        }
    }
}
