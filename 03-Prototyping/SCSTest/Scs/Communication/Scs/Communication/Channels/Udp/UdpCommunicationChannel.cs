using System;
using System.Threading.Tasks;
using AppSettingsManager;
using Hik.Communication.Scs.Communication.EndPoints;
using Hik.Communication.Scs.Communication.EndPoints.Udp;
using Hik.Communication.Scs.Communication.Messages;
using MulticastAdapter.Implementation;
using MulticastAdapter.Interface.Config;

namespace Hik.Communication.Scs.Communication.Channels.Udp
{
    class UdpCommunicationChannel : CommunicationChannelBase {
        private ScsUdpEndPoint _endPoint;
        private readonly MulticastAdapterComponent _multicastAdapter;
        private bool _running;

        /// <summary>
        ///     This object is just used for thread synchronizing (locking).
        /// </summary>
        private readonly object _syncLock;

        public override ScsEndPoint RemoteEndPoint {
            get { throw new UdpCommunicationHasNoRemoteEndpointException(); }
        }


        public UdpCommunicationChannel(ScsUdpEndPoint endPoint) {
            _endPoint = endPoint;
            _running = false;
            _multicastAdapter = new MulticastAdapterComponent(
                new GlobalConfig(_endPoint.IpAddress, _endPoint.UdpPort, 50000, 4),
                new MulticastSenderConfig()
            );
            _syncLock = new object();
        }

        public override void Disconnect() {
            if (CommunicationState != CommunicationStates.Connected) return;
            _multicastAdapter.CloseSocket();
            CommunicationState = CommunicationStates.Disconnected;
            OnDisconnected();
        }

        protected override void StartInternal() {
            _running = true;
            _multicastAdapter.ReopenSocket();
            Task.Run(() => ListenAndReceive());
        }

        private void ListenAndReceive() {
            while (_running) {
                var receivedBytes = _multicastAdapter.readMulticastGroupMessage();

                //Read messages according to current wire protocol
                var messages = WireProtocol.CreateMessages(receivedBytes);

                //Raise MessageReceived event for all received messages
                foreach (var message in messages)
                {
                    OnMessageReceived(message);
                }
            }
        }

        protected override void SendMessageInternal(IScsMessage message) {
            //Send message

            lock (_syncLock)
            {
                //Create a byte array from message according to current protocol
                var messageBytes = WireProtocol.GetBytes(message);

                //Send all bytes to the remote application
                _multicastAdapter.SendMessageToMulticastGroup(messageBytes);

                LastSentMessageTime = DateTime.Now;
                OnMessageSent(message);
            }
        }
    }

    internal class UdpCommunicationHasNoRemoteEndpointException : Exception {}
}
