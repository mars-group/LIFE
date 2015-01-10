using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using ASC.Communication.Scs.Communication.EndPoints;
using ASC.Communication.Scs.Communication.EndPoints.Udp;
using ASC.Communication.Scs.Communication.Messages;
using MulticastAdapter.Implementation;
using MulticastAdapter.Interface.Config.Types;

namespace ASC.Communication.Scs.Communication.Channels.Udp
{
    class UdpCommunicationChannel : CommunicationChannelBase
    {
        #region private fields

        private readonly AscUdpEndPoint _endPoint;
        //private readonly MulticastAdapterComponent _multicastAdapter;

        /// <summary>
        ///     A flag to control thread's running
        /// </summary>
        private volatile bool _running;

        /// <summary>
        ///     This object is just used for thread synchronizing (locking).
        /// </summary>
        private readonly object _syncLock;

        private readonly UdpClient _udpReceivingClient;
        private IPEndPoint _listenEndPoint;

        private readonly List<UdpClient> _udpSendingClients;
        private readonly IPAddress _mcastGroupIpAddress;
        private int _sendingPort;
        #endregion

        public override AscEndPoint RemoteEndPoint {
            get { throw new UdpCommunicationHasNoRemoteEndpointException(); }
        }


        public UdpCommunicationChannel(AscUdpEndPoint endPoint) {
            _endPoint = endPoint;

            _running = false;

            _mcastGroupIpAddress = IPAddress.Parse(_endPoint.McastGroup);

            _listenEndPoint = new IPEndPoint(IPAddress.Any, _endPoint.UdpServerListenPort);

            _sendingPort = endPoint.UdpClientListenPort+1;

            _udpReceivingClient = GetReceivingClient(_listenEndPoint);

            JoinMulticastGroup();
            
            _udpSendingClients = GetSendingClients();

            _syncLock = new object();
        }



        public override void Disconnect() {
            if (CommunicationState != CommunicationStates.Connected) return;
            _udpReceivingClient.Close();
            CommunicationState = CommunicationStates.Disconnected;
            OnDisconnected();
        }

        #region protected Methods

        protected override void StartInternal()
        {
            if (_running)
            {
                return;
            }

            _running = true;
            var listenEndPoint = new IPEndPoint(IPAddress.Any, 0);
            var udpState = new UdpState {Endpoint = listenEndPoint, UdpClient = _udpReceivingClient};
            _udpReceivingClient.BeginReceive(ReceiveCallback, udpState);
            /*Task.Run(() =>
            {
                while (_running)
                {
                    var recBytes = _udpReceivingClient.Receive(ref listenEndPoint);
                    
                    var stream = new MemoryStream(recBytes);
                    stream.SetLength(recBytes.Length);
                    IScsMessage msg;
                    try
                    {
                        msg = (IScsMessage) new BinaryFormatter().Deserialize(stream);
                    }
                    finally
                    {
                        stream.Dispose();
                    }
                    
                    
                    OnMessageReceived(msg);
                }

            });*/
        }


        protected override void SendMessageInternal(IScsMessage message)
        {
            //Send message

            lock (_syncLock)
            {
                //Create a byte array from message according to current protocol
                var memoryStream = new MemoryStream();
                
                new BinaryFormatter().Serialize(memoryStream, message);
                 
                
                var messageBytes = memoryStream.ToArray();
                var endpoint = new IPEndPoint(_mcastGroupIpAddress, _endPoint.UdpClientListenPort);

                //Send all bytes to the remote application
                _udpSendingClients.ForEach(client =>
                {
                    client.BeginSend(
                        messageBytes,
                        messageBytes.Length,
                        endpoint,
                        SendCallback,
                        client
                        );
                });

                LastSentMessageTime = DateTime.Now;
                OnMessageSent(message);
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
        }

        #endregion




        #region private Methods

        /// <summary>
        /// Joins the multicast group via all available interfaces
        /// </summary>
        private void JoinMulticastGroup()
        {
            foreach (var networkInterface in MulticastNetworkUtils.GetAllMulticastInterfaces())
            {
                foreach (var unicastAddr in networkInterface.GetIPProperties().UnicastAddresses)
                {
                    if (unicastAddr.Address.AddressFamily == MulticastNetworkUtils.GetAddressFamily(IPVersionType.IPv4))
                        _udpReceivingClient.JoinMulticastGroup(_mcastGroupIpAddress, unicastAddr.Address);
                }
            }
        }

        private UdpClient GetReceivingClient(IPEndPoint listenEndPoint)
        {
            //var udpClient = new UdpClient(listenEndPoint);
            var udpClient = new UdpClient {ExclusiveAddressUse = false};

            // allow another client to bind to this port
            udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udpClient.Client.Bind(listenEndPoint);
            return udpClient;
        }

        private List<UdpClient> GetSendingClients()
        {
            var resultList = new List<UdpClient>();
            foreach (var networkInterface in MulticastNetworkUtils.GetAllMulticastInterfaces())
            {
                foreach (var unicastAddress in networkInterface.GetIPProperties().UnicastAddresses)
                {
                    if (unicastAddress.Address.AddressFamily == MulticastNetworkUtils.GetAddressFamily(IPVersionType.IPv4))
                    {
                        var updClient = SetupSocket(unicastAddress.Address);
                        updClient.MulticastLoopback = false;
                        resultList.Add(updClient);
                    }
                }
            }

            return resultList;
        }

        private UdpClient SetupSocket(IPAddress unicastAddress)
        {
            try
            {
                return new UdpClient(new IPEndPoint(unicastAddress, _sendingPort));
            }
            catch (SocketException socketException)
            {
                //if sending port is already in use increment port and try again.
                if (socketException.ErrorCode != 10048) throw;
                _sendingPort = _sendingPort + 1;
                return SetupSocket(unicastAddress);
            }
        }

        /// <summary>
        ///     This method is used as callback method in _udpReceivingClient's BeginReceive method.
        ///     It reveives bytes from udp socket.
        /// </summary>
        /// <param name="ar">Asyncronous call result</param>
        private void ReceiveCallback(IAsyncResult ar)
        {
            if (!_running) return;

            //try
            //{
                //Get received bytes
                UdpClient udpClient = ((UdpState)(ar.AsyncState)).UdpClient;
            
                IPEndPoint listenEndPoint = ((UdpState)(ar.AsyncState)).Endpoint;
                var bytesRead = _udpReceivingClient.EndReceive(ar, ref listenEndPoint);
                if (bytesRead.Length > 0)
                {
                    LastReceivedMessageTime = DateTime.Now;

                    var stream = new MemoryStream(bytesRead);
                    stream.SetLength(bytesRead.Length);
                    IScsMessage msg;
                    try
                    {
                        msg = (IScsMessage) new BinaryFormatter().Deserialize(stream);
                    }
                    finally
                    {
                        stream.Dispose();
                    }


                    OnMessageReceived(msg);
                }
                else throw new CommunicationException("Udp socket is closed");

                //Read more bytes if still running
                if (_running) _udpReceivingClient.BeginReceive(ReceiveCallback, ar.AsyncState);
            /*}
            catch (Exception ex)
            {
                Disconnect();
                throw;
            }*/
        }

        private class UdpState
        {
            public IPEndPoint Endpoint { get; set; }
            public UdpClient UdpClient { get; set; }
        }

        #endregion
    }

    internal class UdpCommunicationHasNoRemoteEndpointException : Exception {}
}
