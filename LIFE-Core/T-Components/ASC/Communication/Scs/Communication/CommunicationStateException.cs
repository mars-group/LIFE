//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System;
using System.Runtime.Serialization;

namespace ASC.Communication.Scs.Communication {
    /// <summary>
    ///     This application is thrown if communication is not expected state.
    /// </summary>
    [Serializable]
    public class CommunicationStateException : CommunicationException {
        /// <summary>
        ///     Contstructor.
        /// </summary>
        public CommunicationStateException() {}

        /// <summary>
        ///     Contstructor for serializing.
        /// </summary>
        public CommunicationStateException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context) {}

        /// <summary>
        ///     Contstructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        public CommunicationStateException(string message)
            : base(message) {}

        /// <summary>
        ///     Contstructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public CommunicationStateException(string message, Exception innerException)
            : base(message, innerException) {}
    }
}