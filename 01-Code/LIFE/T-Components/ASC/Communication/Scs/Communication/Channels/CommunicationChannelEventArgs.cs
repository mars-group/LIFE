//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System;

namespace ASC.Communication.Scs.Communication.Channels {
    /// <summary>
    ///     Stores communication channel information to be used by an event.
    /// </summary>
    internal class CommunicationChannelEventArgs : EventArgs {
        /// <summary>
        ///     Communication channel that is associated with this event.
        /// </summary>
        public ICommunicationChannel Channel { get; private set; }

        /// <summary>
        ///     Creates a new CommunicationChannelEventArgs object.
        /// </summary>
        /// <param name="channel">Communication channel that is associated with this event</param>
        public CommunicationChannelEventArgs(ICommunicationChannel channel) {
            Channel = channel;
        }
    }
}