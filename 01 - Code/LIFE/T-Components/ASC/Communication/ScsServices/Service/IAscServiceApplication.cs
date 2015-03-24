﻿using System;
using ASC.Communication.ScsServices.Communication.Messages;

namespace ASC.Communication.ScsServices.Service {
    /// <summary>
    ///     Represents a SCS Service Application that is used to construct and manage a SCS service.
    /// </summary>
    public interface IAscServiceApplication {
        /// <summary>
        ///     This event is raised when an AddShadowAgentMessage has been received
        /// </summary>
        event EventHandler<AddShadowAgentEventArgs> AddShadowAgentMessageReceived;

        /// <summary>
        ///     This event is raised when an RemoveShadowAgentMessage has been received
        /// </summary>
        event EventHandler<RemoveShadowAgentEventArgs> RemoveShadowAgentMessageReceived;

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
        ///     Service class type. Must be derived from AscService and must implement
        ///     TServiceInterface.
        /// </typeparam>
        /// <param name="service">An instance of TServiceClass.</param>
        void AddService<TServiceInterface, TServiceClass>(TServiceClass service)
            where TServiceClass : AscService, TServiceInterface
            where TServiceInterface : class;

        /// <summary>
        ///     Removes a previously added service object from this service application.
        ///     It removes object according to interface type.
        /// </summary>
        /// <typeparam name="TServiceInterface">Service interface type</typeparam>
        /// <returns>True: removed. False: no service object with this interface</returns>
        bool RemoveService<TServiceInterface>() where TServiceInterface : class;

        bool RemoveService<TServiceInterface>(Guid serviceGuid) where TServiceInterface : class;
    }
}