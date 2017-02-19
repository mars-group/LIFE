//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System;

namespace Hik.Communication.ScsServices.Service {
    /// <summary>
    ///     Represents a SCS Service Application that is used to construct and manage a SCS service.
    /// </summary>
    public interface IScsServiceApplication {
        /// <summary>
        ///     This event is raised when a new client connected to the service.
        /// </summary>
        event EventHandler<ServiceClientEventArgs> ClientConnected;

        /// <summary>
        ///     This event is raised when a client disconnected from the service.
        /// </summary>
        event EventHandler<ServiceClientEventArgs> ClientDisconnected;

        /// <summary>
        ///     Starts service application.
        /// </summary>
        void Start();

        /// <summary>
        ///     Stops service application.
        /// </summary>
        void Stop();

        /// <summary>
        ///     Adds a service object to this service application.
        ///     Only single service object can be added for a service interface type.
        /// </summary>
        /// <typeparam name="TServiceInterface">Service interface type</typeparam>
        /// <typeparam name="TServiceClass">
        ///     Service class type. Must be delivered from ScsService and must implement
        ///     TServiceInterface.
        /// </typeparam>
        /// <param name="service">An instance of TServiceClass.</param>
        void AddService<TServiceInterface, TServiceClass>(TServiceClass service)
            where TServiceClass : ScsService, TServiceInterface
            where TServiceInterface : class;

        /// <summary>
        ///     Removes a previously added service object from this service application.
        ///     It removes object according to interface type.
        /// </summary>
        /// <typeparam name="TServiceInterface">Service interface type</typeparam>
        /// <returns>True: removed. False: no service object with this interface</returns>
        bool RemoveService<TServiceInterface>() where TServiceInterface : class;
    }
}