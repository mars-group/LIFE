//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System;
using ASC.Communication.Scs.Communication.Channels;
using ASC.Communication.Scs.Communication.Channels.Udp;
using ASC.Communication.Scs.Communication.Messengers;
using ASC.Communication.Scs.Communication.Protocols;
using CustomUtilities.Collections;

namespace ASC.Communication.Scs.Server {
    /// <summary>
    ///     Represents a SCS server that is used to accept and manage client connections.
    /// </summary>
    public interface IAscServer {

        /// <summary>
        ///     Gets/sets wire protocol factory to create IWireProtocol objects.
        /// </summary>
        IAcsWireProtocolFactory WireProtocolFactory { get; set; }

        IMessenger GetMessenger();

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