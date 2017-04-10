//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

using ASC.Communication.Scs.Communication.Channels;

namespace ASC.Communication.Scs.Client.Udp
{
    internal class AscUdpClient : AscClientBase
    {
        private readonly ICommunicationChannel _udpchannel;

        public AscUdpClient(ICommunicationChannel udpChannel)
        {
            _udpchannel = udpChannel;
        }

        protected override ICommunicationChannel CreateCommunicationChannel()
        {
            return _udpchannel;
        }
    }
}