//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System;

namespace Hik.Communication.Scs.Communication.Channels {
    /// <summary>
    ///     Represents a communication listener.
    ///     A connection listener is used to accept incoming client connection requests.
    /// </summary>
    internal interface IConnectionListener {
        /// <summary>
        ///     This event is raised when a new communication channel connected.
        /// </summary>
        event EventHandler<CommunicationChannelEventArgs> CommunicationChannelConnected;

        /// <summary>
        ///     Starts listening incoming connections.
        /// </summary>
        void Start();

        /// <summary>
        ///     Stops listening incoming connections.
        /// </summary>
        void Stop();
    }
}