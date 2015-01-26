﻿using System.Net;
using ASC.Communication.Scs.Communication.Channels;
using ASC.Communication.Scs.Communication.Channels.Tcp;
using ASC.Communication.Scs.Communication.EndPoints.Tcp;

namespace ASC.Communication.Scs.Client.Tcp {
    /// <summary>
    ///     This class is used to communicate with server over TCP/IP protocol.
    /// </summary>
    internal class AscTcpClient : AscClientBase {
        /// <summary>
        ///     The endpoint address of the server.
        /// </summary>
        private readonly AscTcpEndPoint _serverEndPoint;

        /// <summary>
        ///     Creates a new AscTcpClient object.
        /// </summary>
        /// <param name="serverEndPoint">The endpoint address to connect to the server</param>
        public AscTcpClient(AscTcpEndPoint serverEndPoint) {
            _serverEndPoint = serverEndPoint;
        }

        /// <summary>
        ///     Creates a communication channel using ServerIpAddress and ServerPort.
        /// </summary>
        /// <returns>Ready communication channel to communicate</returns>
        protected override ICommunicationChannel CreateCommunicationChannel() {
            return new TcpCommunicationChannel(
                TcpHelper.ConnectToServer(
                    new IPEndPoint(IPAddress.Parse(_serverEndPoint.IpAddress), _serverEndPoint.TcpPort),
                    ConnectTimeout
                    ));
        }
    }
}