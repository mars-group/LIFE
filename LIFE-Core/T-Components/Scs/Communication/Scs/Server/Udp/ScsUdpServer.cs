//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

using Hik.Communication.Scs.Communication.Channels;
using Hik.Communication.Scs.Communication.Channels.Udp;
using Hik.Communication.Scs.Communication.EndPoints.Udp;

namespace Hik.Communication.Scs.Server.Udp
{
    class ScsUdpServer : ScsServerBase
    {
        private ScsUdpEndPoint _endpoint;

        public ScsUdpServer(ScsUdpEndPoint scsUdpEndPoint)
        {
            _endpoint = scsUdpEndPoint;
        }

        protected override IConnectionListener CreateConnectionListener()
        {
            return new UdpConnectionListener(_endpoint);
        }
    }
}