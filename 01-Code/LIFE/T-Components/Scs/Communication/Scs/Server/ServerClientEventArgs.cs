//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System;

namespace Hik.Communication.Scs.Server {
    /// <summary>
    ///     Stores client information to be used by an event.
    /// </summary>
    public class ServerClientEventArgs : EventArgs {
        /// <summary>
        ///     Client that is associated with this event.
        /// </summary>
        public IScsServerClient Client { get; private set; }

        /// <summary>
        ///     Creates a new ServerClientEventArgs object.
        /// </summary>
        /// <param name="client">Client that is associated with this event</param>
        public ServerClientEventArgs(IScsServerClient client) {
            Client = client;
        }
    }
}