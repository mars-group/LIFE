//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulticastAdapter.Interface
{
    public interface IMulticastAdapter : IMulticastReceiver, IMulticastSender
    {
        /// <summary>
        ///     Close the underlying communication socket
        /// </summary>
        void CloseSocket();


        /// <summary>
        ///     Reopen the closed socket if it was closed before. If the method is called and the Socket is already open nothing
        ///     happens.
        /// </summary>
        void ReopenSocket();
    }
}