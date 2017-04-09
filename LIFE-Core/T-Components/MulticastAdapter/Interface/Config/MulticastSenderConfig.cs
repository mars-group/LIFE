//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

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

        public MulticastSenderConfig(bool sendOnAllInterfaces, string sendingInterfaceName, string sendingInterfaceIp,
            BindingType bindingType)
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