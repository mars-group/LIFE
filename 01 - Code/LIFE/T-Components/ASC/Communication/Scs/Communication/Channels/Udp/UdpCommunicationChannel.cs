using System;
using System.Threading.Tasks;
using AppSettingsManager;
using ASC.Communication.Scs.Communication.EndPoints;
using ASC.Communication.Scs.Communication.EndPoints.Udp;
using ASC.Communication.Scs.Communication.Messages;
using MulticastAdapter.Implementation;
using MulticastAdapter.Interface.Config;

namespace ASC.Communication.Scs.Communication.Channels.Udp
{
    class UdpCommunicationChannel : CommunicationChannelBase {
        private AscUdpEndPoint _endPoint;
        private readonly MulticastAdapterComponent _multicastAdapter;
        private bool _running;

        /// <summary>
        ///     This object is just used for thread synchronizing (locking).
        /// </summary>
        private readonly object _syncLock;

        public override AscEndPoint RemoteEndPoint {
            get { throw new UdpCommunicationHasNoRemoteEndpointException(); }
        }


        public UdpCommunicationChannel(AscUdpEndPoint endPoint) {
            _endPoint = endPoint;
            _running = false;
            _multicastAdapter = new MulticastAdapterComponent(
                new GlobalConfig(_endPoint.McastGroup, _endPoint.UdpListenPort, _endPoint.UdpListenPort+1, 4),
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
            //_multicastAdapter.ReopenSocket();
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
