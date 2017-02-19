//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using ASC.Communication.Scs.Communication.Channels;
using ASC.Communication.Scs.Communication.Channels.Tcp;
using ASC.Communication.Scs.Communication.EndPoints.Tcp;
using ASC.Communication.Scs.Communication.Messengers;

namespace ASC.Communication.Scs.Server.Tcp {
    /// <summary>
    ///     This class is used to create a TCP server.
    /// </summary>
    internal class AscTcpServer : AscServerBase {
        /// <summary>
        ///     The endpoint address of the server to listen incoming connections.
        /// </summary>
        private readonly AscTcpEndPoint _endPoint;

        /// <summary>
        ///     Creates a new AscTcpServer object.
        /// </summary>
        /// <param name="endPoint">The endpoint address of the server to listen incoming connections</param>
        public AscTcpServer(AscTcpEndPoint endPoint) {
            _endPoint = endPoint;
        }

        public override IMessenger GetMessenger() {
            throw new System.NotImplementedException();
        }

        /// <summary>
        ///     Creates a TCP connection listener.
        /// </summary>
        /// <returns>Created listener object</returns>
        protected override IConnectionListener CreateConnectionListener() {
            return new TcpConnectionListener(_endPoint);
        }
    }
}