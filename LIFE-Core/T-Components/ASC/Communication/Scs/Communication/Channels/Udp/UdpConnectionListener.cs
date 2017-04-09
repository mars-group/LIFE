//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

namespace ASC.Communication.Scs.Communication.Channels.Udp
{
    /// <summary>
    /// Will just raise the OnCommunicationChannelconnected Event to fullful the 
    /// SCS framework's requirements.
    /// </summary>
    class UdpConnectionListener : ConnectionListenerBase
    {
        private readonly ICommunicationChannel _udpchannel;

        public UdpConnectionListener(ICommunicationChannel udpchannel)
        {
            _udpchannel = udpchannel;
        }

        public override void Start()
        {
            OnCommunicationChannelConnected(_udpchannel);
        }

        public override void Stop()
        {
            // nothing to be done here
        }
    }
}