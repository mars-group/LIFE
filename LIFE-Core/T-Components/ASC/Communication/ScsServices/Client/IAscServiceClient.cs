//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using ASC.Communication.Scs.Client;

namespace ASC.Communication.ScsServices.Client {
    /// <summary>
    ///     Represents a service client that consumes a SCS service.
    /// </summary>
    /// <typeparam name="T">Type of service interface</typeparam>
    public interface IAscServiceClient<out T> : IConnectableClient where T : class {
        /// <summary>
        ///     Reference to the service proxy to invoke remote service methods.
        /// </summary>
        T ServiceProxy { get; }

        /// <summary>
        ///     Timeout value when invoking a service method.
        ///     If timeout occurs before end of remote method call, an exception is thrown.
        ///     Use -1 for no timeout (wait indefinite).
        ///     Default value: 60000 (1 minute).
        /// </summary>
        int Timeout { get; set; }
    }
}