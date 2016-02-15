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
    ///     Base class for all services that is serviced by IAscServiceApplication.
    ///     A class must be derived from AscService to serve as a SCS service.
    /// </summary>
    public abstract class AscService {
        /// <summary>
        ///     The current client for a thread that called service method.
        /// </summary>
        [ThreadStatic] private static IAscServiceClient _currentClient;

        /// <summary>
        ///     Gets the current client which called this service method.
        /// </summary>
        /// <remarks>
        ///     This property is thread-safe, if returns correct client when
        ///     called in a service method if the method is called by SCS system,
        ///     else throws exception.
        /// </remarks>
        protected internal IAscServiceClient CurrentClient {
            get {
                if (_currentClient == null) {
                    throw new Exception(
                        "Client channel can not be obtained. CurrentClient property must be called by the thread which runs the service method.");
                }

                return _currentClient;
            }

            internal set { _currentClient = value; }
        }

        /// <summary>
        ///     Initializes a new AscService Class with a unique Guid
        /// </summary>
        /// <param name="serviceGuid">Optional parameter to set the Guid of this Service. Usually auto-generated.</param>
        protected AscService(byte[] serviceGuid = null) {
            ServiceID = serviceGuid != null ? new Guid(serviceGuid) : Guid.NewGuid();
        }

        public Guid ServiceID { get; set; }
    }
}