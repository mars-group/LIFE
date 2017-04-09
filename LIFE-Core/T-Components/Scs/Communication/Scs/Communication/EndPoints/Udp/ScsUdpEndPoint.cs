//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

using System;
using Hik.Communication.Scs.Client;
using Hik.Communication.Scs.Client.Udp;
using Hik.Communication.Scs.Server;
using Hik.Communication.Scs.Server.Udp;

namespace Hik.Communication.Scs.Communication.EndPoints.Udp
{
    public class ScsUdpEndPoint : ScsEndPoint
    {
        public string IpAddress { get; private set; }

        public int UdpPort { get; set; }

        public ScsUdpEndPoint(string address)
        {
            var splittedAddress = address.Trim().Split(':');
            IpAddress = splittedAddress[0].Trim();
            UdpPort = Convert.ToInt32(splittedAddress[1].Trim());
        }


        internal override IScsServer CreateServer()
        {
            return new ScsUdpServer(this);
        }

        internal override IScsClient CreateClient()
        {
            return new ScsUdpClient(this);
        }
    }
}