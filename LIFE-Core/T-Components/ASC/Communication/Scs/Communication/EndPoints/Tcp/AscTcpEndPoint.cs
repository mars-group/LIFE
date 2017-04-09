//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

using System;
using ASC.Communication.Scs.Client;
using ASC.Communication.Scs.Client.Tcp;
using ASC.Communication.Scs.Server;
using ASC.Communication.Scs.Server.Tcp;

namespace ASC.Communication.Scs.Communication.EndPoints.Tcp
{
    /// <summary>
    ///     Represens a TCP end point in SCS.
    /// </summary>
    public sealed class AscTcpEndPoint : AscEndPoint
    {
        /// <summary>
        ///     IP address of the server.
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        ///     Listening TCP Port for incoming connection requests on server.
        /// </summary>
        public int TcpPort { get; private set; }

        /// <summary>
        ///     Creates a new AscTcpEndPoint object with specified port number.
        /// </summary>
        /// <param name="tcpPort">Listening TCP Port for incoming connection requests on server</param>
        public AscTcpEndPoint(int tcpPort)
        {
            TcpPort = tcpPort;
        }

        /// <summary>
        ///     Creates a new AscTcpEndPoint object with specified IP address and port number.
        /// </summary>
        /// <param name="ipAddress">IP address of the server</param>
        /// <param name="port">Listening TCP Port for incoming connection requests on server</param>
        public AscTcpEndPoint(string ipAddress, int port)
        {
            IpAddress = ipAddress;
            TcpPort = port;
        }

        /// <summary>
        ///     Creates a new AscTcpEndPoint from a string address.
        ///     Address format must be like IPAddress:Port (For example: 127.0.0.1:10085).
        /// </summary>
        /// <param name="address">TCP end point Address</param>
        /// <returns>Created ScsTcpEndpoint object</returns>
        public AscTcpEndPoint(string address)
        {
            var splittedAddress = address.Trim().Split(':');
            IpAddress = splittedAddress[0].Trim();
            TcpPort = Convert.ToInt32(splittedAddress[1].Trim());
        }

        /// <summary>
        ///     Creates a Scs Server that uses this end point to listen incoming connections.
        /// </summary>
        /// <returns>Scs Server</returns>
        internal override IAscServer CreateServer()
        {
            return new AscTcpServer(this);
        }

        /// <summary>
        ///     Creates a Scs Client that uses this end point to connect to server.
        /// </summary>
        /// <returns>Scs Client</returns>
        internal override IScsClient CreateClient()
        {
            return new AscTcpClient(this);
        }

        /// <summary>
        ///     Generates a string representation of this end point object.
        /// </summary>
        /// <returns>String representation of this end point object</returns>
        public override string ToString()
        {
            return string.IsNullOrEmpty(IpAddress) ? ("tcp://" + TcpPort) : ("tcp://" + IpAddress + ":" + TcpPort);
        }
    }
}