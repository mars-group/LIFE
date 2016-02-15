//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hik.Communication.Scs.Communication.EndPoints;
using Hik.Communication.Scs.Communication.EndPoints.Udp;

namespace Hik.Communication.Scs.Communication.Channels.Udp
{
    /// <summary>
    /// Will just raise the OnCommunicationChannelconnected Event to fullful the 
    /// SCS framework's requirements.
    /// </summary>
    class UdpConnectionListener : ConnectionListenerBase
    {
        private ScsUdpEndPoint _endPoint;

        public UdpConnectionListener(ScsUdpEndPoint endpoint) {
            _endPoint = endpoint;
        }

        public override void Start() {
            OnCommunicationChannelConnected(new UdpCommunicationChannel(_endPoint));
        }

        public override void Stop() {
            // nothing to be done here
        }
    }
}
