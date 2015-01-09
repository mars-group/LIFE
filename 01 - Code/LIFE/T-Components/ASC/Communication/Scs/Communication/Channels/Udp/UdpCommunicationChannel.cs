using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ASC.Communication.Scs.Communication.EndPoints;
using ASC.Communication.Scs.Communication.EndPoints.Udp;
using ASC.Communication.Scs.Communication.Messages;
using MulticastAdapter.Implementation;
using MulticastAdapter.Interface.Config.Types;

namespace ASC.Communication.Scs.Communication.Channels.Udp
{
    class UdpCommunicationChannel : CommunicationChannelBase {
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
        private readonly IPEndPoint _listenEndPoint;

        private readonly List<UdpClient> _udpSendingClients;
        private readonly IPAddress _mcastGroupIpAddress;
        private int _sendingPort;


        public override AscEndPoint RemoteEndPoint {
            get { throw new UdpCommunicationHasNoRemoteEndpointException(); }
        }


        public UdpCommunicationChannel(AscUdpEndPoint endPoint) {
            _endPoint = endPoint;
            _running = false;
            _sendingPort = endPoint.UdpListenPort + 1;
            _mcastGroupIpAddress = IPAddress.Parse(_endPoint.McastGroup);

            /*_multicastAdapter = new MulticastAdapterComponent(
                new GlobalConfig(_endPoint.McastGroup, _endPoint.UdpListenPort, _endPoint.UdpListenPort+1, 4),
                new MulticastSenderConfig()
            );
             */

            _listenEndPoint = new IPEndPoint(IPAddress.Any, _endPoint.UdpListenPort);
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

        protected override void StartInternal() {
            _running = true;
            var udpState = new UdpState {Endpoint = _listenEndPoint, UdpClient = _udpReceivingClient};
            _udpReceivingClient.BeginReceive(ReceiveCallback, udpState);
        }


        protected override void SendMessageInternal(IScsMessage message)
        {
            //Send message

            lock (_syncLock)
            {
                //Create a byte array from message according to current protocol
                var messageBytes = WireProtocol.GetBytes(message);

                //Send all bytes to the remote application
                Parallel.ForEach(_udpSendingClients, client =>
                {
                    client.Send(messageBytes, messageBytes.Length, new IPEndPoint(_mcastGroupIpAddress, _endPoint.UdpListenPort));
                });

                LastSentMessageTime = DateTime.Now;
                OnMessageSent(message);
            }
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
                        updClient.JoinMulticastGroup(_mcastGroupIpAddress, unicastAddress.Address);
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

            try
            {
                //Get received bytes
                //UdpClient udpClient = ((UdpState)(ar.AsyncState)).UdpClient;
                IPEndPoint listenEndPoint = ((UdpState)(ar.AsyncState)).Endpoint;
                var bytesRead = _udpReceivingClient.EndReceive(ar, ref listenEndPoint);
                if (bytesRead.Length > 0)
                {
                    LastReceivedMessageTime = DateTime.Now;

                    //Read messages according to current wire protocol
                    var messages = WireProtocol.CreateMessages(bytesRead);

                    //Raise MessageReceived event for all received messages
                    foreach (var message in messages)
                    {
                        OnMessageReceived(message);
                    }
                }
                else throw new CommunicationException("Udp socket is closed");

                //Read more bytes if still running
                if (_running) _udpReceivingClient.BeginReceive(ReceiveCallback, ar.AsyncState);
            }
            catch (Exception ex)
            {
                Disconnect();
                throw;
            }
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
