//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System;

namespace ASC.Communication.Scs.Communication.Messages {
    /// <summary>
    ///     This message is used to send/receive a text as message data.
    /// </summary>
    [Serializable]
    public class AscTextMessage : AscMessage {
        /// <summary>
        ///     Message text that is being transmitted.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        ///     Creates a new AscTextMessage object.
        /// </summary>
        public AscTextMessage() {}

        /// <summary>
        ///     Creates a new AscTextMessage object with Text property.
        /// </summary>
        /// <param name="text">Message text that is being transmitted</param>
        public AscTextMessage(string text) {
            Text = text;
        }

        /// <summary>
        ///     Creates a new reply AscTextMessage object with Text property.
        /// </summary>
        /// <param name="text">Message text that is being transmitted</param>
        /// <param name="repliedMessageId">
        ///     Replied message id if this is a reply for
        ///     a message.
        /// </param>
        public AscTextMessage(string text, string repliedMessageId)
            : this(text) {
            RepliedMessageId = repliedMessageId;
        }

        /// <summary>
        ///     Creates a string to represents this object.
        /// </summary>
        /// <returns>A string to represents this object</returns>
        public override string ToString() {
            return string.IsNullOrEmpty(RepliedMessageId)
                ? string.Format("AscTextMessage [{0}]: {1}", MessageId, Text)
                : string.Format("AscTextMessage [{0}] Replied To [{1}]: {2}", MessageId, RepliedMessageId, Text);
        }
    }
}