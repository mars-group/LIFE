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
using System.Threading;
using Hik.Communication.Scs.Communication.EndPoints;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.Scs.Communication.Messages;
using Hik.Scs.Communication.Scs.Communication.Channels.Tcp;

namespace Hik.Communication.Scs.Communication.Channels.Tcp {
    /// <summary>
    ///     This class is used to communicate with a remote application over TCP/IP protocol.
    /// </summary>
    internal class TcpCommunicationChannel : CommunicationChannelBase {
        #region Public properties

        /// <summary>
        ///     Gets the endpoint of remote application.
        /// </summary>
        public override ScsEndPoint RemoteEndPoint {
            get { return _remoteEndPoint; }
        }

        private readonly ScsTcpEndPoint _remoteEndPoint;

        #endregion

        #region Private fields

        /// <summary>
        ///     Size of the buffer that is used to receive bytes from TCP socket.
        /// </summary>
        private const int ReceiveBufferSize = 8*1024; //8KB

        private const int NumReadConnections = 10;
        private const int NumWriteConnections = 10;

        private readonly BufferManager _readBufferManager;
        //private readonly BufferManager _writeBufferManager;

        private readonly SocketAsyncEventArgsPool _readPool;

        private readonly Semaphore _maxNumberReadClients;

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

        private Thread _listenThread;

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
            if(clientSocket == null) { throw new ArgumentNullException(nameof(clientSocket));}
            _clientSocket = clientSocket;
            _clientSocket.NoDelay = true;

            if(_clientSocket.RemoteEndPoint == null) { Console.WriteLine("ERROR ENDPOINT NULL!");}

            var ipEndPoint = (IPEndPoint) _clientSocket.RemoteEndPoint;
            _remoteEndPoint = new ScsTcpEndPoint(ipEndPoint.Address.ToString(), ipEndPoint.Port);

            _readBufferManager = new BufferManager(ReceiveBufferSize * NumReadConnections, ReceiveBufferSize);
            _readPool = new SocketAsyncEventArgsPool(NumReadConnections);

            _maxNumberReadClients = new Semaphore(NumReadConnections, NumReadConnections);

            //_buffer = new byte[ReceiveBufferSize];
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
                if (_clientSocket.Connected) _clientSocket.Shutdown(SocketShutdown.Both);

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
            // Allocates one large byte buffer which all I/O operations use a piece of.  This gaurds
            // against memory fragmentation
            _readBufferManager.InitBuffer();

            for (var i = 0; i < NumReadConnections; i++)
            {
                //Pre-allocate a set of reusable SocketAsyncEventArgs
                var readEventArg = new SocketAsyncEventArgs();
                readEventArg.Completed += IO_Completed;

                // receive only from connected remote endpoint! NOT:receive from every address and port
                readEventArg.RemoteEndPoint = _clientSocket.RemoteEndPoint; //new IPEndPoint(IPAddress.Any, 0);

                // assign a byte buffer from the buffer pool to the SocketAsyncEventArg object
                _readBufferManager.SetBuffer(readEventArg);

                // add SocketAsyncEventArg to the pool
                _readPool.Push(readEventArg);
            }

            _running = true;
            // begin to receive in Thread to not block the sending side
            _listenThread = new Thread(Receive) {IsBackground = true};
            _listenThread.Start();
            //_clientSocket.BeginReceive(_buffer, 0, _buffer.Length, 0, ReceiveCallback, null);
        }


        #endregion

        #region Private methods

        /// <summary>
        ///     Sends a message to the remote application.
        /// </summary>
        /// <param name="message">Message to be sent</param>
        protected override void SendMessageInternal(IScsMessage message)
        {
            //Send message
            var totalSent = 0;
            lock (_syncLock)
            {
                //Create a byte array from message according to current protocol
                var messageBytes = WireProtocol.GetBytes(message);
                //Send all bytes to the remote application
                while (totalSent < messageBytes.Length)
                {
                    var sent = _clientSocket.Send(messageBytes, totalSent, messageBytes.Length - totalSent,
                        SocketFlags.None);
                    if (sent <= 0)
                    {
                        throw new CommunicationException("Message could not be sent via TCP socket. Only " + totalSent +
                                                         " bytes of " + messageBytes.Length + " bytes are sent.");
                    }

                    totalSent += sent;
                }

                LastSentMessageTime = DateTime.Now;
                OnMessageSent(message);
            }
        }

        private void Receive()
        {
            while (true)
            {
                _maxNumberReadClients.WaitOne();
                // Pop a SocketAsyncEventArgs object from the stack
                var readEventArgs = _readPool.Pop();

                // As soon as the client is connected, post a receive to the connection
                var willRaiseEvent = _clientSocket.ReceiveAsync(readEventArgs);
                if (!willRaiseEvent)
                {
                    // operation completed synchronously
                    ProcessReceive(readEventArgs);
                }
                // Accept the next connection request
            }
        }

        // This method is called whenever a receive or send operation is completed on a socket
        //
        // <param name="e">SocketAsyncEventArg associated with the completed receive operation</param>
        private void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            // determine which type of operation just completed and call the associated handler
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.ReceiveFrom:
                    ProcessReceive(e);
                    break;
                case SocketAsyncOperation.SendTo:
                   // ProcessSend(e);
                    break;
                default:
                    throw new ArgumentException("The last operation completed on the socket was not a receive or send");
            }

        }

        // This method is invoked when an asynchronous receive operation completes.
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            var bytesRead = e.BytesTransferred;
            if (bytesRead > 0)
            {
                //Copy received bytes to a new byte array
                var receivedBytes = new byte[bytesRead];
                Array.Copy(_buffer, 0, receivedBytes, 0, bytesRead);

                //Read messages according to current wire protocol
                var messages = WireProtocol.CreateMessages(receivedBytes);

                //Raise MessageReceived event for all received messages
                foreach (var message in messages)
                {
                    OnMessageReceived(message);
                }
            }
            else
            {
                throw new CommunicationException("Tcp socket is closed");
            }


            // put the SocketAsyncEventArg object back onto the stack for later usage
            _readPool.Push(e);
            // release the semaphore
            _maxNumberReadClients.Release();

            // currently nothing, later on: Handle messages too large for the buffer,
            // this will need a 4 digit prefix to indicate the message size
        }

        #endregion
    }
}