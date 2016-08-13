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
using ASC.Communication.Scs.Communication.EndPoints;
using ASC.Communication.Scs.Communication.EndPoints.Tcp;
using ASC.Communication.Scs.Communication.Messages;

namespace ASC.Communication.Scs.Communication.Channels.Tcp {
    /// <summary>
    ///     This class is used to communicate with a remote application over TCP/IP protocol.
    /// </summary>
    internal class TcpCommunicationChannel : CommunicationChannelBase {
        #region Public properties

        /// <summary>
        ///     Gets the endpoint of remote application.
        /// </summary>
        public override AscEndPoint RemoteEndPoint {
            get { return _remoteEndPoint; }
        }

        private readonly AscTcpEndPoint _remoteEndPoint;

        #endregion

        #region Private fields

        /// <summary>
        ///     Size of the buffer that is used to receive bytes from TCP socket.
        /// </summary>
        private const int ReceiveBufferSize = 4*1024; //4KB

        /// <summary>
        ///     This buffer is used to receive bytes
        /// </summary>
        private readonly byte[] _buffer;

        /// <summary>
        ///     Socket object to send/reveice messages.
        /// </summary>
        private readonly Socket _clientSocket;

        /// <summary>
        ///     A flag to control thread's running
        /// </summary>
        private volatile bool _running;

        /// <summary>
        ///     This object is just used for thread synchronizing (locking).
        /// </summary>
        private readonly object _syncLock;

        #endregion

        #region Constructor

        /// <summary>
        ///     Creates a new TcpCommunicationChannel object.
        /// </summary>
        /// <param name="clientSocket">
        ///     A connected Socket object that is
        ///     used to communicate over network
        /// </param>
        public TcpCommunicationChannel(Socket clientSocket) {
            _clientSocket = clientSocket;
            _clientSocket.NoDelay = true;

            var ipEndPoint = (IPEndPoint) _clientSocket.RemoteEndPoint;
            _remoteEndPoint = new AscTcpEndPoint(ipEndPoint.Address.ToString(), ipEndPoint.Port);

            _buffer = new byte[ReceiveBufferSize];
            _syncLock = new object();
        }

        #endregion

        #region Public methods

        /// <summary>
        ///     Disconnects from remote application and closes channel.
        /// </summary>
        public override void Disconnect() {
            if (CommunicationState != CommunicationStates.Connected) return;

            _running = false;
            try {
                if (_clientSocket.Connected) _clientSocket.Close();

                _clientSocket.Dispose();
            }
            catch {}

            CommunicationState = CommunicationStates.Disconnected;
            OnDisconnected();
        }

        #endregion

        #region Protected methods

        /// <summary>
        ///     Starts the thread to receive messages from socket.
        /// </summary>
        protected override void StartInternal() {
            _running = true;
            _clientSocket.BeginReceive(_buffer, 0, _buffer.Length, 0, ReceiveCallback, null);
        }

        /// <summary>
        ///     Sends a message to the remote application.
        /// </summary>
        /// <param name="message">Message to be sent</param>
        protected override void SendMessageInternal(IAscMessage message) {
            //Send message
            var totalSent = 0;
            lock (_syncLock) {
                //Create a byte array from message according to current protocol
                var messageBytes = WireProtocol.GetBytes(message);
                //Send all bytes to the remote application
                while (totalSent < messageBytes.Length) {
                    var sent = _clientSocket.Send(messageBytes, totalSent, messageBytes.Length - totalSent,
                        SocketFlags.None);
                    if (sent <= 0) {
                        throw new CommunicationException("Message could not be sent via TCP socket. Only " + totalSent +
                                                         " bytes of " + messageBytes.Length + " bytes are sent.");
                    }

                    totalSent += sent;
                }

                LastSentMessageTime = DateTime.Now;
                OnMessageSent(message);
            }
        }
        
        #endregion

        #region Private methods

        /// <summary>
        ///     This method is used as callback method in _clientSocket's BeginReceive method.
		///     It reveives bytes from socket.
        /// </summary>
        /// <param name="ar">Asyncronous call result</param>
        private void ReceiveCallback(IAsyncResult ar) {
            if (!_running) return;

            try {
                //Get received bytes count
                var bytesRead = _clientSocket.EndReceive(ar);
                if (bytesRead > 0) {
                    LastReceivedMessageTime = DateTime.Now;

                    //Copy received bytes to a new byte array
                    var receivedBytes = new byte[bytesRead];
                    Array.Copy(_buffer, 0, receivedBytes, 0, bytesRead);

                    //Read messages according to current wire protocol
                    var messages = WireProtocol.CreateMessages(receivedBytes);

                    //Raise MessageReceived event for all received messages
                    foreach (var message in messages) {
                        OnMessageReceived(message);
                    }
                }
                else throw new CommunicationException("Tcp socket is closed");

                //Read more bytes if still running
                if (_running) _clientSocket.BeginReceive(_buffer, 0, _buffer.Length, 0, ReceiveCallback, null);
            }
            catch(Exception ex) {
                Disconnect();
                //throw;
            }
        }

        #endregion
    }
}