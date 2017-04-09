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
    ///     This message is sent to invoke a method of a remote application.
    /// </summary>
    // TODO Extend to support additional parameter for target agent
    [Serializable]
    public class AscRemoteInvokeMessage : AscMessage
    {
        /// <summary>
        ///     Name of the remove service class.
        /// </summary>
        public string ServiceInterfaceName { get; set; }

        /// <summary>
        ///     Method of remote application to invoke.
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        ///     Parameters of method.
        /// </summary>
        public object[] Parameters { get; set; }

        /// <summary>
        ///     Guid of the ServiceInstance to call
        /// </summary>
        public Guid ServiceID { get; set; }

        /// <summary>
        ///     Represents this object as string.
        /// </summary>
        /// <returns>String representation of this object</returns>
        public override string ToString()
        {
            return string.Format("AscRemoteInvokeMessage: {0}.{1}(...)", ServiceInterfaceName, MethodName);
        }
    }
}