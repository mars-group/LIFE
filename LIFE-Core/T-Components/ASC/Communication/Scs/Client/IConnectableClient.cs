//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

using System;
using ASC.Communication.Scs.Communication;

namespace ASC.Communication.Scs.Client
{
    /// <summary>
    ///     Represents a client for SCS servers.
    /// </summary>
    public interface IConnectableClient : IDisposable
    {
        /// <summary>
        ///     This event is raised when client connected to server.
        /// </summary>
        event EventHandler Connected;

        /// <summary>
        ///     This event is raised when client disconnected from server.
        /// </summary>
        event EventHandler Disconnected;

        /// <summary>
        ///     Timeout for connecting to a server (as milliseconds).
        ///     Default value: 15 seconds (15000 ms).
        /// </summary>
        int ConnectTimeout { get; set; }

        /// <summary>
        ///     Gets the current communication state.
        /// </summary>
        CommunicationStates CommunicationState { get; }

        /// <summary>
        ///     Connects to server.
        /// </summary>
        void Connect();

        /// <summary>
        ///     Disconnects from server.
        ///     Does nothing if already disconnected.
        /// </summary>
        void Disconnect();
    }
}