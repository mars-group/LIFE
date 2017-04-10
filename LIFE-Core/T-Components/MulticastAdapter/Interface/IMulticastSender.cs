//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

namespace MulticastAdapter.Interface
{
    /// <summary>
    /// Sends byte messages to a configured multicastgroup. The Adapter can be configured to send on multible networkinterfaces which are able to multicast.
    /// </summary>
    public interface IMulticastSender
    {
        /// <summary>
        ///     sends a message to the configured multicastgroup on the configured Multicastinterfaces
        ///     </summary>
        /// <param name="msg">the byte message</param>
        void SendMessageToMulticastGroup(byte[] msg);

        /// <summary>
        ///     Close the underlying communication socket
        /// </summary>
        void CloseSocket();

        /// <summary>
        ///     Reopen the closed socket if it was closed before. If the method is called and the Socket is already open nothing
        ///     happend.
        /// </summary>
        void ReopenSocket();
    }
}