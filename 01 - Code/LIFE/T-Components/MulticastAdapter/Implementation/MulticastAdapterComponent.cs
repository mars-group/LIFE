using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MulticastAdapter.Interface;
using MulticastAdapter.Interface.Config;

namespace MulticastAdapter.Implementation
{
    public class MulticastAdapterComponent : IMulticastAdapter
    {

        #region properties & fields
        private IMulticastSender _sender;
        private IMulticastReciever _reciever;
        private Thread _listenThread;
        #endregion

        #region Constructors
        public MulticastAdapterComponent()
        {
            _sender = new UDPMulticastSender();
            _reciever = new UDPMulticastReceiver();

        }

        public MulticastAdapterComponent(GeneralMulticastAdapterConfig generalConfig, MulticastSenderConfig senderConfig)
        {
            _sender = new UDPMulticastSender(generalConfig, senderConfig);
            _reciever = new UDPMulticastReceiver(generalConfig);

        }
        #endregion

        public byte[] readMulticastGroupMessage()
        {
            return _reciever.readMulticastGroupMessage();
        }

        public void SendMessageToMulticastGroup(byte[] msg)
        {
            _sender.SendMessageToMulticastGroup(msg);
        }

        public void CloseSocket()
        {
            _sender.CloseSocket();
            _reciever.CloseSocket();
        }

        public void ReopenSocket()
        {
            _sender.ReopenSocket();
            _reciever.ReopenSocket();

        }
    }
}
