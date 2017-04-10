//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

using System;

namespace Hik.Communication.Scs.Communication.Messages
{
    /// <summary>
    ///     This message is used to send/receive ping messages.
    ///     Ping messages is used to keep connection alive between server and client.
    /// </summary>
    [Serializable]
    public sealed class ScsPingMessage : ScsMessage
    {
        /// <summary>
        ///     Creates a new PingMessage object.
        /// </summary>
        public ScsPingMessage()
        {
        }

        /// <summary>
        ///     Creates a new reply PingMessage object.
        /// </summary>
        /// <param name="repliedMessageId">
        ///     Replied message id if this is a reply for
        ///     a message.
        /// </param>
        public ScsPingMessage(string repliedMessageId)
            : this()
        {
            RepliedMessageId = repliedMessageId;
        }

        /// <summary>
        ///     Creates a string to represents this object.
        /// </summary>
        /// <returns>A string to represents this object</returns>
        public override string ToString()
        {
            return string.IsNullOrEmpty(RepliedMessageId)
                ? string.Format("ScsPingMessage [{0}]", MessageId)
                : string.Format("ScsPingMessage [{0}] Replied To [{1}]", MessageId, RepliedMessageId);
        }
    }
}