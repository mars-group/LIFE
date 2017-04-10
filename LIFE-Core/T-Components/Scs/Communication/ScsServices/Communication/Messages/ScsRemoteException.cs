//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

using System;

namespace Hik.Communication.ScsServices.Communication.Messages
{
    /// <summary>
    ///     Represents a SCS Remote Exception.
    ///     This exception is used to send an exception from an application to another application.
    /// </summary>
    [Serializable]
    public class ScsRemoteException : Exception
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        public ScsRemoteException()
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        public ScsRemoteException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Contstructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public ScsRemoteException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}