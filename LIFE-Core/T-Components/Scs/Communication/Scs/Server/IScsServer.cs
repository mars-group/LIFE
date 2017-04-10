//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

using System;
using CustomUtilities.Collections;
using Hik.Communication.Scs.Communication.Protocols;

namespace Hik.Communication.Scs.Server
{
    /// <summary>
    ///     Represents a SCS server that is used to accept and manage client connections.
    /// </summary>
    public interface IScsServer
    {
        /// <summary>
        ///     This event is raised when a new client connected to the server.
        /// </summary>
        event EventHandler<ServerClientEventArgs> ClientConnected;

        /// <summary>
        ///     This event is raised when a client disconnected from the server.
        /// </summary>
        event EventHandler<ServerClientEventArgs> ClientDisconnected;

        /// <summary>
        ///     Gets/sets wire protocol factory to create IWireProtocol objects.
        /// </summary>
        IScsWireProtocolFactory WireProtocolFactory { get; set; }

        /// <summary>
        ///     A collection of clients that are connected to the server.
        /// </summary>
        ThreadSafeSortedList<long, IScsServerClient> Clients { get; }

        /// <summary>
        ///     Starts the server.
        /// </summary>
        void Start();

        /// <summary>
        ///     Stops the server.
        /// </summary>
        void Stop();
    }
}