//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System;

namespace ASC.Communication.ScsServices.Service {
    /// <summary>
    ///     Any SCS Service interface class must have this attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public class AscServiceAttribute : Attribute {
        /// <summary>
        ///     Service Version. This property can be used to indicate the code version.
        ///     This value is sent to client application on an exception, so, client application can know that service version is
        ///     changed.
        ///     Default value: NO_VERSION.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        ///     Creates a new AscServiceAttribute object.
        /// </summary>
        public AscServiceAttribute() {
            Version = "NO_VERSION";
        }
    }
}