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
using ASC.Communication.ScsServices.Communication.Messages;
using MsgPack.Serialization;
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

        /// <summary>
        /// The UDP receiver client
        /// </summary>
        private readonly UdpClient _udpReceivingClient;

        /// <summary>
        /// All UDP sending clients. One client per interface is created
        /// </summary>
        private readonly List<UdpClient> _udpSendingClients;

        /// <summary>
        /// The multicast address being used for this communicationchannel
        /// </summary>
        private readonly IPAddress _mcastGroupIpAddress;

        /// <summary>
        /// The port to start sending on. Will automatically be increased if already in use
        /// </summary>
        private int _sendingStartPort;

        /// <summary>
        /// The Formatter used to serialize and de-serialize
        /// </summary>
        private readonly BinaryFormatter _binaryFormatter;

        private MessagePackSerializer<AscMessage> _mspPackSerializer;

        #endregion

        /// <summary>
        /// At the moment we don't need a remote endpoint to call methods on the other side.
        /// </summary>
        public override AscEndPoint RemoteEndPoint {
            get { throw new UdpCommunicationHasNoRemoteEndpointException(); }
        }


        public UdpCommunicationChannel(AscUdpEndPoint endPoint) {
            _endPoint = endPoint;
            // running is false, not yet
            _running = false;
            // create multicast IPAddress
            _mcastGroupIpAddress = IPAddress.Parse(_endPoint.McastGroup);
            // sending port starts with listenport +1, will be increased if port is not availabel
            _sendingStartPort = endPoint.UdpPort+1;
            // get a receiving UDP client which listens on all interfaces and every port
            _udpReceivingClient = GetReceivingClient(new IPEndPoint(IPAddress.Any, _endPoint.UdpPort));
            // Join multicast groups with all receiving clients
            JoinMulticastGroup();
            // get all sending udpClients. One per active and multicast enabled interface
            _udpSendingClients = GetSendingClients();
            // a lock object to be used for sending method
            _syncLock = new object();

            _mspPackSerializer = MessagePackSerializer.Get<AscMessage>();
        }



        public override void Disconnect() {
            // do nothing atm... TODO: think about it

            /*if (CommunicationState != CommunicationStates.Connected) return;
            _udpReceivingClient.Close();
            Parallel.ForEach(_udpSendingClients, client => client.Close());
            CommunicationState = CommunicationStates.Disconnected;
            OnDisconnected();
            */
        }

        #region protected Methods

        protected override void StartInternal()
        {
            // check if running so as to not call this multiple times from different client objects
            if (_running)
            {
                // nothing to be done, so return
                return;
            }

            _running = true;
            var listenEndPoint = new IPEndPoint(IPAddress.Any, 0);
            var udpState = new UdpState {Endpoint = listenEndPoint, UdpClient = _udpReceivingClient};
            // start receiving in an asynchronous way
            _udpReceivingClient.BeginReceive(ReceiveCallback, udpState);
        }


        protected override void SendMessageInternal(IAscMessage message)
        {
            //Create a byte array from message according to current protocol
            var memoryStream = new MemoryStream();

            var msgType = message.GetType();
            var serializer = MessagePackSerializer.Get(msgType);
            //if (msgType == typeof (AscRemoteInvokeMessage)) {

                //var serializer = MessagePackSerializer.Get<AscMessage>();
                serializer.Pack(memoryStream, message);
            //}

                 
                
            var messageBytes = memoryStream.ToArray();
            var endpoint = new IPEndPoint(_mcastGroupIpAddress, _endPoint.UdpPort);

            lock (_syncLock)
                {
                //Send all bytes to the remote application asynchronously
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
                // store last time a message was sent
                LastSentMessageTime = DateTime.Now;
                OnMessageSent(message);
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            // nothing to be done here
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
            if (listenEndPoint == null) throw new ArgumentNullException("listenEndPoint");
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

        /// <summary>
        /// Sets up the sending socket.
        /// Will increase the sending port, if it is already in use
        /// </summary>
        /// <param name="unicastAddress"></param>
        /// <returns></returns>
        private UdpClient SetupSocket(IPAddress unicastAddress)
        {
            try
            {
                return new UdpClient(new IPEndPoint(unicastAddress, _sendingStartPort));
            }
            catch (SocketException socketException)
            {
                //if sending port is already in use increment port and try again.
                if (socketException.ErrorCode != 10048) throw;
                _sendingStartPort = _sendingStartPort + 1;
                return SetupSocket(unicastAddress);
            }
        }

        /// <summary>
        ///     This method is used as callback method in _udpReceivingClient's BeginReceive method.
        ///     It receives bytes from udp socket.
        /// </summary>
        /// <param name="ar">Asyncronous call result</param>
        private void ReceiveCallback(IAsyncResult ar)
        {
            if (!_running) return;

            try
            {
                // fetch listenendpoint from status object
                var listenEndPoint = ((UdpState)(ar.AsyncState)).Endpoint;
                // receive the complete datagram
                var bytesRead = _udpReceivingClient.EndReceive(ar, ref listenEndPoint);
                if (bytesRead.Length > 0)
                {
                    LastReceivedMessageTime = DateTime.Now;

                    var stream = new MemoryStream(bytesRead);
                    stream.SetLength(bytesRead.Length);

                    // deserialize
                    IAscMessage msg;
                    try {
                        
                        var baseMessage = _mspPackSerializer.Unpack(stream);
                        stream.Position = 0;
                        var msgType = Type.GetType(baseMessage.ActualMessageType);
                        var actualSerializer = MessagePackSerializer.Get(msgType);
                        msg = (IAscMessage) actualSerializer.Unpack(stream);
                    }
                    finally
                    {
                        stream.Dispose();
                    }
                    // inform all listeners about the new message
                    OnMessageReceived(msg);
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

    [Serializable]
    internal class UdpCommunicationHasNoRemoteEndpointException : Exception {}
}
