//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System;
using System.Net;
using System.Net.Sockets;

namespace Hik.Communication.Scs.Client.Tcp {
    /// <summary>
    ///     This class is used to simplify TCP socket operations.
    /// </summary>
    internal static class TcpHelper {
        /// <summary>
        ///     This code is used to connect to a TCP socket with timeout option.
        /// </summary>
        /// <param name="endPoint">IP endpoint of remote server</param>
        /// <param name="timeoutMs">Timeout to wait until connect</param>
        /// <returns>Socket object connected to server</returns>
        /// <exception cref="SocketException">Throws SocketException if can not connect.</exception>
        /// <exception cref="TimeoutException">Throws TimeoutException if can not connect within specified timeoutMs</exception>
        public static Socket ConnectToServer(EndPoint endPoint, int timeoutMs)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Connect(endPoint);
                return socket;
            }
            catch (SocketException socketException)
            {
                socket.Close();
                throw;
            }
        }
    }
}