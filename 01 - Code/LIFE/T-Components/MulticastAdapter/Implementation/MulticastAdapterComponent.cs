using System.Configuration;
using System.Threading;
using AppSettingsManager;
using ConfigurationAdapter.Interface;
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
       

        public MulticastAdapterComponent(GlobalConfig globalConfiguration, MulticastSenderConfig senderConfiguration)
        {
            _sender = new UDPMulticastSender(globalConfiguration, senderConfiguration);
            _reciever = new UDPMulticastReceiver(globalConfiguration);

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
