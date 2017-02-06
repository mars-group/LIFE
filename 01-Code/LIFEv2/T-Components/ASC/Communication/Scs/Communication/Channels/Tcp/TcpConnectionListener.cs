//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ASC.Communication.Scs.Communication.EndPoints.Tcp;

namespace ASC.Communication.Scs.Communication.Channels.Tcp {
    /// <summary>
    ///     This class is used to listen and accept incoming TCP
    ///     connection requests on a TCP port.
    /// </summary>
    internal class TcpConnectionListener : ConnectionListenerBase {
        /// <summary>
        ///     The endpoint address of the server to listen incoming connections.
        /// </summary>
        private readonly AscTcpEndPoint _endPoint;

        /// <summary>
        ///     Server socket to listen incoming connection requests.
        /// </summary>
        private TcpListener _listenerSocket;

        /// <summary>
        ///     The thread to listen socket
        /// </summary>
        private Thread _thread;

        /// <summary>
        ///     A flag to control thread's running
        /// </summary>
        private volatile bool _running;

        /// <summary>
        ///     Creates a new TcpConnectionListener for given endpoint.
        /// </summary>
        /// <param name="endPoint">The endpoint address of the server to listen incoming connections</param>
        public TcpConnectionListener(AscTcpEndPoint endPoint) {
            _endPoint = endPoint;
        }

        /// <summary>
        ///     Starts listening incoming connections.
        /// </summary>
        public override void Start() {
            StartSocket();
            _running = true;
            _thread = new Thread(DoListenAsThread);
            _thread.Start();
        }

        /// <summary>
        ///     Stops listening incoming connections.
        /// </summary>
        public override void Stop() {
            _running = false;
            StopSocket();
        }

        /// <summary>
        ///     Starts listening socket.
        /// </summary>
        private void StartSocket() {
            _listenerSocket = new TcpListener(IPAddress.Any, _endPoint.TcpPort); 
            _listenerSocket.Start();
        }

        /// <summary>
        ///     Stops listening socket.
        /// </summary>
        private void StopSocket() {
            try {
                _listenerSocket.Stop();
            }
            catch {}
        }

        /// <summary>
        ///     Entrance point of the thread.
        ///     This method is used by the thread to listen incoming requests.
        /// </summary>
        private void DoListenAsThread()
        {
            while (_running)
            {
                try
                {
                    var clientSocketTask = _listenerSocket.AcceptSocketAsync();
                    var clientSocket = clientSocketTask.Result;
                    if (clientSocket.Connected)
                        OnCommunicationChannelConnected(new TcpCommunicationChannel(clientSocket));
                }
                catch(Exception ex)
                {
                    Console.Write($"Caught an exception in SCS listening Thread. Exception: {ex}");
                    //Disconnect, wait for a while and connect again.
                    StopSocket();
                    Thread.Sleep(1000);
                    if (!_running) return;

                    try
                    {
                        StartSocket();
                    }
                    catch (Exception ex2)
                    {
                        Console.Write($"Caught an exception in SCS while restarting listening Thread. Exception: {ex2}");
                    }
                }
            }
        }
    }
}