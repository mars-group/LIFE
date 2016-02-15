//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System;

namespace Hik.Communication.Scs.Communication.Messages {
    /// <summary>
    ///     Stores message to be used by an event.
    /// </summary>
    public class MessageEventArgs : EventArgs {
        /// <summary>
        ///     Message object that is associated with this event.
        /// </summary>
        public IScsMessage Message { get; private set; }

        /// <summary>
        ///     Creates a new MessageEventArgs object.
        /// </summary>
        /// <param name="message">Message object that is associated with this event</param>
        public MessageEventArgs(IScsMessage message) {
            Message = message;
        }
    }
}