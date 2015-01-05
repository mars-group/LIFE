using System;
using MulticastAdapter.Interface.Config.Types;

namespace MulticastAdapter.Interface.Config
{
    public class MulticastSenderConfig
    {
        //if true send on all relevant multicast interfaces
        public Boolean SendOnAllInterfaces;

        #region single sending interface options
        // determines by which type the binding interface should be chosen
        public BindingType BindingType;
        public string SendingInterfaceName;
        public string SendingInterfaceIP;
        #endregion

        public MulticastSenderConfig(bool sendOnAllInterfaces, string sendingInterfaceName, string sendingInterfaceIp, BindingType bindingType)
        {
            this.SendOnAllInterfaces = sendOnAllInterfaces;
            SendingInterfaceName = sendingInterfaceName;
            SendingInterfaceIP = sendingInterfaceIp;
            BindingType = bindingType;
        }

        public MulticastSenderConfig()
        {
            this.SendOnAllInterfaces = true;
            SendingInterfaceName = "Ethernet";
            SendingInterfaceIP = "127.0.0.1";
            BindingType = BindingType.IP;
        }
      
    }

}


