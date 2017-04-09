//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

using System;

namespace Hik.Communication.Scs.Communication.Channels
{
    /// <summary>
    ///     This class provides base functionality for communication listener classes.
    /// </summary>
    internal abstract class ConnectionListenerBase : IConnectionListener
    {
        /// <summary>
        ///     This event is raised when a new communication channel is connected.
        /// </summary>
        public event EventHandler<CommunicationChannelEventArgs> CommunicationChannelConnected;

        /// <summary>
        ///     Starts listening incoming connections.
        /// </summary>
        public abstract void Start();

        /// <summary>
        ///     Stops listening incoming connections.
        /// </summary>
        public abstract void Stop();

        /// <summary>
        ///     Raises CommunicationChannelConnected event.
        /// </summary>
        /// <param name="client"></param>
        protected virtual void OnCommunicationChannelConnected(ICommunicationChannel client)
        {
            var handler = CommunicationChannelConnected;
            if (handler != null) handler(this, new CommunicationChannelEventArgs(client));
        }
    }
}