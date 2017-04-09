//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

using System;
using ASC.Communication.Scs.Communication.Messages;

namespace ASC.Communication.ScsServices.Communication.Messages
{
    /// <summary>
    ///     This message is sent as response message to a AscRemoteInvokeMessage.
    ///     It is used to send return value of method invocation.
    /// </summary>
    [Serializable]
    public class AscRemoteInvokeReturnMessage : AscMessage
    {
        /// <summary>
        ///     Return value of remote method invocation.
        /// </summary>
        public object ReturnValue { get; set; }

        /// <summary>
        ///     If any exception occured during method invocation, this field contains Exception object.
        ///     If no exception occured, this field is null.
        /// </summary>
        public AcsRemoteException RemoteException { get; set; }

        /// <summary>
        /// The serviceObject's ID used to identify the target object
        /// </summary>
        public Guid ServiceID { get; set; }

        /// <summary>
        ///     Represents this object as string.
        /// </summary>
        /// <returns>String representation of this object</returns>
        public override string ToString()
        {
            return string.Format("AscRemoteInvokeReturnMessage: Returns {0}, Exception = {1}", ReturnValue,
                RemoteException);
        }
    }
}